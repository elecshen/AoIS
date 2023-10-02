﻿using System.Data;
using NetControler;
using Server.Models;
using System.Reflection;
using NLog;

namespace Server
{
	public class ServerControler
	{
		private static ServerControler? instance;
		public static ServerControler GetInstance()
		{
			instance ??= new();
			return instance;
		}

		public void Init(IEnumerable<ModelConfiguration> configurations)
		{
			ModelTypes.AddRange(configurations);
		}

		private readonly List<ModelConfiguration> ModelTypes;
		private readonly Logger Logger;
		private ServerControler()
		{
			ModelTypes = new();
			Logger = LogManager.GetCurrentClassLogger();
		}

		public Message GetModelTypes()
		{
			Dictionary<string, string[]> models = new();
			foreach (var m in ModelTypes)
			{
				models.Add(m.Name, m.AcceptableObjects.Select(o => o.Name).ToArray());
			}
			return new Message(MessageHeader.ModelTypesList, new string[2], models.GetType(), models);
		}

		public dynamic? CreateModel(string[] content)
		{
			dynamic? model = null;

			ModelConfiguration? choosedModel;
			int chosedObject;
			if ((choosedModel = ModelTypes.Find(m => m.Name == content[0])) is not null
				&& (chosedObject = choosedModel.AcceptableObjects.FindIndex(o => o.Name == content[1])) != -1)
			{
				try
				{
					Type modelType = choosedModel.ModelType.MakeGenericType(choosedModel.AcceptableObjects[chosedObject].ObjectType);
					model = (dynamic?)Activator.CreateInstance(
						modelType,
						choosedModel.AcceptableObjects[chosedObject].ConnectionParameters
						);
				}
				catch (Exception ex)
				{
					Logger.Error(ex);
				}
			}
			return model;
		}

#pragma warning disable CA1822 // Пометьте члены как статические
		public Message GetTable(dynamic model)
#pragma warning restore CA1822 // Пометьте члены как статические
		{
			List<string[]> list = new();
			var entries = model.GetValues();
			PropertyInfo[] properties;
			if (entries.Count > 0)
			{

				properties = entries[0].GetType().GetProperties();
				properties = properties.Where(p =>
				{
					var m = p.GetGetMethod();
					return m is not null && !m.IsVirtual;
				}).ToArray();
				foreach (var entry in entries)
				{
					list.Add(properties.Select(x => $"{x.GetValue(entry)}").ToArray());
				}
			}
			return new Message(MessageHeader.TableContent, new string[2], list.GetType(), list);
		}

#pragma warning disable CA1822 // Пометьте члены как статические
		private Message GetErrorMessage(string message)
#pragma warning restore CA1822 // Пометьте члены как статические
		{
			return new Message(MessageHeader.Error, new string[2], typeof(string), message);
		}

		public Message AddEntry(IEnumerable<string> content, dynamic model)
		{
			try
			{
				model.AddEntry(content);
			}
			catch (Exception ex)
			{
				if (ex is InvalidArrayLengthException || ex is FormatException)
				{
					Logger.Error(ex);
					return GetErrorMessage(ex.Message);
				}
				else
					throw;
			}
			return GetTable(model);
		}

		public Message EditEntry(List<string> content, dynamic model)
		{
			try
			{
				model.EditEntry(int.Parse(content[0]), content.GetRange(1, content.Count-1));
			}
			catch (Exception ex)
			{
				if (ex is InvalidArrayLengthException || ex is FormatException)
				{
					Logger.Error(ex);
					return GetErrorMessage(ex.Message);
				}
				else
					throw;
			}
			return GetTable(model);
		}

		public Message RemoveEntry(string content, dynamic model)
		{
			try
			{
				model.RemoveEntry(int.Parse(content));
			}
			catch (ArgumentOutOfRangeException ex)
			{
				Logger.Error(ex);
				return GetErrorMessage(ex.Message);
			}
			return GetTable(model);
		}
	}
}
