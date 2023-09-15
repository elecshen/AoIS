namespace LB1
{
	public static class CSVFileDriver
	{
		public static bool CheckFile(string path)
		{
			return File.Exists(path);
		}

		public static string[][] GetTableStr(string path, string sep=";")
		{
			var strLines = CSVFileReader.ReadLines(path);
			if(strLines.Count == 0) return Array.Empty<string[]>();
			string[][] tableStr = new string[strLines.Count][];
			for (int i = 0; i < strLines.Count; i++)
			{
				tableStr[i] = strLines[i].Split(sep);
			}
			return tableStr;
		}

		public static bool SaveTable(string path, List<object> table)
		{
			CSVFileWriter.SaveFile(path, table.Select(x => x.ToString()).ToList()!);
			return false;
		}
	}
}
