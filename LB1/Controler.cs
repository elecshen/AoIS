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

		public event Action? UpdateTable;
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
		public bool IsStateChanged { get; set; }
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

		public readonly List<string[]> Layout;
		public readonly List<string> Messages;
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
			Layout = new();
			ChoosedEntry = new();
			IsEditing = false;
			IsStateChanged = false;
			Messages = new List<string>();
		}

		public void SwitchScreenTo(ViewStates state)
		{
			ViewState = state;
			IsStateChanged = true;
		}

		public string[] GetModelTypes()
		{
			string[] models = new string[ModelTypes.Count];
			for (int i = 0; i < ModelTypes.Count; i++)
			{
				models[i] = ModelTypes[i].Key;
				if (ModelTypes[i].Value is null)
					models[i] += "(не поддерживается)";
			}
			return models;
		}

		public bool SetModel(int choosedModel)
		{
			if(ModelTypes[choosedModel].Value is null) 
				return false;
			Model = (Model?)Activator.CreateInstance(ModelTypes[choosedModel].Value!, new object[] { typeof(CSVObject) });
			if (Model == null)
				return false;
			return true;
		}

		private bool IsModelSelected()
		{
			if (Model is null)
			{
				SwitchScreenTo(ViewStates.ModelSelect);
				return false;
			}
			return true;
		}

		public void SetCSVPath(string? path)
		{
			if (!IsModelSelected())
				return;
			if (path is null)
			{
				SwitchScreenTo(ViewStates.CSVFileWrongPath);
				return;
			}
			if (Model is CSVModel model)
			{
				model.PathCSVFile = path;
				if (model.UploadTable())
				{
					UpdateTable?.Invoke();
					SwitchScreenTo(ViewStates.ShowTable);
				}
				else
					SwitchScreenTo(ViewStates.CSVFileWrongPath);
				return;
			}
			else
				throw new Exception("Invalid model type");
		}

		public List<string[]> GetTable()
		{
			if (IsModelSelected())
				return Model!.Table.Table.Select(e => e.ToString()!.Split(';')).ToList();
			return new();
		}

		public List<Type> GetTypes()
		{
			if (IsModelSelected())
				return Model!.Table.Types.ToList();
			return new();
		}

		public bool AddEntry()
		{
			if (!IsModelSelected())
				return false;
			object[] props = new object[ChoosedEntry.Count - 1];
			try
			{
				for (int i = 1; i < ChoosedEntry.Count; i++)
				{
					props[i - 1] = Validator.ConvertToType(GetTypes()[i - 1], ChoosedEntry[i]);
				}
				object obj = Activator.CreateInstance(Model!.Table.ObjectsType, props)!;
				if (Model.AddEntry(obj))
				{
					UpdateTable?.Invoke();
					return true;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}

		public bool EditEntry()
		{
			if (!IsModelSelected())
				return false;
			object[] props = new object[ChoosedEntry.Count - 1];
			for (int i = 1; i < ChoosedEntry.Count; i++)
			{
				props[i-1] = Validator.ConvertToType(GetTypes()[i - 1], ChoosedEntry[i]);
			}
			try
			{
				object obj = Activator.CreateInstance(Model!.Table.ObjectsType, props)!;
				if (Model.EditEntry(EntryKey, obj))
				{
					UpdateTable?.Invoke();
					return true;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}

		public bool RemoveEntry()
		{
			if (!IsModelSelected())
				return false;
			if (Model!.RemoveEntry(Model.FindEntry(CellTop)))
			{
				UpdateTable?.Invoke();
				return true;
			}
			return false;
		}

		public void ChooseEntry(string startStr)
		{
			ChoosedEntry.Clear();
			EntryKey = CellTop;
			if (startStr == "+")
				ChoosedEntry.AddRange(new string[GetTypes().Count + 1]);
			else
			{
				ChoosedEntry.AddRange(Layout[CellTop]);
				ChoosedEntry.Remove(ChoosedEntry.Last());
			}
			ChoosedEntry[0] = startStr;
		}
		public void Button__AddEntry()
		{
			ChooseEntry("+");
			IsEditing = true;
			UpdateTable?.Invoke();
		}

		public void Button__EditEntry()
		{
			ChooseEntry("~");
			IsEditing = true;
			UpdateTable?.Invoke();
		}

		public void Button__RemoveEntry()
		{
			RemoveEntry();
		}

		public void Button__SaveEdit()
		{
			if (ChoosedEntry.FirstOrDefault() == "+")
			{
				if (AddEntry())
				{
					Button__ExitEdit();
					return;
				}
			}
			else
			{
				if (EditEntry())
				{
					Button__ExitEdit();
					return;
				}
			}
			Messages.Add("Сохранение не удалось.");
		}

		public void Button__ExitEdit()
		{
			IsEditing = false;
			UpdateTable?.Invoke();
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}
}
