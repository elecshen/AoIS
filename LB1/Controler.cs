using Microsoft.Extensions.Configuration;

namespace LB1
{
	public enum ViewStates
	{
		ModelSelect,
		ShowTable,
	}

	public class Controler
	{
		private static Controler? instance;
		public static Controler GetInstance()
		{
			instance ??= new();
			return instance;
		}

		public event Action? UpdateTable;
		public List<KeyValuePair<string, Type?>> ModelTypes { get; }
		public ViewStates ViewState { get; private set; }
		public bool IsStateChanged { get; set; }
		public int ChoosedModel { get; set; }
		public int CellTop { get; set; }
		public int CellLeft { get; set; }

		public readonly List<string[]> Layout;
		public readonly List<string> Messages;
		public List<string> ChoosedEntry { get; }
		public int ChoosedEntryKey { get; set; }
		public bool IsEditing { get; set; }

		public IModel? Model { get; private set; }
		private readonly IConfigurationRoot configuration;

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
			Messages = new();

			configuration = new ConfigurationBuilder()
				.AddIniFile(new FileInfo("..\\..\\..\\conf.ini").FullName)
				.Build();
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

		public void SetModel()
		{
			if(ModelTypes[ChoosedModel].Value is null)
			{
				SwitchScreenTo(ViewStates.ModelSelect);
				return;
			}
			var section = configuration.GetSection(ModelTypes[ChoosedModel].Value!.ToString().Split('.').Last());
			var path = section["CSVFilePath"] ?? "";
			var objType = Type.GetType("LB1." + section["CSVObjectTypeName"]) ?? Type.Missing;
			try
			{
				Model = (IModel?)Activator.CreateInstance(ModelTypes[ChoosedModel].Value!, new object[] { path, objType });
				UpdateTable?.Invoke();
				SwitchScreenTo(ViewStates.ShowTable);
			}
			catch (Exception ex)
			{
				Messages.Add(ex.Message);
				SwitchScreenTo(ViewStates.ModelSelect);
			}
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

		public int CountOfField()
		{
			if (!IsModelSelected())
				return 0;
			return Model!.CountOfFields();
		}

		public List<string[]> GetTable()
		{
			if (IsModelSelected())
				return Model!.GetValues().Select(e => e.ToString()!.Split(';')).ToList();
			return new();
		}

		public bool AddEntry()
		{
			if (!IsModelSelected())
				return false;
			try
			{
				Model!.AddEntry(ChoosedEntry.Skip(1).ToArray());
				UpdateTable?.Invoke();
				return true;
			}
			catch (Exception ex)
			{
				if (ex is InvalidArrayLengthException || ex is FormatException)
				{
					Messages.Add(ex.Message);
				}
				return false;
			}
		}

		public bool EditEntry()
		{
			if (!IsModelSelected())
				return false;
			try
			{
				Model!.EditEntry(ChoosedEntryKey, ChoosedEntry.Skip(1).ToArray());
				UpdateTable?.Invoke();
				return true;
			}
			catch (Exception ex)
			{
				if(ex is InvalidArrayLengthException || ex is FormatException)
				{
					Messages.Add(ex.Message);
				}
				return false;
			}
		}

		public void RemoveEntry()
		{
			if (!IsModelSelected())
				return;
			try
			{
				Model!.RemoveEntry(CellTop);
				UpdateTable?.Invoke();
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Messages.Add(ex.Message);
			}
		}

		public void ChooseEntry(string startStr)
		{
			ChoosedEntry.Clear();
			ChoosedEntryKey = CellTop;
			if (startStr == "+")
				ChoosedEntry.AddRange(new string[Model!.CountOfFields() + 1]);
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
			if (ChoosedEntry.FirstOrDefault() == "+" && AddEntry()
				|| ChoosedEntry.FirstOrDefault() == "~" && EditEntry())
				Button__ExitEdit();
		}

		public void Button__ExitEdit()
		{
			IsEditing = false;
			UpdateTable?.Invoke();
		}
	}
}
