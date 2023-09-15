using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LB1
{
	public enum ViewStates
	{
		ModelSelect,
		CSVFilePath,
		CSVFileWrongPath,
		ShowTable,
	}

	public class Controler : INotifyPropertyChanged
	{
		private static Controler? instance;
		public static Controler GetInstance()
		{
			instance ??= new();
			return instance;
		}

		public List<KeyValuePair<string, Type?>> ModelTypes { get; }
		private ViewStates viewState;
		public ViewStates ViewState
		{
			get { return viewState; }
			private set
			{
				viewState = value;
				OnPropertyChanged();
			}
		}
		private int choosedModel = 0;
		public int ChoosedModel
		{
			get { return choosedModel; }
			set
			{
				choosedModel = value;
				OnPropertyChanged();
			}
		}
		private int cellTop = 0;
		public int CellTop
		{
			get { return cellTop; }
			set
			{
				cellTop = value;
				OnPropertyChanged();
			}
		}
		private int cellLeft = 0;
		public int CellLeft
		{
			get { return cellLeft; }
			set 
			{ 
				cellLeft = value; 
				OnPropertyChanged(); 
			}
		}
		public List<string> ChoosedEntry { get; }
		public int EntryKey { get; set; }
		public bool IsEditing { get; set; }

		public Model? Model { get; private set; }

		private Controler()
		{
			ModelTypes = new()
			{
				new( "CSV файл", typeof(CSVModel) ),
				new( "MS SQL", null ),
				new( "Extra model", null)
			};
			ChoosedEntry = new();
			IsEditing = false;
		}

		public void SwitchScreenTo(ViewStates state)
		{
			ViewState = state;
		}

		public bool SetModel(int choosedModel)
		{
			if(ModelTypes[choosedModel].Value is null) 
				return false;
			Model = (dynamic?)Activator.CreateInstance(ModelTypes[choosedModel].Value!, new object[] { typeof(CSVObject) });
			if (Model == null)
				return false;
			if ((dynamic)Model is CSVModel)
				((CSVModel)Model!).PropertyChanged += CheckPath;
			return true;
		}

		public void CheckPath(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "PathCSVFile")
			{
				if (((CSVModel)Model!).UploadTable())
				{
					SwitchScreenTo(ViewStates.ShowTable);
				}
				else
				{
					SwitchScreenTo(ViewStates.CSVFileWrongPath);
				}
			}
		}

		public ModelTable? GetModelTable()
		{
			if (Model !=  null)
				return Model.Table;
			return null;
		}

		public bool AddEntry()
		{

			object[] props = new object[ChoosedEntry.Count - 1];
			try
			{
				for (int i = 1; i < ChoosedEntry.Count; i++)
				{
					props[i - 1] = Validator.ConvertToType(Model!.Table.Types[i - 1], ChoosedEntry[i]);
				}
				object obj = Activator.CreateInstance(Model!.Table.ObjectsType, props)!;
				return Model.AddEntry(obj);
			}
			catch
			{
				return false;
			}
		}

		public bool EditEntry()
		{
			object[] props = new object[ChoosedEntry.Count - 1];
			for (int i = 1; i < ChoosedEntry.Count; i++)
			{
				props[i-1] = Validator.ConvertToType(Model!.Table.Types[i - 1], ChoosedEntry[i]);
			}
			try
			{
				object obj = Activator.CreateInstance(Model!.Table.ObjectsType, props)!;
				return Model.EditEntry(EntryKey, obj);
			}
			catch
			{
				return false;
			}
		}

		public void RemoveEntry()
		{
			Model?.RemoveEntry(Model.FindEntry(CellTop));
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}
}
