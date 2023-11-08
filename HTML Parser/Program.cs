using HTML_Parser.Models;

namespace HTML_Parser
{
    internal class Program
    {
        static async Task Main()
        {
            await Parser.Parse(@"https://2droida.ru/collection/televizory-xiaomi");
            using var dbcon = new LocalDBContext();
            foreach(var t in dbcon.Tvs)
                Console.WriteLine("{0}\n\t{1}\n\t{2}\n\t{3}", t.Name, t.Brand, t.Diagonal, t.Weight);
        }
    }
}