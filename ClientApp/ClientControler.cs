using Microsoft.Extensions.Configuration;
using TableReader.NetControler;

namespace TableReader.ClientApp
{
	public enum ViewStates
	{
		ModelSelect,
		ShowTable,
	}

	public class ClientControler
	{
		private static ClientControler? instance;
		public static ClientControler GetInstance()
		{
			instance ??= new();
			return instance;
		}

		public event Action? UpdateTable;
		public ViewStates ViewState { get; private set; }
		public List<KeyValuePair<string, Type?>> ModelTypes { get; }
		public bool IsStateChanged { get; set; }
		public int ChoosedModel { get; set; }
		public int CellTop { get; set; }
		public int CellLeft { get; set; }

		public readonly List<string[]> Layout;
		public readonly List<string> Errors;
		public List<string> ChoosedEntry { get; }
		public int ChoosedEntryKey { get; set; }
		public bool IsEditing { get; set; }
		private readonly IConfigurationRoot configuration;
		public NetClient netClient;

		public string[][]? Table;

		private ClientControler()
		{
			ModelTypes = new();
			Layout = new();
			ChoosedEntry = new();
			IsEditing = false;
			IsStateChanged = false;
			Errors = new();
			configuration = new ConfigurationBuilder()
				.AddIniFile(new FileInfo("..\\..\\..\\conf.ini").FullName)
				.Build();
			netClient = new("127.0.0.1", 8888);
		}

		public void SwitchScreenTo(ViewStates state)
		{
			ViewState = state;
			IsStateChanged = true;
		}

		public List<string> GetModelTypes()
		{
			IMessage answer = netClient.SendRequest(new ModelTypesMessage(new List<string>()));
			return ((ModelTypesMessage)answer).Content;
		}

		public bool SetModel()
		{
			var section = configuration.GetSection(ModelTypes[ChoosedModel].Value!.ToString().Split('.').Last());
			List<string> messData = new()
			{
				ChoosedModel.ToString(),
				section["CSVFilePath"] ?? "noPath",
				section["CSVObjectTypeName"] ?? "noObjType",
			};
			IMessage answer = netClient.SendRequest(new SetModelMessage(messData));
			if(answer is TableMessage message)
			{
				Table = message.Content;
				SwitchScreenTo(ViewStates.ShowTable);
				return true;
			}
			else if(answer is ErrorMessage ex)
			{
				Errors.Add(ex.Content);
			}
			SwitchScreenTo(ViewStates.ModelSelect);
			return false;
		}

		public int CountOfFields()
		{
			if (Table is not null && Table.Length > 0)
				return Table[0].Length;
			return 0;
		}

		public List<string[]> GetTable()
		{
			if (Table is not null && Table.Length > 0)
				return Table.ToList();
			return new();
		}

		public bool AddEntry()
		{
			List<string> messData = new(ChoosedEntry)
			{
				[0] = "-1"
			};
			IMessage answer = netClient.SendRequest(new EntryMessage(messData));
			if(answer is TableMessage message)
			{
				Table = message.Content;
				UpdateTable?.Invoke();
				return true;
			}
			else if(answer is ErrorMessage ex)
			{
				Errors.Add(ex.Content);
			}
			return false;
		}

		public bool EditEntry()
		{
			List<string> messData = new(ChoosedEntry)
			{
				[0] = ChoosedEntryKey.ToString()
			};
			IMessage answer = netClient.SendRequest(new EntryMessage(messData));
			if (answer is TableMessage message)
			{
				Table = message.Content;
				UpdateTable?.Invoke();
				return true;
			}
			else if (answer is ErrorMessage ex)
			{
				Errors.Add(ex.Content);
			}
			return false;
		}

		public bool RemoveEntry()
		{
			string messData = CellTop.ToString();
			IMessage answer = netClient.SendRequest(new RemoveEntryMessage(messData));
			if (answer is TableMessage message)
			{
				Table = message.Content;
				UpdateTable?.Invoke();
				return true;
			}
			else if (answer is ErrorMessage ex)
			{
				Errors.Add(ex.Content);
			}
			return false;
		}

		public void ChooseEntry(string startStr)
		{
			ChoosedEntry.Clear();
			ChoosedEntryKey = CellTop;
			if (startStr == "+")
				ChoosedEntry.AddRange(new string[CountOfFields() + 1]);
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
		{//SwichScreen?
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
