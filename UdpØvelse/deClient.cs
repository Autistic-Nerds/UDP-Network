using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace UdpØvelse
{
	internal class deClient
	{
		private string username;
		private bool connectionSuccessful;

		private UdpClient client;
		private Task awaitingConnectionTask;

		private IPAddress address;
		private int port = 7777;

		public deClient()
		{
			Initialise();
			AppDomain.CurrentDomain.ProcessExit += ProcessExitHandler;
		}

		private void Initialise()
		{
			Console.WriteLine($"Choose a username");

		selectUsername:
			username = Console.ReadLine();
			Main.ClearPreviousConsoleLine();
			if (string.IsNullOrWhiteSpace(username))
			{
				Console.WriteLine($"'{username}' is an invalid username, please try again.");
				goto selectUsername;
			}
			Console.Clear();

			Console.WriteLine($"Enter IP to connect to");
		selectAddress:
			string? ip = Console.ReadLine();
			Main.ClearPreviousConsoleLine();
			if (ip == "localhost" || string.IsNullOrWhiteSpace(ip))
				ip = "127.0.0.1";
			if (!IPAddress.TryParse(ip, out address))
			{
				Console.WriteLine($"'{ip}' is an invalid IP, please try again.");
				goto selectAddress;
			}
			else
			{
				AttemptConnection();
			}
		}

		private void AttemptConnection()
		{
			Console.Clear();
			Console.WriteLine($"Attempting to connect to '{address}'");

			connectionSuccessful = false;
			client = new UdpClient();
			IPEndPoint endPoint = new IPEndPoint(address, port);
			client.Connect(endPoint);

			UdpListener listener = new UdpListener();
			listener.Connect(client, endPoint);
			listener.OnDataReceived += OnReceiveData;
			listener.OnDecodedDataReceived += OnReceiveMessage;

			client.Send(NetCode.EstablishConnection);
			awaitingConnectionTask = new Task(async () =>
			{
				int attempt = 0;
				while(!connectionSuccessful)
				{
					await Task.Delay(1500);
					if (attempt > 4 || connectionSuccessful)
						break;
					Console.WriteLine($"Trying to establish connection...{++attempt}");
				}
				if(!connectionSuccessful)
				{
					Console.WriteLine($"Server could not be reached after {attempt} attempts.");
				}
			});
			awaitingConnectionTask.Start();
		}

		private void OnSuccessfulConnection()
		{
			Console.WriteLine("Connection Establish Successful");
			connectionSuccessful = true;
			byte[] nameData = Encoding.UTF8.GetBytes(username);
			client.Send(nameData);

			while (connectionSuccessful)
			{
				string? input = Console.ReadLine();
				if (!string.IsNullOrWhiteSpace(input))
				{
					byte[] data = Encoding.UTF8.GetBytes(input);
					client.Send(data);
				}
			}
		}

		private void OnReceiveData(byte[] data, IPEndPoint endPoint)
		{
			StringBuilder sb = new StringBuilder($"Data Received: ");
			for (int i = 0; i < data.Length; i++)
				sb.Append($"{data[i]} ");
			Console.WriteLine(sb.ToString());
			if(!connectionSuccessful)
			{
				if (NetCode.Compare(data, NetCode.SuccessfulConnection))
					OnSuccessfulConnection();
			}
		}

		private void OnReceiveMessage(string msg)
		{

		}

		protected virtual void ProcessExitHandler(object? sender, EventArgs e)
		{
		}
	}
}