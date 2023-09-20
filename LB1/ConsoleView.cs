using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace LB1
{
	public class ConsoleView
	{
		private readonly Controler con;

		private readonly Dictionary<Type, int> offsetsList;
		private readonly List<string> buttonsText;

		public ConsoleView(Controler controler)
		{
			con = controler;
			con.UpdateTable += PrepareInterface;
			offsetsList = new()
				{
					{typeof(int), 4},
					{typeof(double), 8},
					{typeof(bool), -7},
					{typeof(string), -15},
				};
			buttonsText = new() { "Добавить", "Сохранить", "Отменить", "Удалить" };
			offsetProgButton = new();
			layoutButtons = new();
			ModelTypesSelector__Init(con.GetModelTypes());
		}

		#region ModelTypesSelector-----------------------------------------
		private string[] ModelTypes;
		[MemberNotNull(nameof(ModelTypes))]
		public void ModelTypesSelector__Init(string[] models)
		{
			if (models is null)
				throw new ArgumentNullException(nameof(models), "Initialization with null");
			ModelTypes = models;
			ModelTypesSelector__CheckBorders();
		}
		public void ModelTypesSelector__Draw()
		{
			Console.WriteLine("Выберите модель для работы с данными:");
			if (ModelTypes is null)
				return;
			foreach (var model in ModelTypes)
			{
				Console.WriteLine("    "+model);
			}
			Console.SetCursorPosition(0, 1 + con.ChoosedModel);
			Console.Write("->");
		}
		private void ModelTypesSelector__CheckBorders()
		{
			if (con.ChoosedModel > ModelTypes.Length - 1)
				con.ChoosedModel = ModelTypes.Length - 1;
			else if (con.ChoosedModel < 0)
				con.ChoosedModel = 0;
		}
		private bool ModelTypesSelector__ChooseModel(ConsoleKey key)
		{
			if (key == ConsoleKey.Enter)
			{
				return con.SetModel(con.ChoosedModel);
			}
			if (key == ConsoleKey.DownArrow)
				con.ChoosedModel++;
			else if (key == ConsoleKey.UpArrow)
				con.ChoosedModel--;
			ModelTypesSelector__CheckBorders();
			return false;
		}
		#endregion ModelTypesSelector--------------------------------------

		#region CSVFilePathEntry-------------------------------------------
		private void Draw__EnterFilePath(bool isWrong = false)
		{
			if (isWrong)
				Console.WriteLine("Неверный путь!");
			Console.WriteLine("Введите путь до файла:");
			con.SetCSVPath(Console.ReadLine());
		}
		#endregion CSVFilePathEntry----------------------------------------

		#region ShowTable--------------------------------------------------

		private readonly List<int> offset = new();
		private readonly List<int> offsetProg = new();
		private readonly List<int> offsetProgButton;
		private readonly List<Action[]> layoutButtons;

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

		private void PrepareInterface()
		{
			MakeLayout(con.GetTable());
			CalcLineOffsets(con.GetTypes());
			CalcButtonOffsets();
			CheckCellBorders();
		}

		private void MakeLayout(List<string[]> table)
		{
			con.Layout.Clear();
			layoutButtons.Clear();
			for (int i = 0; i < table.Count; i++)
			{
				con.Layout.Add(new string[table[i].Length + 2]);
				layoutButtons.Add(new Action[con.Layout.Last().Length]);
				con.Layout[i][0] = i.ToString();
				con.Layout[i][^1] = buttonsText[3];
				table[i].CopyTo(con.Layout[i], 1);
				for (int j = 0; j < layoutButtons[i].Length - 1; j++)
					layoutButtons[i][j] += con.Button__EditEntry;
				layoutButtons[i][^1] += con.Button__RemoveEntry;
			}
			if (con.IsEditing)
			{
				con.Layout.Add(con.ChoosedEntry.ToArray());
				layoutButtons.Add(new Action[con.Layout.Last().Length]);
				con.Layout.Add(new string[] { buttonsText[2], buttonsText[1] });
				layoutButtons.Add(new Action[con.Layout.Last().Length]);
				for (int j = 0; j < layoutButtons[^2].Length; j++)
					layoutButtons[^2][j] += Button__EditCell;
				layoutButtons[^1][0] += con.Button__ExitEdit;
				layoutButtons[^1][1] += con.Button__SaveEdit;
			}
			else
			{
				con.Layout.Add(new string[] { buttonsText[0] });
				layoutButtons.Add(new Action[con.Layout.Last().Length]);
				layoutButtons[^1][0] += con.Button__AddEntry;
			}
		}

		private void CalcButtonOffsets()
		{
			offsetProgButton.Clear();
			offsetProgButton.Add(0);
			foreach (var elem in con.Layout[^1])
				offsetProgButton.Add(offsetProgButton.Last() + elem.Length + 1);
		}

		private void CalcLineOffsets(List<Type> types)
		{
			offset.Clear();
			offsetProg.Clear();
			offset.Add(offsetsList[typeof(int)]);
			foreach (var type in types)
				offset.Add(offsetsList[type]);
			offset.Add(buttonsText[3].Length);

			offsetProg.Add(0);
			foreach (var off in offset)
				offsetProg.Add(Math.Abs(offsetProg.Last()) + Math.Abs(off) + 3);
		}

		private void CheckCellBorders()
		{
			if (con.IsEditing)
			{
				if (con.CellTop < con.Layout.Count - 2)
					con.CellTop = con.Layout.Count - 2;
			}
			else if (con.CellTop < 0)
				con.CellTop = 0;
			if (con.CellTop > con.Layout.Count - 1)
				con.CellTop = con.Layout.Count - 1;
			if (con.CellLeft > con.Layout[con.CellTop].Length - 1)
				con.CellLeft = con.Layout[con.CellTop].Length - 1;
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

		private void Draw__ShowTable()
		{
			int counter;
			string l;
			for (int i = 0; i < con.Layout.Count - 1; i++)
			{
				counter = 0;
				Console.Write(" ");
				foreach (var val in con.Layout[i])
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
			for (int i = 0; i < con.Layout[^1].Length; i++)
			{
				Console.SetCursorPosition(offsetProgButton[i] + 1, con.Layout.Count - 1);
				Console.Write(con.Layout[^1][i]);
			}
		}

		private void Draw__TableSelector(bool isSelected = false)
		{
			string l;
			if (isSelected)
				l = "{}";
			else
				l = "[]";
			if (con.CellTop == con.Layout.Count - 1)
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
			Console.SetCursorPosition(offsetProg[con.CellLeft] + 1, con.Layout.Count - 2);
			string l = $"{{0,{offset[con.CellLeft]}}}";
			Console.Write(string.Format(l, ""));
			Console.SetCursorPosition(offsetProg[con.CellLeft] + 1, con.Layout.Count - 2);
			return Console.ReadLine()!;
		}

		private void Draw__WarningMessage()
		{
			if (con.Messages.Count > 0)
			{
				Console.SetCursorPosition(0, con.Layout.Count);
				while (con.Messages.Count > 0)
				{
					Console.WriteLine(con.Messages[0]);
					con.Messages.RemoveAt(0);
				}
			}
		}
		#endregion ShowTable-----------------------------------------------

		public void Run()
		{
			con.SwitchScreenTo(ViewStates.ModelSelect);
			ConsoleKey pressedKey = ConsoleKey.NoName;
			while (true)
			{
				if (SetScreen())
					continue;
				pressedKey = Console.ReadKey().Key;
				if (pressedKey == ConsoleKey.Escape)
					break;

				switch (con.ViewState)
				{
					case ViewStates.ModelSelect:
						if (ModelTypesSelector__ChooseModel(pressedKey))
						{
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

		private bool SetScreen()
		{
			if (con.IsStateChanged)
			{
				con.IsStateChanged = false;
				Console.Clear();
				switch (con.ViewState)
				{
					case ViewStates.ModelSelect:
						ModelTypesSelector__Draw();
						break;
					case ViewStates.CSVFilePath:
						Draw__EnterFilePath(false);
						return true;
					case ViewStates.CSVFileWrongPath:
						Draw__EnterFilePath(true);
						return true;
					case ViewStates.ShowTable:
						Draw__ShowTable();
						break;
				}
			}
			return false;
		}

		private void Button__EditCell()
		{
			Draw__TableSelector(true);
			con.ChoosedEntry[con.CellLeft] = Draw__Input();
			MakeLayout(con.GetTable());
		}
	}
}
