using NetControler;
using NLog.Config;
using Server.Models;
using Server.Models.CSVModel;
using Server.Models.CSVModel.Entities;
using Server.Models.MSSQLModel;
using Server.Models.MSSQLModel.Entities;
using NLog;

namespace Server
{
    internal class Program
	{
		static async Task Main()
		{
			LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
			List<ModelConfiguration> configurations = new()
			{
				new ModelConfiguration("CSV файл", typeof(CSVModel<>)),
				new ModelConfiguration("База данных MS SQL", typeof(MSSQLModel<,>)),
			};
			configurations[0].AcceptableObjects.Add(new ModelObject("Что то", new Type[] { typeof(MyCSVObject) }, "D:\\file.csv"));
			configurations[1].AcceptableObjects.Add(new ModelObject("Преподаватели", new Type[] { typeof(DMContext), typeof(Преподаватели) }));
			configurations[1].AcceptableObjects.Add(new ModelObject("Формы Контроля", new Type[] { typeof(DMContext), typeof(ФормыКонтроля) }));
			ServerControler.GetInstance().Init(configurations);

			NetReceiver receiver = new();
			receiver.Routes.Add(
					MessageHeader.GetModelTypes,
					(message) =>
						ServerControler.GetInstance().GetModelTypes()
				);
			receiver.Routes.Add(
					MessageHeader.ModelParamsList,
					(message) =>
					{
						if (message.Content is string[] modelParams && modelParams.Length == 2)
						{
							var model = ServerControler.GetInstance().CreateModel(modelParams);
							if (model is not null)
							{
								message = ServerControler.GetInstance().GetTable(model);
								message.ModelData = modelParams;
								return message;
							}
						}
						
						return new Message(MessageHeader.Error, new string[2], typeof(string), "Failed to create a model");
					}
				);
			receiver.Routes.Add(
					MessageHeader.AddEntry,
					(message) =>
					{
						if (message.Content is string[] content)
						{
							var model = ServerControler.GetInstance().CreateModel(message.ModelData);
							if (model is not null)
							{
								message = ServerControler.GetInstance().AddEntry(content, model);
								message.ModelData = message.ModelData;
								return message;
							}
						}

						return new Message(MessageHeader.Error, new string[2], typeof(string), "Failed to create a model");
					}
				);
			receiver.Routes.Add(
					MessageHeader.EditEntry,
					(message) =>
					{
						if (message.Content is List<string> content)
						{
							var model = ServerControler.GetInstance().CreateModel(message.ModelData);
							if (model is not null)
							{
								message = ServerControler.GetInstance().EditEntry(content, model);
								message.ModelData = message.ModelData;
								return message;
							}
						}

						return new Message(MessageHeader.Error, new string[2], typeof(string), "Failed to create a model");
					}
				);
			receiver.Routes.Add(
					MessageHeader.RemoveEntry,
					(message) =>
					{
						if (message.Content is string content)
						{
							var model = ServerControler.GetInstance().CreateModel(message.ModelData);
							if (model is not null)
							{
								message = ServerControler.GetInstance().RemoveEntry(content, model);
								message.ModelData = message.ModelData;
								return message;
							}
						}

						return new Message(MessageHeader.Error, new string[2], typeof(string), "Failed to create a model");
					}
				);
			await receiver.Run();
		}
	}
}