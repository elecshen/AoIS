namespace Server.IOFile
{
    public static class FileReader
    {
        public static List<string> ReadLines(string path)
        {
            using StreamReader sr = new(path, System.Text.Encoding.Default);
            List<string> lines = new();
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                lines.Add(line);
            }
            return lines;
        }
    }
}
