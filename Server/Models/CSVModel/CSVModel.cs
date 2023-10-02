using System.Reflection;

namespace Server.Models.CSVModel
{
    public class CSVModel<TObject> : IModel<TObject> where TObject : class, new()
    {
        private readonly string pathCSVFile;
        private bool isNeedSaveTable;
        private readonly List<TObject> table;
        public CSVModel(string pathCSVFile)
        {
            if (pathCSVFile is null || !CSVFileDriver.CheckFile(pathCSVFile))
                throw new FileNotFoundException("The path is not specified or is specified incorrectly");
            this.pathCSVFile = pathCSVFile;
            table = new();
            isNeedSaveTable = true;
        }

        public List<TObject> GetValues()
        {
            try
            {
                UploadTable();
            }
            catch { throw; }
            return table;
        }

        private void UploadTable()
        {
            table.Clear();

            var entries = CSVFileDriver.GetTableStr(pathCSVFile);
            isNeedSaveTable = false;
            foreach (var entry in entries)
            {
                AddEntry(entry);
            }
            isNeedSaveTable = true;
        }

        private void SaveTable()
        {
            List<string[]> list = new();
            PropertyInfo[] properties;
            if (table.Count > 0)
            {
                properties = table[0].GetType().GetProperties();
                foreach (var entry in table)
                {
                    list.Add(properties.Select(x => $"{x.GetValue(entry)}").ToArray());
                }
            }
            CSVFileDriver.SaveTable(pathCSVFile, list);
        }

#pragma warning disable CA1822 // Пометьте члены как статические
        private TObject TryCreateEntry(IEnumerable<string> entryFields)
#pragma warning restore CA1822 // Пометьте члены как статические
        {
            TObject newEntry = new();
            var properties = newEntry.GetType().GetProperties();
            if (entryFields.Count() != properties.Length)
                throw new InvalidArrayLengthException("The number of array elements does not match the number of object properties");
            int i = 0;
            try
            {
                for (; i < entryFields.Count(); i++)
                    properties[i].SetValue(newEntry, Validator.ConvertToType(properties[i].PropertyType, entryFields.ElementAt(i)));
            }
			catch (FormatException ex)
			{
				throw new FormatException($"Failed to convert field {i} to type {properties[i].PropertyType}\n" + ex.Message, ex);
			}
			catch (MethodAccessException ex)
			{
				throw new MethodAccessException($"Inconsistency on the availability of property \"{properties[i].Name}\"\n" + ex.Message, ex);
			}
			return newEntry;
        }

        public void AddEntry(IEnumerable<string> entryFields)
        {
            TObject entry;
            try
            {
                entry = TryCreateEntry(entryFields);
                if (isNeedSaveTable)
                    UploadTable();
            }
            catch { throw; }
            table.Add(entry);
            if (isNeedSaveTable)
                SaveTable();
        }

        public void EditEntry(int key, IEnumerable<string> entryFields)
        {
            TObject entry;
            try
            {
                entry = TryCreateEntry(entryFields);
                UploadTable();
            }
            catch { throw; }
            table[key] = entry;
            SaveTable();
        }

        public void RemoveEntry(int key)
        {
            try
            {
                UploadTable();
                table.RemoveAt(key);
            }
            catch { throw; }
            SaveTable();
        }
    }
}
