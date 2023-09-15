namespace LB1
{
	public static class Validator
	{
		public static Type[] FindTypes(string[] csvLine)
		{
			Type[] types = new Type[csvLine.Length];
			for (int i = 0; i< csvLine.Length; i++)
			{
				if (int.TryParse(csvLine[i], out _)) types[i] = typeof(int);
				else if (double.TryParse(csvLine[i], out _)) types[i] = typeof(double);
				else if (bool.TryParse(csvLine[i], out _)) types[i] = typeof(bool);
				else types[i] = typeof(string);
			}
			return types;
		}

		public static object ConvertToType(Type type, string str)
		{
			if (type == typeof(int)) return int.Parse(str);
			else if (type == typeof(double)) return double.Parse(str);
			else if (type == typeof(bool)) return bool.Parse(str);
			else return str;
		}
	}
}
