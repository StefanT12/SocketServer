using Entity;
using Entity.ContentTypes;
using Server;
using Server.Interfaces;
using SocketServer.Experiment;
using SocketServer.Experiment.Factory;
using SocketServer.Experiment.Interfaces;
using SocketServer.Experiments;
using SocketServer.NewStuff;
using SocketServer.Utility;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Test;

class Program
{
    static async Task Main(string[] args)
    {
        var cryptoData = CryptographyUtility.GenerateData("asd");

        var clientRegistry = new ClientRegistry();

        //------------------

        var serverSocket = SocketFactory.CreateSocket(IPAddress.Any, SocketFactory.SocketClientType.Tcp);

        ITcpServerSocket tcpServerSocket = new TcpServerSocket(serverSocket, clientRegistry, 100);

        tcpServerSocket = new EncryptionServerLayer(tcpServerSocket, clientRegistry);

        var serverMarshaller = new ServerMarshaller(tcpServerSocket);

        var serverHandler = new ServerHandler(serverMarshaller);

        serverHandler.AddListener<Message>(new GenericServerListener());

        serverMarshaller.Start();
        //------------------

        var clientSocket = SocketFactory.CreateSocket(IPAddress.Loopback, SocketFactory.SocketClientType.Tcp);

        clientRegistry.ClientData.TryAdd(clientSocket.LocalEndPoint.ToString(), new ClientMeta
        {
            Encryptor = cryptoData.Encryptor,
            Decryptor = cryptoData.Decryptor
        });

        ISocket socketClient = new SocketClient(clientSocket, $"{IPAddress.Loopback}:{((IPEndPoint)(serverSocket.LocalEndPoint)).Port}");

        socketClient = new EcryptedClient(socketClient, cryptoData.Encryptor, cryptoData.Decryptor);

        var marshallerClient = new MarshallerClient(socketClient);

        var dispatcher = new Dispatcher(marshallerClient);

        var genericListener1 = new GenericClientListener();

        dispatcher.AddListener<int>(genericListener1);
        dispatcher.AddListener<Message>(genericListener1);

        //------------------

        
        await marshallerClient.StartClientAsync();

        //------------------

        await marshallerClient.SendObject(new Message { Id = 12, Msg = "na pula!!!"});

        await Task.Delay(10000);

        Console.ReadKey();

        //client.Dispose();
        //server.Dispose();

        Console.WriteLine("Closed Server & Client \n Press any key to exit");
        Console.ReadKey();
    }
}