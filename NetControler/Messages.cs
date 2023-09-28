using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TableReader.NetControler
{
	public enum MessageHeader
	{
		GetModelTypes,
		ModelTypesList,
		ModelParamsList,
		TableContent,
		AddEntry,
		EditEntry,
		RemoveEntry,
		Error,
	}

	[JsonConverter(typeof(MessageConverter))]
	public class Message
	{
		public MessageHeader Header { get; }
		public Type ContentType { get; }
		public object Content { get; }
		public Message(MessageHeader header, Type contentType, object content)
		{
			Header = header;
			ContentType = contentType;
			Content = content;
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
			var content = JsonSerializer.Deserialize(ref reader, contentType);

			// Read the end of the object
			reader.Read();

			return new Message(header, contentType, content!);
		}

		public override void Write(Utf8JsonWriter writer, Message value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(nameof(Message.Header));
			JsonSerializer.Serialize(writer, value.Header, options);

			writer.WritePropertyName(nameof(Message.ContentType));
			JsonSerializer.Serialize(writer, value.ContentType.FullName, options);

			writer.WritePropertyName(nameof(Message.Content));
			JsonSerializer.Serialize(writer, value.Content, options);

			writer.WriteEndObject();
		}
	}
}