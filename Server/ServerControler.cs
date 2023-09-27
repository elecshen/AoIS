using Microsoft.Extensions.Configuration;
using System.Data;
using System.IO;
using TableReader.NetControler;

namespace TableReader.Server
{
	public class ServerControler
	{
		private static ServerControler? instance;
		public static ServerControler GetInstance()
		{
			instance ??= new();
			return instance;
		}

		public List<KeyValuePair<string, Type?>> ModelTypes { get; }

		public IModel? Model { get; private set; }
		private readonly IConfigurationRoot configuration;

		private ServerControler()
		{
			configuration = new ConfigurationBuilder()
				.AddIniFile(new FileInfo("..\\..\\..\\conf.ini").FullName)
				.Build();

			ModelTypes = new();
			foreach (var type in (configuration["ModelTypes"] ?? "").Split(",").Select(x => x.Split(",")))
			{
				ModelTypes.Add(new(type[0], Type.GetType("LB1" + type[1])));
			};
		}

		public IMessage GetModelTypes()
		{
			string[] models = new string[ModelTypes.Count];
			for (int i = 0; i < ModelTypes.Count; i++)
			{
				models[i] = ModelTypes[i].Key;
				if (ModelTypes[i].Value is null)
					models[i] += "(не поддерживается)";
			}
			return new ModelTypesMessage(models.ToList());
		}

		public void SetModel(SetModelMessage message)
		{
			int ChoosedModel = int.Parse(message.Content[0]);
			IMessage answer;
			if (ModelTypes[ChoosedModel].Value is null)
			{
				answer = new ErrorMessage("");
			}
			else
			{
				try
				{
					Model = (IModel?)Activator.CreateInstance(
						ModelTypes[ChoosedModel].Value!, 
						new object[] {
							message.Content[0],
							message.Content[0] });
					answer = new TableMessage(GetTable().ToArray());
				}
				catch (Exception ex)
				{
					Messages.Add(ex.Message);
					SwitchScreenTo(ViewStates.ModelSelect);
				}
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
				if (ex is InvalidArrayLengthException || ex is FormatException)
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
