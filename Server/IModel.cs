namespace TableReader.Server
{
	public interface IModel
	{
		public List<object> GetValues();
		public int CountOfFields();
		public object FindEntry(int key);
		public void AddEntry(string[] entryFields);
		public void EditEntry(int key, string[] entryFields);
		public void RemoveEntry(int key);
	}
}
