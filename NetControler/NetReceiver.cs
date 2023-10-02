using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using NLog;

namespace NetControler
{
	public class NetReceiver
	{
		private readonly TcpListener listener;
		private readonly Logger Logger;
		public readonly Dictionary<MessageHeader, Func<Message, Message>> Routes;
		public NetReceiver()
		{
			listener = new(System.Net.IPAddress.Any, 8080);
			Routes = new();
			Logger = LogManager.GetCurrentClassLogger();
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
				Logger.Error(ex);
			}
			finally
			{
				listener.Stop();
				Logger.Info("The server is stopped");
			}
		}

		private async Task ProcessClientAsync(TcpClient tcpClient)
		{
			try
			{
				var stream = tcpClient.GetStream();
				Message message;

				List<byte> data = new();
				do
				{
					data.Add((byte)stream.ReadByte());
				}
				while (stream.DataAvailable);
				string json = Encoding.Unicode.GetString(data.ToArray());
				message = JsonSerializer.Deserialize<Message>(json)!;
				var messType = message.Header;

				message = Routes[message.Header].Invoke(message);

				string log = $"Connection  to {tcpClient.Client.RemoteEndPoint}: {messType} -> {message.Header}";
				if (message.ModelData[0] is not null) 
					log += $"\nModel params: {message.ModelData[0]}<{message.ModelData[1]}>";
				if (message.Header != MessageHeader.Error)
					Logger.Info(log);
				else
					Logger.Warn(log + $"\n{(string)message.Content!}");
				

				await stream.WriteAsync(Encoding.Unicode.GetBytes(JsonSerializer.Serialize(message)));
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "Соединение разорвано: " + tcpClient.Client.RemoteEndPoint);
			}
			finally
			{
				tcpClient.Close();
			}
		}
	}
}
