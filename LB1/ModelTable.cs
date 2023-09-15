using System.Collections.ObjectModel;

namespace LB1
{
	public class ModelTable
	{
		private List<Type> types;
		public ReadOnlyCollection<Type> Types { get { return types.AsReadOnly(); } }
		protected List<object> table;
		public ReadOnlyCollection<object> Table { get { return table.AsReadOnly(); } }
		public readonly Type ObjectsType;

		public ModelTable(Type objectsType)
		{
			table = new();
			ObjectsType = objectsType;
			types = ((dynamic?)Activator.CreateInstance(ObjectsType)!).GetPropsType();
		}

		public bool AddEntryStr(string[] entryField)
		{
			if (entryField.Length != Types.Count) return false;
			object[] props = new object[entryField.Length];
			try
			{
				for (int i = 0; i < entryField.Length; i++)
				{
					props[i] = Validator.ConvertToType(types[i], entryField[i]);
				}
			}
			catch
			{
				return false;
			}
			table.Add(Activator.CreateInstance(ObjectsType, props)!);
			return true;
		}

		public bool AddEntryStr(string[][] entryField)
		{
			if (entryField.Length > 0)
			{
				foreach (var field in entryField)
					AddEntryStr(field);
				return true;
			}
			return false;
		}

		public bool AddEntry(object entry)
		{
			if (entry == null)
				return false;
			try
			{
				Convert.ChangeType(entry, ObjectsType);
			}
			catch { return false; }
			table.Add(entry);
			return true;
		}

		public bool AddEntry(object[] entries)
		{
			if (entries.Length > 0)
			{
				foreach (var entry in entries)
					AddEntry(entry);
				return true;
			}
			return false;
		}

		public bool EditEntry(int key, object entry)
		{
			if (key < 0 && key >= Table.Count)
				return false;
			if (entry == null)
				return false;
			try
			{
				Convert.ChangeType(entry, ObjectsType);
			}
			catch { return false; }
			table[key] = entry;
			return true;
		}

		public bool RemoveEntry(int key)
		{
			try
			{
				table.RemoveAt(key);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool RemoveEntry(object entry)
		{
			if (entry == null)
				return false;
			try
			{
				Convert.ChangeType(entry, ObjectsType);
			}
			catch { return false; }
			table.Remove(entry);
			return true;
		}

		public void ClearTable()
		{
			table.Clear();
		}
	}
}
