using System.Text.Json;

namespace TableReader.NetControler
{
	public interface IMessage
	{
		public string MessageType
		{
			get { return GetType().ToString(); }
		}
		//The data property should be only one and be called "Content"
		public string GetJson()
		{
			return JsonSerializer.Serialize(this);
		}
		public static IMessage FromJson(string json)
		{
			var jObj = JsonDocument.Parse(json);
			Type type = Type.GetType(jObj.RootElement.GetProperty(nameof(MessageType)).GetString()!)!;
			return (IMessage)Activator.CreateInstance(type, new object[] { jObj.RootElement.GetProperty("Content") })!;
		}
	}
	public abstract class TextMessage : IMessage
	{
		public string Content { get; }
		public TextMessage(string content)
		{
			Content = content;
		}
		public TextMessage(JsonElement content)
		{
			Content = content.ToString();
		}
	}
	public abstract class StringListMessage : IMessage
	{
		public List<string> Content { get; }
		public StringListMessage(List<string> content)
		{
			Content = content;
		}
		public StringListMessage(JsonElement content)
		{
			Content = content.EnumerateArray().Select(x => x.ToString()).ToList();
		}
	}

	public class ModelTypesMessage : StringListMessage
	{
		public ModelTypesMessage(List<string> content) : base(content) { }
		public ModelTypesMessage(JsonElement content) : base(content) { }
	}
	public class SetModelMessage : StringListMessage
	{
		public SetModelMessage(List<string> content) : base(content) { }
		public SetModelMessage(JsonElement content) : base(content) { }
	}
	public class TableMessage : IMessage
	{
		public string[][] Content { get; }
		public TableMessage(string[][] content)
		{
			Content = content;
		}
		public TableMessage(JsonElement content)
		{
			var eArray = content.EnumerateArray();
			Content = new string[eArray.Count()][];
			for (int i = 0; i < Content.Length; i++)
			{
				Content[i] = eArray.ElementAt(i).EnumerateArray().Select(x => x.ToString()).ToArray();
			}
		}
	}
	public class EntryMessage : StringListMessage
	{
		public EntryMessage(List<string> content) : base(content) { }
		public EntryMessage(JsonElement content) : base(content) { }
	}
	public class RemoveEntryMessage : TextMessage
	{
		public RemoveEntryMessage(string content) : base(content) { }
		public RemoveEntryMessage(JsonElement content) : base(content) { }
	}
	public class ErrorMessage : TextMessage
	{
		public ErrorMessage(string content) : base(content) { }
		public ErrorMessage(JsonElement content) : base(content) { }
	}
}