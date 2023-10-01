using System.Net.Sockets;
using System.Text.Json;
using System.Text;

namespace NetControler
{
	public class NetReceiver
	{
		private readonly TcpListener listener;
		public readonly Dictionary<MessageHeader, Func<Message, Message>> Routes;
		public NetReceiver()
		{
			listener = new(System.Net.IPAddress.Any, 8080);
			Routes = new();
		}

		public async Task Run()
		{
			try
			{
				listener.Start();

				while (true)
				{
					var tcpClient = await listener.AcceptTcpClientAsync();
					_ = Task.Run(async () => await ProcessClientAsync(tcpClient));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			finally
			{
				listener.Stop();
			}
		}

		private async Task ProcessClientAsync(TcpClient tcpClient)
		{
			try
			{
				Console.WriteLine("Новое подключение: " + tcpClient.Client.RemoteEndPoint);
				NetworkStream stream = tcpClient.GetStream();
				Message message;
				while (tcpClient.Connected)
				{
					List<byte> data = new();
					while (!stream.DataAvailable) ;
					do
					{
						data.Add((byte)stream.ReadByte());
					}
					while (stream.DataAvailable);
					string json = Encoding.Unicode.GetString(data.ToArray());
					message = JsonSerializer.Deserialize<Message>(json)!;

					message = Routes[message.Header].Invoke(message);

					await stream.WriteAsync(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(message)));
				}
			}
			catch { }
			finally
			{
				Console.WriteLine("Подключение разорвано: " + tcpClient.Client.RemoteEndPoint);
				tcpClient.Close();
			}
		}
	}
}
