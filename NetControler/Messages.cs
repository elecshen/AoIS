using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetControler
{
	public enum MessageHeader
	{
		GetModelTypes, //client
		ModelTypesList, //server
		ModelParamsList, //client
		TableContent, //server
		AddEntry, //client
		EditEntry, //client
		RemoveEntry, //client
		Error, //server
	}

	[JsonConverter(typeof(MessageConverter))]
	public class Message
	{
		public MessageHeader Header { get; }
		public string[] ModelData { get; set; }
		public Type ContentType { get; }
		public object? Content { get; }
		public Message(MessageHeader header, string[] modelData, Type contentType, object? content)
		{
			Header = header;
			ModelData = modelData;
			ContentType = contentType;
			Content = content;
		}
		public Message(MessageHeader header)
		{
			Header = header;
			ModelData = new string[2];
			ContentType = typeof(void);
		}
	}

	public class MessageConverter : JsonConverter<Message>
	{
		public override Message Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			// Read the start of the object
			reader.Read();

			var header = JsonSerializer.Deserialize<MessageHeader>(ref reader);

			reader.Read();
			if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != nameof(Message.ModelData))
			{
				throw new JsonException($"Expected \"{nameof(Message.ModelData)}\" property.");
			}
			var modelData = JsonSerializer.Deserialize<string[]>(ref reader);

			reader.Read();
			if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != nameof(Message.ContentType))
			{
				throw new JsonException($"Expected \"{nameof(Message.ContentType)}\" property.");
			}
			var contentType = Type.GetType(
				JsonSerializer.Deserialize<string>(ref reader)
				?? throw new JsonException("The \"Content Type\" property is null.")
				) ?? throw new JsonException("The \"Content Type\" property has no supported value.");

			reader.Read();
			if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != nameof(Message.Content))
			{
				throw new JsonException($"Expected \"{nameof(Message.Content)}\" property.");
			}
			object? content = null;
			if (contentType == typeof(void))
				reader.Skip();
			else
				content = JsonSerializer.Deserialize(ref reader, contentType);

			// Read the end of the object
			reader.Read();

			return new Message(header, modelData ?? new string[2], contentType, content);
		}

		public override void Write(Utf8JsonWriter writer, Message value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(Message.Header));
			JsonSerializer.Serialize(writer, value.Header, options);

			writer.WritePropertyName(nameof(Message.ModelData));
			JsonSerializer.Serialize(writer, value.ModelData, options);

			writer.WritePropertyName(nameof(Message.ContentType));
			JsonSerializer.Serialize(writer, value.ContentType.FullName, options);

			writer.WritePropertyName(nameof(Message.Content));
			JsonSerializer.Serialize(writer, value.Content, options);

			writer.WriteEndObject();
		}
	}
}