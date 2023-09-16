namespace LB1
{
	internal class Program
	{
		static void Main()
		{
			ConsoleView view = new(Controler.GetInstance());
			view.Run();
		}
	}
}