using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using HTML_Parser.Models;
using HTML_Parser.Models.Entities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Reflection;

namespace HTML_Parser
{
    public class Parser
    {
        public static void Parse(string url, bool isUseAS)
        {
            // Указываем путь до браузера (путь установки по умолчанию
            var chromeDriverService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            // Отключение вывода диагности в консоль (можно не отключать, если нужно посмотреть ошибки на странице и тд)
            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            
            var options = new ChromeOptions();
            // Отключаем отображение интерфейса браузера. Для тестирования рекомендуется оставить отображение интерфейса.
            options.AddArguments(new List<string>() { "headless" });
            // Создаём драйвер браузера.
            using IWebDriver driver = new ChromeDriver(chromeDriverService, options);
            // Установка таймаута ожидания для корректного парсинга догружаемых элементов.
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Navigate().GoToUrl(url);
            List<string> alist = driver.FindElements(By.CssSelector("div.app-catalog-1tp0ino.e1an64qs0 a")).Select(elem => elem.GetAttribute("href") + "properties/").ToList();
            List<Laptop> laptops;
            if (isUseAS)
            { // Вариант с использованием AngleSharp. Быстрее
                var parser = new HtmlParser();
                
                List<string> htmls = new();
                foreach (var a in alist.Take(10))
                { // Сохраняем код каждой необходимой страницы
                    Console.WriteLine(a);
                    driver.Navigate().GoToUrl(a);
                    // Следующая строка нужна для того чтобы сработал таймер ожидания и были догружены нужные элементы сайта
                    driver.FindElement(By.CssSelector("span.app-catalog-1eqtzki")); // Ищем первый попавшийся элемент из таблицы характеристик
                    htmls.Add(driver.PageSource);
                }
                // Парсим все созранённые страницы
                laptops = AngleSharpParse(parser, htmls);
            }
            else
            { // Вариант на чистом Selenium. Медленнее
                laptops = SeleniumParse(driver, alist);
            }
            using var dbcon = new LocalDBContext();
            dbcon.AddRange(laptops);
            dbcon.SaveChanges();
        }

        private static List<Laptop> AngleSharpParse(HtmlParser parser, List<string> htmls)
        {
            IHtmlDocument doc;
            List<Laptop> laptops = new();
            Laptop laptop;
            KeyValuePair<string, string> prop;

            foreach (var html in htmls)
            {
                doc = parser.ParseDocument(html);
                // Ищем заголовок с названием ноутбука и убираем лишнее
                laptop = new()
                {
                    Name = doc.QuerySelector("h1.e1ubbx7u0.eml1k9j0.app-catalog-tn2wxd.e1gjr6xo0").InnerHtml.Split(',')[0].Replace("Характеристики ", string.Empty)
                };
                // Находим все блоки содержащие насвание характеристик и их значения
                foreach(var element in doc.QuerySelectorAll("div.app-catalog-xc0ceg.e1ckvoeh5"))
                {
                    try
                    {
                        prop = new( // Создаём пару из названия и значения характеристики
                            string.Join(" ", element.QuerySelectorAll("span.app-catalog-1eqtzki").Children((string?)null).Select(ch => ch.InnerHtml)).ToLower(),
                            element.QuerySelector("span.app-catalog-1uhv1s4 span").InnerHtml
                        );
                    }
                    catch { continue; }
                    // Проверям характеристику и если надо обновляем свойства объекта
                    GetValue(ref laptop, prop);
                }
                laptops.Add(laptop);
            }
            return laptops;
        }

        private static List<Laptop> SeleniumParse(IWebDriver driver, List<string> alist)
        {
            List<Laptop> laptops = new();
            Laptop laptop;
            KeyValuePair<string, string> prop;

            foreach (var a in alist.Take(10))
            {
                Console.WriteLine(a);
                driver.Navigate().GoToUrl(a);
                // Следующая строка нужна для того чтобы сработал таймер ожидания и были догружены нужные элементы сайта
                driver.FindElement(By.CssSelector("span.app-catalog-1eqtzki"));
                // Ищем заголовок с названием ноутбука и убираем лишнее
                laptop = new()
                {
                    Name = driver.FindElement(By.CssSelector("h1.e1ubbx7u0.eml1k9j0.app-catalog-tn2wxd.e1gjr6xo0")).Text.Split(',')[0].Replace("Характеристики ", string.Empty)
                };
                // Находим все блоки содержащие насвание характеристик и их значения
                foreach (var webElement in driver.FindElements(By.CssSelector("div.app-catalog-xc0ceg.e1ckvoeh5")))
                {
                    try
                    {
                        prop = new( // Создаём пару из названия и значения характеристики
                            webElement.FindElement(By.CssSelector("span.app-catalog-1eqtzki")).Text.ToLower(),
                            webElement.FindElement(By.CssSelector("span.app-catalog-1uhv1s4")).Text
                        );
                    }
                    catch { continue; }
                    // Проверям характеристику и если надо обновляем свойства объекта
                    GetValue(ref laptop, prop);
                }
                laptops.Add(laptop);
            }
            return laptops;
        }

        // Массив для хранения слов по которым будут находиться нужные характеристики
        static readonly string[][] keys = new string[][]
            {
                new string[] {"операционная", "система" },
                new string[] {"диагональ", "экрана", "в", "дюймах" },
                new string[] {"процессор" },
                new string[] {"тип", "графического", "процессора" },
                new string[] {"графический", "процессор" },
            };

        private static void GetValue(ref Laptop laptop, KeyValuePair<string, string> prop)
        {
            // Проверяем наличие каждого слова в названии
            if (keys[0].All(prop.Key.Contains))
                laptop.Os = prop.Value;
            else if (keys[1].All(prop.Key.Contains))
                laptop.ScreenDiagonal = prop.Value;
            // Т.к слово одно и участвует в названиях других характеристи, используем сравнение
            else if (keys[2][0] == prop.Key)
                laptop.ProcessorModel = prop.Value;
            else if (keys[3].All(prop.Key.Contains))
                laptop.VideoCardType = prop.Value;
            else if (keys[4].All(prop.Key.Contains))
                laptop.VideoCardModel = prop.Value;
        }
    }
}
