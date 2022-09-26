namespace UdpØvelse
{
	internal static class NetCode
	{
		private static readonly byte[] establishConnection = new byte[] { 43 };
		private static readonly byte[] successfulConnection = new byte[] { 48 };

		private static readonly byte[] disconnectConnection = new byte[] { 73 };
		private static readonly byte[] forcedDisconnect = new byte[] { 74 };
		private static readonly byte[] successfulDisconnected = new byte[] { 75 };

		private static readonly byte[] confirmationCode = new byte[] { 54, 2, 202, 147, 104, 160, 249 };

		public static byte[] EstablishConnection => establishConnection;
		public static byte[] SuccessfulConnection => successfulConnection;
		public static byte[] Disconnect => disconnectConnection;
		public static byte[] ForcedDisconnect => forcedDisconnect;
		public static byte[] SuccessfulDisconnected => successfulDisconnected;
		
		static NetCode()
		{
			establishConnection = establishConnection.Concat(confirmationCode).ToArray();
			successfulConnection = successfulConnection.Concat(confirmationCode).ToArray();
			disconnectConnection = disconnectConnection.Concat(confirmationCode).ToArray();
			forcedDisconnect = forcedDisconnect.Concat(confirmationCode).ToArray();
			successfulDisconnected = successfulDisconnected.Concat(confirmationCode).ToArray();
		}

		public static bool Compare(byte[] data, byte[] code)
		{
			if (data.Length != code.Length)
				return false;
			for(int i = 0; i < data.Length; i++)
			{
				if (data[0] != code[0])
					return false;
			}
			return true;
		}

		public static bool IsCodeData(byte[] data)
		{
			if (Compare(data, EstablishConnection))
				return true;
			else if (Compare(data, SuccessfulConnection))
				return true;
			else if (Compare(data, Disconnect))
				return true;
			else if (Compare(data, ForcedDisconnect))
				return true;
			else if (Compare(data, SuccessfulDisconnected))
				return true;
			return false;
		}

		public static string ToString(byte[] code)
		{
			if (Compare(code, establishConnection))
				return "Establish Connection Call";
			else if (Compare(code, successfulConnection))
				return "Connection Establish Successfully";
			else
				return "default";
		}

	}
}