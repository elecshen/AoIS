namespace LB1
{
	public class CSVTable
	{
		public List<Type> FieldTypes { get; }
		public List<object> Table { get; }
		public readonly Type EntryObjectType;

		public CSVTable(Type objectsType)
		{
			Table = new();
			EntryObjectType = objectsType;
			try
			{
				FieldTypes = ((dynamic?)Activator.CreateInstance(EntryObjectType)!).GetPropsType();
			}
			catch
			{
				throw new UnsupportedTypeException($"Type {objectsType} does not have a default constructor"); ;
			}
		}

		private bool CheckEntryType(object entry)
		{
			if (entry is null)
				return false;
			try
			{
				Convert.ChangeType(entry, EntryObjectType);
				return true;
			}
			catch 
			{ 
				return false; 
			}
		}

		public void AddEntry(object entry)
		{
			if (CheckEntryType(entry))
				Table.Add(entry);
			else
				throw new UnsupportedTypeException($"Type of the entry is not {EntryObjectType}");
		}

		public void EditEntry(int key, object entry)
		{
			if (key < 0 && key >= Table.Count)
				throw new ArgumentOutOfRangeException(nameof(key));
			if (CheckEntryType(entry))
				Table[key] = entry;
			else
				throw new UnsupportedTypeException($"Type of the entry is not {EntryObjectType}");
		}

		public void RemoveEntry(int key)
		{
			try
			{  
				Table.RemoveAt(key); 
			} 
			catch (ArgumentOutOfRangeException ex) 
			{ 
				throw new ArgumentOutOfRangeException($"Failed to delete an item with index {key}", ex);
			}
		}
	}
}
