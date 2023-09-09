using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LB1
{
	internal class Program
	{
		
		static void Main(string[] args)
		{
			View view = new(new Controler());
			view.Run();
		}

		public class View
		{
			private readonly Controler con;

			private int choosedModel = 0;
			private bool keyCheckON;

			public View(Controler controler)
			{
				this.con = controler;
				keyCheckON = true;
				con.PropertyChanged += SetScreen;
			}

			public void Run()
			{
				con.SwitchScreenTo(ViewStates.ModelSelect);
				ConsoleKey pressedKey = ConsoleKey.NoName;
				while (true)
				{
					if (keyCheckON)
					{
						pressedKey = Console.ReadKey().Key;
						if (pressedKey == ConsoleKey.Escape)
							break;
					}

					switch (con.ViewState)
					{
						case ViewStates.ModelSelect:
							if (ChooseModel(pressedKey))
								con.SwitchScreenTo(ViewStates.CSVFilePath);
							else
								con.SwitchScreenTo(ViewStates.ModelSelect);
							break;
						case ViewStates.CSVFilePath:
							break;
					}
				}
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
							Draw__EnterFilePath();
							break;
					}
				}
			}

			private bool ChooseModel(ConsoleKey key)
			{
				if(key == ConsoleKey.Enter) 
					return true;
				if(key == ConsoleKey.DownArrow)
					choosedModel++;
				else if (key == ConsoleKey.UpArrow)
					choosedModel--;

				if (choosedModel > con.ModelTypes.Length - 1) 
					choosedModel = 0;
				if (choosedModel < 0)
					choosedModel = con.ModelTypes.Length - 1;
				return false;
			}

			private void Draw__ChooseModelType()
			{
				if (choosedModel > con.ModelTypes.Length - 1) choosedModel = 0;
				Console.WriteLine("Выберите модель для работы с данными:");
				foreach (var modelType in con.ModelTypes)
				{
					Console.WriteLine($"\t{modelType}");
				}
				Console.SetCursorPosition(0, 1 + choosedModel);
				Console.Write("->");
			}

			private void Draw__EnterFilePath()
			{
				if (con.PathCSVFile == "!")
					Console.WriteLine("Неверный путь!");
				Console.WriteLine("Введите путь до файла:");
				con.PathCSVFile = Console.ReadLine();
			}
		}
	}
}