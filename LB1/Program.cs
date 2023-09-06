namespace AoIS_LB1
{
	public class Entry
	{
		public int Key { get; private set; }
		private object[] data;
		public object[] Data { get { return data; } }

		Entry(int rowNum, object[] data)
		{
			Key = rowNum;
			this.data = data;
		}

		void SetCell(int colNum, object cell)
		{
			data[colNum] = cell;
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