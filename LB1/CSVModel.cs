namespace LB1
{
	public class CSVModel : IModel
	{
		private readonly string pathCSVFile;
		private readonly CSVTable tableHandler;
		private bool addingMode;
		public CSVModel(string pathCSVFile, Type objectType)
		{
			if (pathCSVFile is null || !CSVFileDriver.CheckFile(pathCSVFile))
				throw new FileNotFoundException("The path is not specified or is specified incorrectly");
			this.pathCSVFile = pathCSVFile;
			if (!typeof(ModelObject).IsAssignableFrom(objectType))
				throw new UnsupportedTypeException($"Type {objectType} does not inherit {typeof(ModelObject)}");
			try
			{
				tableHandler = new(objectType);

				UploadTable();
			}
			catch { throw; }
		}

		public List<object> GetValues()
		{
			return tableHandler.Table.ToList();
		}

		public int CountOfFields()
		{
			return tableHandler.FieldTypes.Count;
		}

		private void UploadTable()
		{
			var entries = CSVFileDriver.GetTableStr(pathCSVFile);

			addingMode = true;
			foreach (var entry in entries)
			{
				AddEntry(entry);
			}
			addingMode = false;
		}

		private void RefreshTable()
		{
			CSVFileDriver.SaveTable(pathCSVFile, GetValues());
			tableHandler.Table.Clear();
			UploadTable();
		}

		public object FindEntry(int key)
		{
			return tableHandler.Table[key];
		}

		private object TryCreateEntry(string[] entryFields)
		{
			if (entryFields.Length != tableHandler.FieldTypes.Count)
				throw new InvalidArrayLengthException("The number of array elements does not match the number of object properties");
			object[] props = new object[entryFields.Length];
			int i = 0;
			try
			{
				for (; i < entryFields.Length; i++)
					props[i] = Validator.ConvertToType(tableHandler.FieldTypes[i], entryFields[i]);
			}
			catch (FormatException ex)
			{
				throw new FormatException($"Failed to convert field {i} to type {tableHandler.FieldTypes[i]}", ex);
			}
			return Activator.CreateInstance(tableHandler.EntryObjectType, props)!;
		}

		public void AddEntry(string[] entryFields)
		{
			object entry;
			try
			{
				entry = TryCreateEntry(entryFields);
				tableHandler.AddEntry(entry);
			}
			catch { throw; }
			if (!addingMode)
				RefreshTable();
		}

		public void EditEntry(int key, string[] entryFields)
		{
			object entry;
			try
			{
				entry = TryCreateEntry(entryFields);
				tableHandler.EditEntry(key, entry);
			}
			catch { throw; }

			RefreshTable();
		}

		public void RemoveEntry(int key)
		{
			try
			{
				tableHandler.RemoveEntry(key);
			}
			catch { throw; }

			RefreshTable();
		}
	}
}
