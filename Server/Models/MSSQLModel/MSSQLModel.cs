using Microsoft.EntityFrameworkCore;

namespace Server.Models.MSSQLModel
{
	public class MSSQLModel<TContext,TObject> : IModel<TObject> 
		where TContext : DbContext, new() 
		where TObject : class, new()
	{
		public List<TObject> GetValues()
		{
			using var context = new TContext();
			var etities = context.Set<TObject>();
			return etities.ToList();
		}

		private TObject TryCreateEntry(IEnumerable<string> entryFields)
		{
			return TryCreateEntry(entryFields, out _);
		}

#pragma warning disable CA1822 // Пометьте члены как статические
		private TObject TryCreateEntry(IEnumerable<string> entryFields, out System.Reflection.PropertyInfo[] properties)
#pragma warning restore CA1822 // Пометьте члены как статические
		{
			TObject newEntry = new();
			properties = newEntry.GetType().GetProperties().Where(p => 
			{ 
				var m = p.GetGetMethod(); 
				return m is not null && !m.IsVirtual; 
			}).ToArray();
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
				throw new FormatException($"Failed to convert field {i} to type {properties[i].PropertyType}", ex);
			}
			catch (MethodAccessException ex)
			{
				throw new MethodAccessException($"Inconsistency on the availability of property \"{properties[i].Name}\"", ex);
			}
			return newEntry;
		}

		public void AddEntry(IEnumerable<string> entryFields)
		{
			TObject entry;
			try
			{
				entry = TryCreateEntry(entryFields);
				using var context = new TContext();
				var entries = context.Set<TObject>();
				entries.Add(entry);
				context.SaveChanges();
			}
			catch { throw; }
		}

		public void EditEntry(int key, IEnumerable<string> entryFields)
		{
			TObject entry;
			try
			{
				entry = TryCreateEntry(entryFields, out var properties);
				using var context = new TContext();
				var exEntry = context.Set<TObject>().ToList().ElementAt(key);
				int i = 0;
				try
				{
					for (; i < entryFields.Count(); i++)
					{
						var value = Validator.ConvertToType(properties[i].PropertyType, entryFields.ElementAt(i));
						if (properties[i].GetValue(exEntry) != value)
							properties[i].SetValue(exEntry, value);
					}
				}
				catch (FormatException ex)
				{
					throw new FormatException($"Failed to convert field {i} to type {properties[i].PropertyType}", ex);
				}
				catch (MethodAccessException ex)
				{
					throw new MethodAccessException($"Inconsistency on the availability of property \"{properties[i].Name}\"", ex);
				}
				context.SaveChanges();
			}
			catch { throw; }
		}

		public void RemoveEntry(int key)
		{
			try
			{
				using var context = new TContext();
				var entriesSet = context.Set<TObject>();
				entriesSet.Remove(entriesSet.ToList().ElementAt(key));
				context.SaveChanges();
			}
			catch { throw; }
		}
	}
}
