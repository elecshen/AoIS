using HTML_Parser.Models;

namespace HTML_Parser
{
    internal class Program
    {
        static void Main()
        {
            // true - запускает парсинг через AngleSharp, false - будет использовать только средства Selenium
            Parser.Parse(@"https://www.citilink.ru/catalog/noutbuki/", true);
            using var dbcon = new LocalDBContext();
            foreach(var t in dbcon.Laptops)
                Console.WriteLine("{0}\n\t{1}\n\t{2}\n\t{3}\n\t{4}\n\t{5}", t.Name, t.Os, t.ScreenDiagonal, t.ProcessorModel, t.VideoCardType, t.VideoCardModel);
        }
    }
}