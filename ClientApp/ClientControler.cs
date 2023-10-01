using Microsoft.Extensions.Configuration;
using NetControler;

namespace ClientApp
{
	public enum ViewStates
	{
		ModelSelect,
		ObjectSelect,
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
		public bool IsStateChanged { get; set; }
		public int ChoosedModel { get; set; }
		public int ChoosedObject { get; set; }
		public int CellTop { get; set; }
		public int CellLeft { get; set; }
		public List<string> ChoosedEntry { get; }
		public int ChoosedEntryKey { get; set; }
		public bool IsEditing { get; set; }

		private readonly NetClient netClient;
		private Dictionary<string, string[]> ModelConfigs;
		private string[] modelData;
		private List<string[]> Table;
		public List<string[]> Layout {  get; }
		public List<string> Errors {  get; }

		private ClientControler()
		{
			IsStateChanged = false;
			ChoosedEntry = new();
			IsEditing = false;

			netClient = new("127.0.0.1", 8080);
			ModelConfigs = new();
			modelData = new string[2];
			Table = new();
			Layout = new();
			Errors = new();
		}

		public void SwitchScreenTo(ViewStates state)
		{
			ViewState = state;
			IsStateChanged = true;
		}

		public List<string> GetModels()
		{
			Message answer = netClient.SendRequest(new Message(MessageHeader.GetModelTypes));
			if (answer.Header == MessageHeader.ModelTypesList && answer.Content is Dictionary<string, string[]> content)
			{
				ModelConfigs = content;
				List<string> result = new(ModelConfigs.Select(m => m.Key));
				return result;
			}
			Errors.Add("Bad message");
			return new();
		}

		public List<string> GetModelObjects()
		{
			List<string> result = new(ModelConfigs.Values.ElementAt(ChoosedModel).ToList());
			return result;
		}

		public bool SetModel()
		{
			string[] messData = new string[] { ModelConfigs.ElementAt(ChoosedModel).Key, ModelConfigs!.ElementAt(ChoosedModel).Value[ChoosedObject] };
			Message answer = netClient.SendRequest(new Message(MessageHeader.ModelParamsList, new string[2], messData.GetType(), messData));
			if (UpdateTableOrShowError(answer))
			{
				modelData = answer.ModelData;
				return true;
			}
			SwitchScreenTo(ViewStates.ModelSelect);
			return false;
		}

		private bool UpdateTableOrShowError(Message message)
		{
			if (message.Header == MessageHeader.TableContent && message.Content is List<string[]> content)
			{
				Table = content;
				UpdateTable?.Invoke();
				SwitchScreenTo(ViewStates.ShowTable);
				return true;
			}
			else if (message.Header == MessageHeader.Error && message.Content is string error)
			{
				Errors.Add(error);
			}
			return false;
		}

		public int CountOfFields()
		{
			if (Table.Count > 0)
				return Table[0].Length;
			return 0;
		}

		public List<string[]> GetTable()
		{
			if (Table.Count > 0)
				return Table;
			return new();
		}

		public bool AddEntry()
		{
			string[] messData = ChoosedEntry.GetRange(1, ChoosedEntry.Count - 1).ToArray();
			Message answer = netClient.SendRequest(new Message(MessageHeader.AddEntry, modelData, messData.GetType(), messData));
			return UpdateTableOrShowError(answer);
		}

		public bool EditEntry()
		{
			List<string> messData = new(ChoosedEntry)
			{
				[0] = ChoosedEntryKey.ToString()
			};
			Message answer = netClient.SendRequest(new Message(MessageHeader.EditEntry, modelData, messData.GetType(), messData));
			return UpdateTableOrShowError(answer);
		}

		public bool RemoveEntry()
		{
			string messData = CellTop.ToString();
			Message answer = netClient.SendRequest(new Message(MessageHeader.RemoveEntry, modelData, messData.GetType(), messData));
			return UpdateTableOrShowError(answer);
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
