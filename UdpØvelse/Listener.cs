using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace UdpØvelse
{
	internal class Listener
	{
		private UdpClient listener;
		private List<UdpClientData> clientList = new List<UdpClientData>();
		public Listener()
		{
			Listen();
		}

		private void Listen()
		{
			int port = 7777;
			listener = new UdpClient(port);
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);

			try
			{
				Console.WriteLine($"Listening on port: {port}");
				while(true)
				{
					var data = listener.Receive(ref groupEP);

					if (NetCode.Compare(data, NetCode.EstablishConnection))
					{
						Console.WriteLine($"Recieved new connection call");
						AddNewClient(groupEP);
						continue;
					}

					var dataDecoded = Encoding.UTF8.GetString(data);
					Console.WriteLine(dataDecoded);
					Console.WriteLine($"recieve data from {groupEP}");

					foreach (UdpClientData client in clientList)
					{
						listener.Send(data, client.EndPoint);
					}
					listener.Send(Encoding.UTF8.GetBytes("Welcome to the internet"), groupEP);
				}
			}
			catch(SocketException e)
			{
				Console.WriteLine(e);
			}
			finally
			{
				listener.Close();
			}
		}

		private void AddNewClient(IPEndPoint groupEP)
		{
			bool recievedNewClient = true;
			foreach (UdpClientData client in clientList)
			{
				if (client.EndPoint.ToString().Equals(groupEP.ToString()))
				{ 
					recievedNewClient = false;
					break;
				}
			}

			if(recievedNewClient)
			{
				listener.Send(NetCode.SuccessfulConnection, groupEP);
				clientList.Add(new UdpClientData(groupEP));
				Console.WriteLine($"New Connection: {groupEP}");
			}
			else
			{

			}
		}
	}
}