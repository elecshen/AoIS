namespace LB1
{
	public class CSVObject : ModelObject
	{
		public string Name { get; set; }
		public string Role { get; set; }
		public bool IsHaveBag { get; set; }
		public double Length { get; set; }
		public double Width { get; set; }
		public int Number { get; set; }

		public CSVObject()
		{
			Name = string.Empty;
			Role = string.Empty;
			IsHaveBag = false;
			Length = 0;
			Width = 0;
			Number = 0;
		}

		public CSVObject(string name, string role, bool isHaveBag, double length, double width, int number)
		{
			Name = name;
			Role = role;
			IsHaveBag = isHaveBag;
			Length = length;
			Width = width;
			Number = number;
		}

		public override List<object> PropsToList()
		{
			return new List<object>
			{
				Name,
				Role,
				IsHaveBag,
				Length,
				Width,
				Number
			};
		}

		public override List<Type> GetPropsType()
		{
			return new List<Type>
			{
				Name.GetType(),
				Role.GetType(),
				IsHaveBag.GetType(),
				Length.GetType(),
				Width.GetType(),
				Number.GetType()
			};
		}

		public override string ToString()
		{
			var l = new string[] {
				Name,
				Role,
				IsHaveBag.ToString(),
				Length.ToString(),
				Width.ToString(),
				Number.ToString()
			};
			return string.Join(";", l);
		}
	}
}
