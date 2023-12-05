using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

namespace COMManager
{
    internal class Program
    {
        static void Main()
        {
            Word.Application wordApp = new();

            object file = @"D:\source\repos\AoIS\COMManager\bin\Debug\net8.0\temp.docx";
            Word.Document wDoc = wordApp.Documents.Add(ref file, false, Word.WdNewDocumentType.wdNewBlankDocument, true);
            Replace("{ЖИВОТНОЕ}", "жирафы");
            Replace("{ДЕЙСТВИЕ}", "выпивать");
            Replace("{КОЛ-ВО РАЗ}", "рассчитаны");
            Replace("{РЕЗУЛЬТАТ ДЕЙСТВИЯ}", "жирафы");

            wDoc.Bookmarks["mark"].Range.Text = "Тут была закладка";

            try
            {
                wDoc.SaveAs2(@"D:\source\repos\AoIS\COMManager\bin\Debug\net8.0\outdocx");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            wordApp.Quit(Word.WdSaveOptions.wdPromptToSaveChanges);

            void Replace(string find, string replace)
            {
                Word.Range range = wDoc.StoryRanges[Word.WdStoryType.wdMainTextStory];
                range.Find.ClearFormatting();

                range.Find.Execute(FindText: find, ReplaceWith: replace);
            }

            Excel.Application excelApp = new();
            Excel.Workbook book = excelApp.Workbooks.Add();
            Excel.Worksheet ws = book.ActiveSheet;

            for (int i = 1, num = 60; i < 10; i++, num--)
            {
                ws.Cells[1, i].Value = i;
                ws.Cells[2, i].Value = num;
            }
            Excel.Range cell = ws.Cells[3, 1];
            cell.Formula = "=SUM(A1:J1)";
            cell.FormulaHidden = false;
            cell.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

            Excel.ChartObject myChart = ((Excel.ChartObjects)ws.ChartObjects(Type.Missing)).Add(50, 100, 400, 300);
            Excel.Chart chart = myChart.Chart;
            chart.ChartType = Excel.XlChartType.xlXYScatterSmooth;
            Excel.Series series = ((Excel.SeriesCollection)chart.SeriesCollection(Type.Missing)).NewSeries();
            series.XValues = ws.Range["A1:J1"];
            chart.SetSourceData(ws.Range["A2:J1"]);
            chart.HasTitle = true;
            chart.ChartTitle.Text = "График из C#";
            chart.HasLegend = true;
            series.Name = "Title";

            try
            {
                ws.SaveAs2(@"D:\source\repos\AoIS\COMManager\bin\Debug\net8.0\outxls");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            excelApp.Quit();
        }
    }
}
