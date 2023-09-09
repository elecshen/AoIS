namespace LB1
{
	public class CSVObject
	{
		public string Name { get; set; }
		public string Role { get; set; }
		public bool IsHaveBag { get; set; }
		public double Length { get; set; }
		public double Width { get; set; }
		public int Number { get; set; }

		public CSVObject(string name, string role, bool isHaveBag, double length, double width, int number)
		{
			Name = name;
			Role = role;
			IsHaveBag = isHaveBag;
			Length = length;
			Width = width;
			Number = number;
		}
	}

	interface IAois_model
	{
		List<object> GetTable();
		object FindEntry(int key);
		bool EditEntry(object entry);
		bool RemoveEntry(object entry);
		bool AddEntry(object entry);
	}
}
