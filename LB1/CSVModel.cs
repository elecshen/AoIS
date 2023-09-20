namespace LB1
{
	public class CSVModel : Model
	{
		private string pathCSVFile;
		public string PathCSVFile
		{
			get { return pathCSVFile; }
			set
			{
				pathCSVFile = value;
				OnPropertyChanged();
			}
		}

		public CSVModel(Type type) : base(type)
		{
			pathCSVFile = string.Empty;
		}

		public override bool UploadTable()
		{
			if (PathCSVFile != null && CSVFileDriver.CheckFile(PathCSVFile))
			{
				Table.ClearTable();
				Table.AddEntryStr(CSVFileDriver.GetTableStr(PathCSVFile));
				return true;
			}
			return false;
		}

		public override bool RefreshTable()
		{
			if(PathCSVFile == null)
				return false;
			CSVFileDriver.SaveTable(PathCSVFile, Table.Table.ToList());
			return UploadTable();
		}

		public override object FindEntry(int key)
		{
			return Table.Table[key];
		}

		public override bool AddEntry(object entry)
		{
			bool result = Table.AddEntry(entry);
			if (result)
				RefreshTable();
			return result;
		}

		public override bool EditEntry(int key, object entry)
		{
			bool result = Table.EditEntry(key, entry);
			if (result)
				RefreshTable();
			return result;
		}

		public override bool RemoveEntry(object entry)
		{
			bool result = Table.RemoveEntry(entry);
			if (result)
				RefreshTable();
			return result;
		}
	}
}
