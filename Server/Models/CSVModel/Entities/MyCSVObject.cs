namespace Server.Models.CSVModel.Entities
{
    public class MyCSVObject
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public bool IsHaveBag { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public int Number { get; set; }

        public MyCSVObject()
        {
            Name = string.Empty;
            Role = string.Empty;
            IsHaveBag = false;
            Length = 0;
            Width = 0;
            Number = 0;
        }

        public MyCSVObject(string name, string role, bool isHaveBag, double length, double width, int number)
        {
            Name = name;
            Role = role;
            IsHaveBag = isHaveBag;
            Length = length;
            Width = width;
            Number = number;
        }
    }
}
