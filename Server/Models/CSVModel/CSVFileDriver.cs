using Server.IOFile;

namespace Server.Models.CSVModel
{
	public static class CSVFileDriver
	{
		public static bool CheckFile(string path)
		{
			return File.Exists(path);
		}

		public static string[][] GetTableStr(string path, string sep = ";")
		{
			var strLines = FileReader.ReadLines(path);
			if (strLines.Count == 0) return Array.Empty<string[]>();
			string[][] tableStr = new string[strLines.Count][];
			for (int i = 0; i < strLines.Count; i++)
			{
				tableStr[i] = strLines[i].Split(sep);
			}
			return tableStr;
		}

		public static bool SaveTable(string path, List<string[]> table)
		{
			FileWriter.SaveFile(path, table.Select(x => string.Join(";", x)).ToList());
			return false;
		}
	}
}
