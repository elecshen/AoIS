using AngleSharp;
using AngleSharp.Dom;
using HTML_Parser.Models;
using HTML_Parser.Models.Entities;

namespace HTML_Parser
{
    public class Parser
    {
        public static async Task Parse(string url)
        {
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            var doc = await context.OpenAsync(url);
            var aList = doc.QuerySelectorAll("div.product-preview__title a").Select(elem => doc.Origin + elem.GetAttribute("href"));
            Tv tv;
            IElement? value;
            foreach ( var a in aList )
            {
                Console.WriteLine(a);
                doc = await context.OpenAsync(a);
                tv = new();
                value = doc.QuerySelector("div.product__area-title > h1.product__title");
                tv.Name = value is not null ? ClearValue(value.TextContent) : "Not found";
                foreach (var prop in doc.QuerySelectorAll("div#tab-characteristics div.property")
                    .Select(prop => new KeyValuePair<string, string>(prop.QuerySelector("div.property__name").TextContent, prop.QuerySelector("div.property__content").TextContent)))
                {
                    
                    if(prop.Key == "Бренд")
                        tv.Brand = ClearValue(prop.Value);
                    else if(prop.Key.Contains("Диагональ"))
                        tv.Diagonal = ClearValue(prop.Value);
                    else if(prop.Key == "Вес")
                        tv.Weight = ClearValue(prop.Value);
                }
                using var dbcon = new LocalDBContext();
                dbcon.Add(tv);
                dbcon.SaveChanges();
            }
        }

        private static string ClearValue(string value)
        {
            return value.Trim(' ', '\n');
        }
    }
}
