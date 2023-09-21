namespace LB1
{
	public static class Validator
	{
		public static object ConvertToType(Type type, string str)
		{
			if (type == typeof(int))
			{
				if (int.TryParse(str, out _))
					return int.Parse(str);
				else
					throw new FormatException();
			}
			else if (type == typeof(double))
			{
				if (double.TryParse(str, out _))
					return double.Parse(str);
				else
					throw new FormatException();
			}
			else if (type == typeof(bool)) 
			{
				if (bool.TryParse(str, out _))
					return bool.Parse(str);
				else
					throw new FormatException();
			}
			else 
				return str;
		}
	}
}
