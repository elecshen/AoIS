namespace Server.Models
{
    public interface IModel<TObject> where TObject : class, new()
    {
		public List<TObject> GetValues();
        public void AddEntry(IEnumerable<string> entryFields);
        public void EditEntry(int key, IEnumerable<string> entryFields);
        public void RemoveEntry(int key);
    }
}
