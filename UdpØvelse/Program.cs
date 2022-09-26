using UdpØvelse;

Console.WriteLine("Do you wish to be a 'Client' or 'Server'?");

invalidInput:

string? input = Console.ReadLine();

switch(input.ToLower())
{
	case "client":
		_ = new Client();
		break;
	case "server":
		_ = new Server();
		break;
	default:
		Console.WriteLine("Invalid input... Please try again.");
		goto invalidInput;
}

Console.ReadKey();