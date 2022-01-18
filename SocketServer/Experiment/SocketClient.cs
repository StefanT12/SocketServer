using Entity;
using SocketServer.Experiment;
using SocketServer.Experiment.Factory;
using SocketServer.Experiment.Interfaces;
using SocketServer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer.Experiments
{
    public class SocketClient : ISocket, IDisposable
    {
        private IList<ISocketListener> _listeners = new List<ISocketListener>();

        private readonly IPEndPoint _serverEndpoint;

        private SemaphoreSlim _sendSemaphore = new SemaphoreSlim(1, 1);
        
        private const int BufSize = 2048;
        private readonly byte[] _buffer = new byte[BufSize];

        private readonly Socket _socket;

        public SocketClient(Socket socket, string ipPort)
        {
            _serverEndpoint = IPEndPoint.Parse(ipPort);

            _socket = socket;
        }
        
        public async Task StartAsync()
        {
            try
            {
                await _socket.ConnectAsync(_serverEndpoint);

                _socket.BeginReceive(_buffer, 0, BufSize, 0, ReceiveCallback, _buffer);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Could not connect to server...retrying in 10s, error: {e}");
                await Task.Delay(10000);
                await StartAsync();
            }
        }

        public void AddListener(ISocketListener listener)
        {
            _listeners.Add(listener);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int bytesRead = _socket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    var bytes = (byte[])ar.AsyncState;

                    foreach (var listener in _listeners)
                    {
                        listener.ReceivedBytes(bytes);
                    }
                }

                _socket.BeginReceive(_buffer, 0, BufSize, 0, ReceiveCallback, _buffer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public async Task<bool> SendBytes(ArraySegment<byte> content)
        {
            try
            {
                await _sendSemaphore.WaitAsync();
                await _socket.SendAsync(content, SocketFlags.None);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _sendSemaphore.Release();
            }
        }

        public void Dispose()
        {
            _socket.Dispose();
            _sendSemaphore.Dispose();
        }





        //private static byte[] ObjectToBytes(object obj)
        //{
        //    int size = Marshal.SizeOf(obj);
        //    byte[] strctInBytes = new byte[size];
        //    IntPtr ptr = Marshal.AllocHGlobal(size);

        //    Marshal.StructureToPtr(obj, ptr, true);
        //    Marshal.Copy(ptr, strctInBytes, 0, size);
        //    Marshal.FreeHGlobal(ptr);

        //    return strctInBytes;
        //}
    }
}
