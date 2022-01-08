using SocketServer.Cryptography;
using SocketServer.Experiments;
using SocketServer.UDP;
using SocketServer.UDP.Client;
using SocketServer.UDP.Entity;
using SocketServer.UDP.Entity.ContentTypes;
using SocketServer.UDP.Interfaces;
using SocketServer.UDP.Server;
using SocketServer.Utility;
using System;
using System.Threading.Tasks;
using UDP;

class Program
{
    static async Task Main(string[] args)
    {
        //var cd = CryptographyUtility.GenerateData("blabla");

        //IProcessor serverProcessor = new Processor(new Cryptography(), new ServerDataHandler());
        //serverProcessor.SetEncryption(cd.Encryptor, cd.Decryptor);
        //Server server = new Server(serverProcessor);

        //IProcessor clientProcessor = new Processor(new Cryptography(), new EngineDataHandler());
        //clientProcessor.SetEncryption(cd.Encryptor, cd.Decryptor);
        //Client client = new Client(server.Address, server.OpenedPort, clientProcessor);

        //var content = new ObjectTransform(1, new V3(1,2,3), new Qternion(1,2,3,4));
        //client.SendAsync(content);

        //Console.ReadKey();

        //server.Dispose();

        //Console.WriteLine("Closed Server \n Press any key to exit");
        //Console.ReadKey();

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

        client.SendData(new Message
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