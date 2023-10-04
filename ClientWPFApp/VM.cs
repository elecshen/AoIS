using NetControler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Dynamic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using ClientWPFApp.Views;

namespace ClientWPFApp
{
	public class VM : INotifyPropertyChanged
	{
		private static VM? instanceVM;
		public static VM InstanceVM
		{
			get => instanceVM ??= new VM();
		}

		public Frame? frame;
		private readonly Page[] pages;

		private string? choosedModel;
		public string? ChoosedModel
		{
			get { return choosedModel; }
			set { choosedModel = value; OnPropertyChanged(); }
		}

		private List<string> modelList;
		public List<string> ModelList
		{
			get { return modelList; }
			set { modelList = value; OnPropertyChanged(); }
		}

		private string? choosedObject;
		public string? ChoosedObject
		{
			get { return choosedObject; }
			set { choosedObject = value; OnPropertyChanged(); }
		}

		private List<string> objectList;
		public List<string> ObjectList
		{
			get { return objectList; }
			set { objectList = value; OnPropertyChanged(); }
		}

		private Visibility tableButtonVisibility;
		public Visibility TableButtonVisibility
		{
			get { return tableButtonVisibility; }
			set { tableButtonVisibility = value; OnPropertyChanged(); }
		}

		private Visibility editingButtonVisibility;
		public Visibility EditingButtonVisibility
		{
			get { return editingButtonVisibility; }
			set { editingButtonVisibility = value; OnPropertyChanged(); }
		}

		private int choosedEntryKey;
		public int ChoosedEntryKey
		{
			get { return choosedEntryKey; }
			set { choosedEntryKey = value; OnPropertyChanged(); }
		}
		private bool IsEditingEntryNew;
		public ObservableCollection<string> EntryToEdit {  get; }

		private Dictionary<string, string[]> modelConfigs;
		private string[] modelData;

		private readonly ObservableCollection<string[]> table;
		public ReadOnlyObservableCollection<string[]> Table { get; }
		private readonly ObservableCollection<ExpandoObject> gridTable;
		public ReadOnlyObservableCollection<ExpandoObject> GridTable { get; }
		private readonly ObservableCollection<ExpandoObject> gridEditTable;
		public ReadOnlyObservableCollection<ExpandoObject> GridEditTable { get; }

		public DataGrid DataGridTable;
		public DataGrid DataGridEdit;

		private VM()
		{
			TableButtonVisibility = Visibility.Visible;
			EditingButtonVisibility = Visibility.Collapsed;
			ChoosedEntryKey = 0;
			pages = new Page[]
			{
				new TablePage(),
			};
			pages[0].DataContext = this;
			DataGridTable = ((TablePage)pages[0]).TableG;
			DataGridEdit = ((TablePage)pages[0]).TableE;

			modelList = new();
			objectList = new();
			EntryToEdit = new();

			modelConfigs = new();
			modelData = new string[2];
			table = new();
			Table = new(table);
			gridTable = new();
			GridTable = new(gridTable);
			gridEditTable = new();
			GridEditTable = new(gridEditTable);

			PropertyChanged += MakeObjectList;
			table.CollectionChanged += FillDataGridTable;

			GetModels();
		}

		private void GetModels()
		{
			Message answer = NetClient.SendRequest(new Message(MessageHeader.GetModelTypes));
			if (answer.Header == MessageHeader.ModelTypesList && answer.Content is Dictionary<string, string[]> content)
			{
				modelConfigs = content;
				ModelList = new(modelConfigs.Select(m => m.Key));
				return;
			}
			ModelList = new();
		}

		private void MakeObjectList(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ChoosedModel) && ChoosedModel is not null)
			{
				ObjectList = new(modelConfigs[ChoosedModel]);
			}
		}

		private bool UpdateTableOrShowError(Message message)
		{
			if (message.Header == MessageHeader.TableContent && message.Content is List<string[]> content)
			{
				table.Clear();
				foreach (var item in content)
					table.Add(item);
				return true;
			}
			else if (message.Header == MessageHeader.Error && message.Content is string error)
			{
				MessageBox.Show(error);
			}
			return false;
		}

		public bool AddEntry(IEnumerable<string> entry)
		{
			string[] messData = entry.ToArray();
			Message answer = NetClient.SendRequest(new Message(MessageHeader.AddEntry, modelData, messData.GetType(), messData));
			return UpdateTableOrShowError(answer);
		}

		public bool EditEntry(IEnumerable<string> entry)
		{
			List<string> messData = new() { ChoosedEntryKey.ToString() };
			messData.AddRange(entry);
			Message answer = NetClient.SendRequest(new Message(MessageHeader.EditEntry, modelData, messData.GetType(), messData));
			return UpdateTableOrShowError(answer);
		}

		private Command? setModelCommand;
		public Command SetModelCommand
		{
			get => setModelCommand ??= new Command(obj =>
			{
				if (ChoosedModel is null || ChoosedObject is null)
				{
					MessageBox.Show("Модель и объект не выбраны!");
					return;
				}
				string[] messData = new string[2] { ChoosedModel, ChoosedObject };
				Message answer = NetClient.SendRequest(new Message(MessageHeader.ModelParamsList, new string[2], messData.GetType(), messData));
				if (UpdateTableOrShowError(answer))
				{
					MakeDataGridHeaders();
					modelData = answer.ModelData;
					frame?.Navigate(pages[0]);
				}
			});
		}

		private Command? addCommand;
		public Command AddCommand
		{
			get => addCommand ??= new Command(obj =>
			{
				DataGridEdit.Visibility = Visibility.Visible;
				TableButtonVisibility = Visibility.Collapsed;
				EditingButtonVisibility = Visibility.Visible;
				IsEditingEntryNew = true;
				SetEntryToEdit();
			});
		}

		private Command? editCommand;
		public Command EditCommand
		{
			get => editCommand ??= new Command(obj =>
			{
				DataGridEdit.Visibility = Visibility.Visible;
				TableButtonVisibility = Visibility.Collapsed;
				EditingButtonVisibility = Visibility.Visible;
				IsEditingEntryNew = false;
				SetEntryToEdit();
			});
		}

		private Command? removeCommand;
		public Command RemoveCommand
		{
			get => removeCommand ??= new Command(obj =>
			{
				string messData = ChoosedEntryKey.ToString();
				Message answer = NetClient.SendRequest(new Message(MessageHeader.RemoveEntry, modelData, messData.GetType(), messData));
				UpdateTableOrShowError(answer);
			});
		}

		private Command? saveCommand;
		public Command SaveCommand
		{
			get => saveCommand ??= new Command(obj =>
			{
				IEnumerable<string> entry = ((IDictionary<string, object>)DataGridEdit.Items[0]).Select(e => (string)e.Value);
				if (IsEditingEntryNew && AddEntry(entry)
					|| EditEntry(entry))
					ExitCommand.Execute(null);
			});
		}

		private Command? exitCommand;
		public Command ExitCommand
		{
			get => exitCommand ??= new Command(obj =>
			{
				DataGridEdit.Visibility = Visibility.Collapsed;
				TableButtonVisibility = Visibility.Visible;
				EditingButtonVisibility = Visibility.Collapsed;
			});
		}

		private void MakeDataGridHeaders()
		{
			if (DataGridTable is null || DataGridEdit is null)
				return;
			DataGridTable.Columns.Clear();
			DataGridEdit.Columns.Clear();
			DataGridTextColumn textColumn;
			Style style = new();
			style.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(15, 4, 15, 4)));
			for (int i = 0; Table.Count > 0 && i < Table[0].Length; i++)
			{
				textColumn = new()
				{
					Header = i.ToString(),
					Binding = new Binding(i.ToString()),
					ElementStyle = style
				};
				DataGridTable.Columns.Add(textColumn);
				textColumn = new()
				{
					Header = i.ToString(),
					Binding = new Binding(i.ToString()),
					ElementStyle = style
				};
				DataGridEdit.Columns.Add(textColumn);
			}
		}

		private void FillDataGridTable(object? sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					gridTable.Add(MakeEntryObject(table[e.NewStartingIndex]));
					break;
				case NotifyCollectionChangedAction.Remove:
					gridTable.RemoveAt(e.OldStartingIndex);
					break;
				case NotifyCollectionChangedAction.Reset:
					gridTable.Clear();
					foreach (var item in table)
						gridTable.Add(MakeEntryObject(item));
					break;
			}
		}

		private void SetEntryToEdit()
		{
			ExpandoObject obj;
			if (IsEditingEntryNew)
				obj = MakeEntryObject(new string[table[0].Length]);
			else
				obj = MakeEntryObject(table[ChoosedEntryKey]);
			gridEditTable.Clear();
			gridEditTable.Add(obj);
		}

		private ExpandoObject MakeEntryObject(string[] entry)
		{
			ExpandoObject obj = new();
			for (int i = 0; i < entry.Length; i++)
			{
				((IDictionary<string, object>)obj!).Add(i.ToString(), entry[i]);
			}
			return obj;
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}
}
