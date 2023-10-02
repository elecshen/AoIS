using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace NetControler
{

	public class NetClient
	{
		public static Message SendRequest(Message message)
		{
			using TcpClient tcpClient = new();
			tcpClient.Connect("127.0.0.1", 8080);
			var stream = tcpClient.GetStream();

			List<byte> data = new();
			string json = JsonSerializer.Serialize(message);
			stream.Write(Encoding.Unicode.GetBytes(json));
			do
			{
				data.Add((byte)stream.ReadByte());
			}
			while (stream.DataAvailable);
			return JsonSerializer.Deserialize<Message>(Encoding.Unicode.GetString(data.ToArray()))!;
		}
	}
}