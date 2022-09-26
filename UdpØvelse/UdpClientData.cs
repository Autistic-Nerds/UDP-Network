using System.Net;

namespace UdpØvelse
{
	internal class UdpClientData
	{
		private string name;
		private IPEndPoint endPoint;
		private float timeSinceLastUpdate;
		private bool initialConnectionCall;

		public string Name => name;
		public IPEndPoint EndPoint => endPoint;
		public bool InitialConnectionCall => initialConnectionCall;
		public float TimeSinceLastUpdate { get => timeSinceLastUpdate; set => timeSinceLastUpdate = value; }

		public UdpClientData(IPEndPoint endPoint)
		{
			name = "";
			this.endPoint = endPoint;
			initialConnectionCall = true;
		}

		public void PopulateClientData(string name)
		{
			initialConnectionCall = false;
			this.name = name;
		}
	}
}