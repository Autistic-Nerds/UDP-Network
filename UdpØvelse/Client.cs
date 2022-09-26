using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpØvelse
{
	internal class Client : UdpBase
	{
		private string? username;
		private bool connectionSuccessful;
		private IPAddress? ipAddress;
		private int port = 7777;
		private Task? awaitingConnectionTask;

		public Client()
		{
			Initialise();
		}

		private void Initialise()
		{
			SelectUsername();
			ConnectTo();
			while(true)
			{
				Update();
			}
		}

		private void SelectUsername()
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
		}

		private void ConnectTo()
		{
			Console.WriteLine($"Enter IP to connect to");
		selectAddress:
			string? ip = Console.ReadLine();
			Main.ClearPreviousConsoleLine();
			if (ip == "localhost" || string.IsNullOrWhiteSpace(ip))
				ip = "127.0.0.1";
			if (!IPAddress.TryParse(ip, out ipAddress))
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
			Console.WriteLine($"Attempting to connect to '{ipAddress}'");

			connectionSuccessful = false;
			IPEndPoint endPoint = new IPEndPoint(ipAddress, port);
			InitClient(endPoint);

			Send(NetCode.EstablishConnection);
			awaitingConnectionTask = new Task(async () =>
			{
				int attempt = 0;
				while (!connectionSuccessful)
				{
					await Task.Delay(1500);
					if (attempt > 4 || connectionSuccessful)
						break;
					Console.WriteLine($"Trying to establish connection...{++attempt}");
				}
				if (!connectionSuccessful)
				{
					Console.WriteLine($"Server could not be reached after {attempt} attempts.");
				}
			});
			awaitingConnectionTask.Start();
		}

		private void OnSuccessfulConnection()
		{
			Console.Clear();
			Console.WriteLine("Connection Establish Successful");
			connectionSuccessful = true;
			byte[] nameData = Encoding.UTF8.GetBytes(username);
			Send(nameData);
		}

		protected virtual void Update()
		{
			if (connectionSuccessful)
			{
				string? input = Console.ReadLine();
				if (!string.IsNullOrWhiteSpace(input))
				{
					byte[] data = Encoding.UTF8.GetBytes(input);
					Send(data);
					Main.ClearPreviousConsoleLine();
					OnReceiveMessage($"{username}: {input}");
				}
			}
		}

		protected override void OnReceiveData(byte[] data, IPEndPoint endPoint)
		{
			if(NetCode.Compare(data, NetCode.SuccessfulConnection))
			{
				OnSuccessfulConnection();
			}
		}

		protected override void OnReceiveMessage(string msg)
		{
			Main.ClearPreviousConsoleLine();
			Console.WriteLine($"{msg}\n");
		}

		protected override void ProcessExitHandler(object? sender, EventArgs e)
		{
			if (connectionSuccessful)
			{
				Send(NetCode.ForcedDisconnect);
				Disconnect();
			}
		}
	}
}