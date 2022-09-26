using System.Net;
using System.Text;

namespace UdpØvelse
{
	internal class Server : UdpBase
	{
		private const int port = 7777;
		private List<UdpClientData> clients = new List<UdpClientData>();

		public Server()
		{
			Initialise();
		}

		private void Initialise()
		{
			Console.Clear();
			InitServer(port);
			while(true)
			{
				Update();
			}
		}

		private void Update()
		{

		}

		protected override void OnReceiveData(byte[] data, IPEndPoint endPoint)
		{
			if (NetCode.Compare(data, NetCode.EstablishConnection))
			{
				Console.WriteLine($"{NetCode.ToString(data)} - {endPoint}");
				Send(NetCode.SuccessfulConnection, endPoint);
			}
		}

		protected override void OnReceiveMessage(string msg)
		{
			Console.WriteLine(msg);
		}
	}
}