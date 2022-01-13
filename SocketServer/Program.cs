using Entity;
using Entity.ContentTypes;
using SocketServer.Cryptography;
using SocketServer.Experiment;
using SocketServer.Experiment.Factory;
using SocketServer.Experiment.Interfaces;
using SocketServer.Experiments;
using SocketServer.NewStuff;
using SocketServer.Server;
using SocketServer.Server.Interfaces;
using SocketServer.Utility;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

class Program
{
    public class GenericC : IGenericListener<int>, IGenericListener<string>, IGenericListener<Message>
    {
        public void ReceiveObject(string obj)
        {
            Console.WriteLine(obj);
        }

        public void ReceiveObject(int obj)
        {
            Console.WriteLine(obj);
        }

        public void ReceiveObject(Message obj)
        {
            Console.WriteLine(obj.GetType().ToString());
            Console.WriteLine(obj.Msg);
        }
    }

   
    static async Task Main(string[] args)
    {

        var socket = SocketFactory.CreateSocket(SocketFactory.SocketClientType.Tcp);

        ISocket socketClient = new SocketClient(socket, Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork).ToString(), 0);

        var cryptoData = CryptographyUtility.GenerateData("asd");

        socketClient = new EcryptedClient(socketClient, cryptoData.Encryptor, cryptoData.Decryptor);

        var marshallerClient = new MarshallerClient(socketClient);

        var dispatcher = new Dispatcher(marshallerClient);

        var genericListener1 = new GenericC();

        dispatcher.AddListener<int>(genericListener1);
        dispatcher.AddListener<Message>(genericListener1);

        marshallerClient.ReceivedBytes(StructUtility.ObjectToBytes(new Datagram(StructUtility.ObjectToBytes(1), ContentType.Integer)));
        marshallerClient.ReceivedBytes(StructUtility.ObjectToBytes(new Datagram(StructUtility.ObjectToBytes(new Message
        {
            Id = 2,
            Msg = "msgplm"
        }), ContentType.Message)));

        {

        }
        //var msg = new Message
        //{
        //    Id = 22,
        //    Msg = "Ready Player1!"
        //};

        //var crypto = new Crypto();
        //crypto.SetCryptingData(CryptographyUtility.GenerateData("asdasd"));

        //var encrypted = crypto.Encrypt(StructUtility.StructToBytes(msg));
        //var decrypted = StructUtility.BytesToStruct<Message>(crypto.Decrypt(encrypted.ToArray()));

        //{

        //}
        //////////////


        //const string symKey = "blabla";

        //SocketClient client = new SocketClient(OnReceive, new Crypto()
        //{
        //    CryptoData = CryptographyUtility.GenerateData("blabla")
        //});

        //var clientNetworkingData = client.SetNetworkingData();

        //INetworkServer server = new ServerNetworking(OnReceiveServer);

        //var serverData = server.InitServer(100);

        //server.WhitelistClient(clientNetworkingData.TcpOpenedPort, clientNetworkingData.UdpOpenedPort, clientNetworkingData.IpAddress, symKey);

        //await client.StartClientAsync(serverData.HostName, serverData.TcpOpenedPort, serverData.UdpOpenedPort);

        //await client.SendData(new Message
        //{
        //    Id = 0,
        //    Msg = "Ready Player1!"
        //});

        Console.ReadKey();

        //client.Dispose();
        //server.Dispose();

        Console.WriteLine("Closed Server & Client \n Press any key to exit");
        Console.ReadKey();
    }

    private static void OnReceive(Datagram data)
    {
        var a = data;
    }

    private static async Task OnReceiveServer(byte[] data)
    {
        var a = StructUtility.BytesToStruct<Datagram>(data);
        await Task.Delay(1);
    }
}