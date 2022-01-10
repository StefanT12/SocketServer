using Entity;
using Entity.ContentTypes;
using SocketServer.Cryptography;
using SocketServer.Experiments;
using SocketServer.Server;
using SocketServer.Server.Interfaces;
using SocketServer.Utility;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        const string symKey = "blabla";

        ClientNetworking client = new ClientNetworking(OnReceive, new Cryptography()
        {
            CryptoData = CryptographyUtility.GenerateData("blabla")
        });

        var clientNetworkingData = client.SetNetworkingData();

        INetworkServer server = new ServerNetworking(OnReceiveServer);
        
        var serverData = server.InitServer(100);

        server.WhitelistClient(clientNetworkingData.TcpOpenedPort, clientNetworkingData.UdpOpenedPort, clientNetworkingData.IpAddress, symKey);

        await client.StartClientAsync(serverData.HostName, serverData.TcpOpenedPort, serverData.UdpOpenedPort);

        await client.SendData(new Message
        {
            Id = 0,
            Msg = "Ready Player1!"
        });

        Console.ReadKey();

        client.Dispose();
        server.Dispose();

        Console.WriteLine("Closed Server & Client \n Press any key to exit");
        Console.ReadKey();
    }

    private static void OnReceive(Datagram data)
    {
        var a = data;
    }

    private static async Task OnReceiveServer(byte[] data)
    {
        await Task.Delay(1);
    }
}