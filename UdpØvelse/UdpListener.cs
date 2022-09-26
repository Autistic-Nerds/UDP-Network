using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpØvelse
{
	internal class UdpListener
	{
		private bool connected;
		private UdpClient? listener;

		private IPEndPoint? groupEP;
		private Task? receiveTask;

		private Action<byte[], IPEndPoint> dataReceiveEvent = delegate { };
		private Action<string> decodedDataReceiveEvent = delegate { };

		public Action<byte[], IPEndPoint> OnDataReceived { get => dataReceiveEvent; set => dataReceiveEvent = value; }
		public Action<string> OnDecodedDataReceived { get => decodedDataReceiveEvent; set => decodedDataReceiveEvent = value; }

		public UdpListener()
		{

		}

		public UdpListener(UdpClient server, int port, Action<byte[], IPEndPoint> dataReceivedCallback, Action<string> messageReceivedCallback)
		{
			this.OnDataReceived += dataReceivedCallback;
			this.OnDecodedDataReceived += messageReceivedCallback;
			Connect(server, port);
		}

		public UdpListener(UdpClient client, IPEndPoint endPoint, Action<byte[], IPEndPoint> dataReceivedCallback, Action<string> messageReceivedCallback)
		{
			this.OnDataReceived += dataReceivedCallback;
			this.OnDecodedDataReceived += messageReceivedCallback;
			Connect(client, endPoint);
		}

		private void Connect()
		{
			connected = true;
			receiveTask = new Task(Listen);
			receiveTask.Start();
		}

		public void Connect(UdpClient client, IPEndPoint endPoint)
		{
			this.listener = client;
			this.groupEP = endPoint;
			Connect();
		}

		public void Connect(UdpClient server, int port)
		{
			listener = server;
			groupEP = new IPEndPoint(IPAddress.Any, port);
			Connect();
		}

		public void Disconnect()
		{
			connected = false;
			listener?.Close();
		}

		private void Listen()
		{
			try
			{
				while (connected)
				{
					byte[] data = listener.Receive(ref groupEP);
					dataReceiveEvent.Invoke(data, groupEP);
					if (!NetCode.IsCodeData(data))
					{
						string dataDecoded = Encoding.UTF8.GetString(data);
						decodedDataReceiveEvent.Invoke(dataDecoded);
					}
				}
			}
			catch (SocketException e)
			{
				Console.WriteLine(e);
			}
			finally
			{
				listener.Close();
			}
		}
	}
}