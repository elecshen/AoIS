namespace AoIS_LB1
{
	public class CSVEntry
	{
		public int Key { get; private set; }
		public object Data { get; private set; }

		CSVEntry(int rowNum, object data)
		{
			Key = rowNum;
			Data = data;
		}
	}

	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello, World!");
		}
	}
}