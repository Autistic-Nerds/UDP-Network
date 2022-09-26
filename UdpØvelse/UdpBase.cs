using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace UdpØvelse
{
	internal abstract class UdpBase
	{
		private UdpListener? listener;
		private UdpClient? udpService;

		public UdpListener? UdpListener => listener;
		public UdpClient? UdpService => udpService;

		private void Init()
		{
			AppDomain.CurrentDomain.ProcessExit += ProcessExitHandler;

		}

		protected void InitServer(int port)
		{
			udpService = new UdpClient(port);
			listener = new UdpListener(udpService, port, OnReceiveData, OnReceiveMessage);
			Init();
		}

		protected void InitClient(IPEndPoint endPoint)
		{
			udpService = new UdpClient();
			udpService.Connect(endPoint);
			listener = new UdpListener(udpService, endPoint, OnReceiveData, OnReceiveMessage);
			Init();
		}

		protected void Disconnect()
		{
			if (udpService != null)
			{
				udpService.Close();
			}
		}

		protected virtual void OnReceiveData(byte[] data, IPEndPoint endPoint)
		{
		}

		protected virtual void OnReceiveMessage(string msg)
		{
		}

		protected void Send(byte[] data)
		{
			if (UdpService == null)
			{
				Console.WriteLine($"Trying to send message, but a UDP Service has not been connected yet...");
				return;
			}

			UdpService.Send(data);
		}

		protected void Send(byte[] data, IPEndPoint groupEP)
		{
			if(UdpService == null)
			{
				Console.WriteLine($"Trying to send message, but a UDP Service has not been connected yet...");
				return;
			}

			UdpService.Send(data, groupEP);
		}

		protected virtual void ProcessExitHandler(object? sender, EventArgs e)
		{
			if (udpService != null)
				udpService.Close();
			if (listener != null)
				listener.Disconnect();
		}
	}
}