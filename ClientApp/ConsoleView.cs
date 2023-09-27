using System.Diagnostics.CodeAnalysis;

namespace TableReader.ClientApp
{
	public class ConsoleView
	{
		private readonly ClientControler con;

		private readonly List<string> buttonsText;

		public ConsoleView(ClientControler controler)
		{
			con = controler;
			con.UpdateTable += ShowTable__PrepareInterface;
			buttonsText = new() { "Добавить", "Сохранить", "Отменить", "Удалить" };
			offsetProgButton = new();
			layoutButtons = new();
			ModelTypesSelector__Init(con.GetModelTypes());
		}

		#region ModelTypesSelector-----------------------------------------

		private List<string> ModelTypes;

		[MemberNotNull(nameof(ModelTypes))]
		public void ModelTypesSelector__Init(List<string> models)
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
			Draw__WarningMessage();
			Console.SetCursorPosition(0, 1 + con.ChoosedModel);
			Console.Write("->");
		}
		private void ModelTypesSelector__CheckBorders()
		{
			if (con.ChoosedModel > ModelTypes.Count - 1)
				con.ChoosedModel = ModelTypes.Count - 1;
			else if (con.ChoosedModel < 0)
				con.ChoosedModel = 0;
		}
		private bool ModelTypesSelector__ChooseModel(ConsoleKey key)
		{
			if (key == ConsoleKey.Enter)
			{
				con.SetModel();
				return true;
			}
			if (key == ConsoleKey.DownArrow)
				con.ChoosedModel++;
			else if (key == ConsoleKey.UpArrow)
				con.ChoosedModel--;
			ModelTypesSelector__CheckBorders();
			return false;
		}
		#endregion ModelTypesSelector--------------------------------------

		#region ShowTable--------------------------------------------------

		private readonly List<int> offset = new();
		private readonly List<int> offsetProg = new();
		private readonly List<int> offsetProgButton;
		private readonly List<Action[]> layoutButtons;

		private bool ShowTable__ChooseCell(ConsoleKey key)
		{
			if (key == ConsoleKey.Enter)
			{
				return ShowTable__DoTableAction();
			}
			if (key == ConsoleKey.DownArrow)
				con.CellTop++;
			else if (key == ConsoleKey.UpArrow)
				con.CellTop--;
			else if (key == ConsoleKey.RightArrow)
				con.CellLeft++;
			else if (key == ConsoleKey.LeftArrow)
				con.CellLeft--;

			ShowTable__CheckCellBorders();
			return false;
		}
		private void ShowTable__PrepareInterface()
		{
			ShowTable__MakeLayout(con.GetTable());
			ShowTable__CalcLineOffsets(con.CountOfFields());
			ShowTable__CalcButtonOffsets();
			ShowTable__CheckCellBorders();
		}
		private void ShowTable__MakeLayout(List<string[]> table)
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
		private void ShowTable__CalcButtonOffsets()
		{
			offsetProgButton.Clear();
			offsetProgButton.Add(0);
			foreach (var elem in con.Layout[^1])
				offsetProgButton.Add(offsetProgButton.Last() + elem.Length + 1);
		}
		private void ShowTable__CalcLineOffsets(int coloumNum)
		{
			offset.Clear();
			offsetProg.Clear();
			offset.Add(4);
			for (int i = 0; i < coloumNum; i++)
				offset.Add(-12);
			offset.Add(buttonsText[3].Length);

			offsetProg.Add(0);
			foreach (var off in offset)
				offsetProg.Add(Math.Abs(offsetProg.Last()) + Math.Abs(off) + 3);
		}
		private void ShowTable__CheckCellBorders()
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
		private bool ShowTable__DoTableAction()
		{
			if (layoutButtons[con.CellTop][con.CellLeft] is null)
				return false;
			layoutButtons[con.CellTop][con.CellLeft].Invoke();
			return true;
		}
		private void ShowTable__Draw()
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
			ShowTable__DrawUnderTableButtons();
			Draw__WarningMessage();
			ShowTable__DrawTableSelector();
		}
		private void ShowTable__DrawUnderTableButtons()
		{
			for (int i = 0; i < con.Layout[^1].Length; i++)
			{
				Console.SetCursorPosition(offsetProgButton[i] + 1, con.Layout.Count - 1);
				Console.Write(con.Layout[^1][i]);
			}
		}
		private void ShowTable__DrawTableSelector(bool isSelected = false)
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
		private string ShowTable__DrawInput()
		{
			Console.SetCursorPosition(offsetProg[con.CellLeft] + 1, con.Layout.Count - 2);
			string l = $"{{0,{offset[con.CellLeft]}}}";
			Console.Write(string.Format(l, ""));
			Console.SetCursorPosition(offsetProg[con.CellLeft] + 1, con.Layout.Count - 2);
			return Console.ReadLine()!;
		}
		#endregion ShowTable-----------------------------------------------

		public void Run()
		{
			con.SwitchScreenTo(ViewStates.ModelSelect);
			ConsoleKey pressedKey;
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
						if (!ModelTypesSelector__ChooseModel(pressedKey))
							con.SwitchScreenTo(ViewStates.ModelSelect);
						break;
					case ViewStates.ShowTable:
						ShowTable__ChooseCell(pressedKey);
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
					case ViewStates.ShowTable:
						ShowTable__Draw();
						break;
				}
			}
			return false;
		}

		private void Draw__WarningMessage()
		{
			if (con.Errors.Count > 0)
			{
				Console.WriteLine("\n");
				while (con.Errors.Count > 0)
				{
					Console.WriteLine(con.Errors[0]);
					con.Errors.RemoveAt(0);
				}
			}
		}

		private void Button__EditCell()
		{
			ShowTable__DrawTableSelector(true);
			con.ChoosedEntry[con.CellLeft] = ShowTable__DrawInput();
			ShowTable__MakeLayout(con.GetTable());
		}
	}
}
