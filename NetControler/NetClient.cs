using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace NetControler
{

	public class NetClient
	{
		private readonly TcpClient tcpClient;
		private readonly NetworkStream stream;

		public NetClient(string ip, int port)
		{
			tcpClient = new TcpClient();
			tcpClient.Connect(ip, port);
			stream = tcpClient.GetStream();
		}

		public Message SendRequest(Message message)
		{
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

		~NetClient()
		{
			stream.Dispose();
			tcpClient.Close();
		}
	}
}