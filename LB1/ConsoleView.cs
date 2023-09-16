using System.ComponentModel;

namespace LB1
{
	public class ConsoleView
	{
		private readonly Controler con;

		private readonly Dictionary<Type, int> offsetsList;
		private readonly List<int> offset = new();
		private readonly List<int> offsetProg = new();
		private readonly List<string> buttonsText;
		private readonly List<int> offsetProgButton;
		private readonly List<string[]> layout;
		private readonly List<Action[]> layoutButtons;
		private readonly List<string> messages;


		public ConsoleView(Controler controler)
		{
			con = controler;
			con.PropertyChanged += SetScreen;
			offsetsList = new()
				{
					{typeof(int), 4},
					{typeof(double), 8},
					{typeof(bool), -7},
					{typeof(string), -15},
				};
			buttonsText = new() { "Добавить", "Сохранить", "Отменить", "Удалить" };
			offsetProgButton = new();
			layout = new();
			layoutButtons = new();
			messages = new();
		}

		public void Run()
		{
			con.SwitchScreenTo(ViewStates.ModelSelect);
			ConsoleKey pressedKey = ConsoleKey.NoName;
			while (true)
			{
				pressedKey = Console.ReadKey().Key;
				if (pressedKey == ConsoleKey.Escape)
					break;

				switch (con.ViewState)
				{
					case ViewStates.ModelSelect:
						if (ChooseModel(pressedKey))
						{
							con.Model!.PropertyChanged += PrepareInterface;
							con.SwitchScreenTo(ViewStates.CSVFilePath);
						}
						else
							con.SwitchScreenTo(ViewStates.ModelSelect);
						break;
					case ViewStates.ShowTable:
						ChooseCell(pressedKey);
						con.SwitchScreenTo(ViewStates.ShowTable);
						break;
				}
			}
		}

		private bool ChooseModel(ConsoleKey key)
		{
			if (key == ConsoleKey.Enter)
			{
				return con.SetModel(con.ChoosedModel);
			}
			if (key == ConsoleKey.DownArrow)
				con.ChoosedModel++;
			else if (key == ConsoleKey.UpArrow)
				con.ChoosedModel--;

			if (con.ChoosedModel > con.ModelTypes.Count - 1)
				con.ChoosedModel = 0;
			if (con.ChoosedModel < 0)
				con.ChoosedModel = con.ModelTypes.Count - 1;
			return false;
		}

		private bool ChooseCell(ConsoleKey key)
		{
			if (key == ConsoleKey.Enter)
			{
				return DoTableAction();
			}
			if (key == ConsoleKey.DownArrow)
				con.CellTop++;
			else if (key == ConsoleKey.UpArrow)
				con.CellTop--;
			else if (key == ConsoleKey.RightArrow)
				con.CellLeft++;
			else if (key == ConsoleKey.LeftArrow)
				con.CellLeft--;

			CheckCellBorders();
			return false;
		}

		private void CheckCellBorders()
		{
			if (con.IsEditing)
			{
				if (con.CellTop < layout.Count - 2)
					con.CellTop = layout.Count - 2;
			}
			else if (con.CellTop < 0)
				con.CellTop = 0;
			if (con.CellTop > layout.Count - 1)
				con.CellTop = layout.Count - 1;
			if (con.CellLeft > layout[con.CellTop].Length - 1)
				con.CellLeft = layout[con.CellTop].Length - 1;
			else if (con.CellLeft < 0)
				con.CellLeft = 0;
		}

		private bool DoTableAction()
		{
			if (layoutButtons[con.CellTop][con.CellLeft] is null)
				return false;
			layoutButtons[con.CellTop][con.CellLeft].Invoke();
			return true;
		}

		private void ChooseEntry(string startStr)
		{
			con.ChoosedEntry.Clear();
			con.EntryKey = con.CellTop;
			if (startStr == "+")
				con.ChoosedEntry.AddRange(new string[con.GetModelTable()!.Types.Count + 1]);
			else
			{
				con.ChoosedEntry.AddRange(layout[con.CellTop]);
				con.ChoosedEntry.Remove(con.ChoosedEntry.Last());
			}
			con.ChoosedEntry[0] = startStr;
		}

		private void Button__AddEntry()
		{
			ChooseEntry("+");
			con.IsEditing = true;
			PrepareInterface();
		}

		private void Button__EditEntry()
		{
			ChooseEntry("~");
			con.IsEditing = true;
			PrepareInterface();
		}

		private void Button__EditCell()
		{
			Draw__TableSelector(true);
			con.ChoosedEntry[con.CellLeft] = Draw__Input();
			UpdateLayout();
		}

		private void Button__SaveEdit()
		{
			if (con.ChoosedEntry.FirstOrDefault() == "+")
			{
				if (con.AddEntry())
				{
					Button__ExitEdit();
					return;
				}
			}
			else
			{
				if (con.EditEntry())
				{
					Button__ExitEdit();
					return;
				}
			}
			messages.Add("Сохранение не удалось.");
		}

		private void Button__ExitEdit()
		{
			con.IsEditing = false;
			PrepareInterface();
		}

		private void SetScreen(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ViewState")
			{

				Console.Clear();
				switch (con.ViewState)
				{
					case ViewStates.ModelSelect:
						Draw__ChooseModelType();
						break;
					case ViewStates.CSVFilePath:
						Draw__EnterFilePath(false);
						break;
					case ViewStates.CSVFileWrongPath:
						Draw__EnterFilePath(true);
						break;
					case ViewStates.ShowTable:
						Draw__ShowTable();
						break;
				}
			}
		}

		private void Draw__ChooseModelType()
		{
			Console.WriteLine("Выберите модель для работы с данными:");
			string text;
			foreach (var modelType in con.ModelTypes)
			{
				text = $"\t{modelType.Key}";
				if (modelType.Value is null)
					text += "(не поддерживается)";
				Console.WriteLine(text);
			}
			Console.SetCursorPosition(0, 1 + con.ChoosedModel);
			Console.Write("->");
		}

		private void Draw__EnterFilePath(bool isWrong)
		{
			if (isWrong)
				Console.WriteLine("Неверный путь!");
			Console.WriteLine("Введите путь до файла:");
			((CSVModel)con.Model!).PathCSVFile = Console.ReadLine();
		}

		private void PrepareInterface(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "UploadTable")
				return;
			PrepareInterface();
		}

		private void PrepareInterface()
		{
			UpdateLayout();
			CalcLineOffsets();
			CalcButtonOffsets();
			CheckCellBorders();
		}

		private void UpdateLayout()
		{
			var modelTable = con.GetModelTable();
			if (modelTable == null)
				return;
			var table = modelTable.Table;
			layout.Clear();
			layoutButtons.Clear();
			for (int i = 0; i < table.Count; i++)
			{
				layout.Add(new string[modelTable.Types.Count + 2]);
				layoutButtons.Add(new Action[layout.Last().Length]);
				layout[i][0] = i.ToString();
				layout[i][^1] = buttonsText[3];
				table[i].ToString()!.Split(";").CopyTo(layout[i], 1);
				for (int j = 0; j < layoutButtons[i].Length - 1; j++)
					layoutButtons[i][j] += Button__EditEntry;
				layoutButtons[i][^1] += con.RemoveEntry;
			}
			if (con.IsEditing)
			{
				layout.Add(con.ChoosedEntry.ToArray());
				layoutButtons.Add(new Action[layout.Last().Length]);
				layout.Add(new string[] { buttonsText[2], buttonsText[1] });
				layoutButtons.Add(new Action[layout.Last().Length]);
				for (int j = 0; j < layoutButtons[^2].Length; j++)
					layoutButtons[^2][j] += Button__EditCell;
				layoutButtons[^1][0] += Button__ExitEdit;
				layoutButtons[^1][1] += Button__SaveEdit;
			}
			else
			{
				layout.Add(new string[] { buttonsText[0] });
				layoutButtons.Add(new Action[layout.Last().Length]);
				layoutButtons[^1][0] += Button__AddEntry;
			}
		}

		private void CalcButtonOffsets()
		{
			offsetProgButton.Clear();
			offsetProgButton.Add(0);
			foreach (var elem in layout[^1])
				offsetProgButton.Add(offsetProgButton.Last() + elem.Length + 1);
		}

		private void CalcLineOffsets()
		{
			var modelTable = con.GetModelTable();
			if (modelTable == null)
				return;
			offset.Clear();
			offsetProg.Clear();
			offset.Add(offsetsList[typeof(int)]);
			foreach (var type in modelTable.Types)
				offset.Add(offsetsList[type]);
			offset.Add(buttonsText[3].Length);

			offsetProg.Add(0);
			foreach (var off in offset)
				offsetProg.Add(Math.Abs(offsetProg.Last()) + Math.Abs(off) + 3);
		}

		private void Draw__ShowTable()
		{
			int counter;
			string l;
			for (int i = 0; i < layout.Count - 1; i++)
			{
				counter = 0;
				Console.Write(" ");
				foreach (var val in layout[i])
				{
					l = $"{{0, {offset[counter++]}}} | ";
					Console.Write(string.Format(l, val));
				}
				Console.WriteLine("");
			}
			Draw__UnderTableButtons();
			Draw__TableSelector();
			Draw__WarningMessage();
		}

		private void Draw__UnderTableButtons()
		{
			for (int i = 0; i < layout[^1].Length; i++)
			{
				Console.SetCursorPosition(offsetProgButton[i] + 1, layout.Count - 1);
				Console.Write(layout[^1][i]);
			}
		}

		private void Draw__TableSelector(bool isSelected = false)
		{
			string l;
			if (isSelected)
				l = "{}";
			else
				l = "[]";
			if (con.CellTop == layout.Count - 1)
			{
				Console.SetCursorPosition(offsetProgButton[con.CellLeft], con.CellTop);
				Console.Write(l[0]);
				Console.SetCursorPosition(offsetProgButton[con.CellLeft + 1], con.CellTop);
				Console.Write(l[1]);
			}
			else
			{
				Console.SetCursorPosition(offsetProg[con.CellLeft], con.CellTop);
				Console.Write(l[0]);
				Console.SetCursorPosition(offsetProg[con.CellLeft + 1] - 2, con.CellTop);
				Console.Write(l[1]);
			}
		}

		private string Draw__Input()
		{
			Console.SetCursorPosition(offsetProg[con.CellLeft] + 1, layout.Count - 2);
			string l = $"{{0,{offset[con.CellLeft]}}}";
			Console.Write(string.Format(l, ""));
			Console.SetCursorPosition(offsetProg[con.CellLeft] + 1, layout.Count - 2);
			return Console.ReadLine()!;
		}

		private void Draw__WarningMessage()
		{
			if (messages.Count > 0)
			{
				Console.SetCursorPosition(0, layout.Count);
				while (messages.Count > 0)
				{
					Console.WriteLine(messages[0]);
					messages.RemoveAt(0);
				}
			}
		}
	}
}
