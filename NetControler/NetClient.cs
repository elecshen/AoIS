using System.Net.Sockets;
using System.Text;

namespace TableReader.NetControler
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

		public IMessage SendRequest(IMessage message)
		{
			List<byte> data = new();
			stream.Write(Encoding.UTF8.GetBytes(message.GetJson()));
			do
			{
				data.Add((byte)stream.ReadByte());
			}
			while (stream.DataAvailable);
			string json = Encoding.UTF8.GetString(data.ToArray());
			return IMessage.FromJson(json);
		}

		~NetClient()
		{
			stream.Dispose();
			tcpClient.Close();
		}
	}
}