namespace ClientApp
{
	internal class Program
	{
		static void Main()
		{
			ConsoleView view = new(ClientControler.GetInstance());
			view.Run();
		}
	}
}