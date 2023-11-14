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
            // Создаём контекст, который будет собирать код сайта
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            var doc = await context.OpenAsync(url);
            // Парсим ссылки на странички товаров
            var aList = doc.QuerySelectorAll("div.product-preview__title a").Select(elem => doc.Origin + elem.GetAttribute("href"));
            Tv tv;
            IElement? value;
            foreach ( var a in aList )
            { // Парсим страницу товара
                Console.WriteLine(a);
                doc = await context.OpenAsync(a);
                tv = new();
                // Ищем название товара
                value = doc.QuerySelector("div.product__area-title > h1.product__title");
                tv.Name = value is not null ? ClearValue(value.TextContent) : "Not found";
                // Ицем таблицу свойств и каждого потомка-свойство преобразуем в пару ключ-значение
                foreach (var prop in doc.QuerySelectorAll("div#tab-characteristics div.property")
                    .Select(prop => new KeyValuePair<string, string>(prop.QuerySelector("div.property__name").TextContent, prop.QuerySelector("div.property__content").TextContent)))
                {
                    // Ищем нужные характеристики
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

        // Значения характеристик имеют много мусорных символов, поэтому очищаем строку от лишних пробелов и переносов
        private static string ClearValue(string value)
        {
            return value.Trim(' ', '\n');
        }
    }
}
