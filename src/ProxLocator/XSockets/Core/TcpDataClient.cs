using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XSockets.Consumers;
using XSockets.Utils;

namespace XSockets.Core
{
    public class TcpDataClient
    {
        private Socket _clientSocket { get; set; }
        public IPEndPoint EndPoint { get; set; }
        private ISocketConsumer _consumer { get; set; }
        private Object _stateObject { get; set; }
        private const int BUFFER_SIZE = 1024;
        private byte[] _buffer { get; set; }

        public TcpDataClient(Socket clientSocket, ISocketConsumer consumer)
        {
            _clientSocket = clientSocket;
            _consumer = consumer;
            _stateObject = new Object();
            _buffer = new byte[BUFFER_SIZE];

            EndPoint = clientSocket.RemoteEndPoint as IPEndPoint;

            Logger.Log(LogStatusEnum.Info,
                String.Format("Client {0} connected.",
                    EndPoint.Address.ToString()));
        }

        /// <summary>
        /// Start listening to new packets from the connection
        /// </summary>
        public void StartReceiving()
        {
            ReceiveNextPacket();
        }

        /// <summary>
        /// Receives the next data packet
        /// </summary>
        private void ReceiveNextPacket()
        {
            try
            {
                _clientSocket.BeginReceive(_buffer, 0, BUFFER_SIZE, 0,
                    new AsyncCallback(OnDataArrived), _stateObject);
            }
            catch (Exception e)
            {
                Logger.Log(LogStatusEnum.Error, e.Message);
                this.Dispose();
                Logger.Log(LogStatusEnum.Warning, "TCP Client disposed because of data receiving error.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void OnDataArrived(IAsyncResult result)
        {
            try
            {
                var readSize = _clientSocket.EndReceive(result);
                if (readSize > 0)
                {
                    var message = _buffer
                        .Take(readSize)
                        .ToArray();

                    if (Watchdog.IsWatchdogMessage(message))
                        _clientSocket.Send(Watchdog.PONG_MSG);
                    else
                    {
                        _consumer.SetSocket(_clientSocket);
                        _consumer.Notify(message, EndPoint);
                    }

                    Logger.Log(
                        LogStatusEnum.Info,
                        String.Format("Received data from {0}: {1}",
                            EndPoint.Address.ToString(),
                            message.ByteArrayToHexString()));

                    ReceiveNextPacket();
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogStatusEnum.Error, "Error ocurred in data receiving: " + e.Message);
                if (e.Source != "System.Net.Sockets")
                {
                    Console.WriteLine(e.Message);
                    ReceiveNextPacket();
                }
            }
        }

        /// <summary>
        /// Checks if client is connected
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            try
            {
                bool part1 = _clientSocket.Poll(1000, SelectMode.SelectRead);
                bool part2 = (_clientSocket.Available == 0);
                if (part1 && part2)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Disposes the client
        /// </summary>
        public void Dispose()
        {
            _clientSocket.Disconnect(true);
            _clientSocket.Dispose();
            Logger.Log(LogStatusEnum.Info, "TCP Client disposed.");
        }
    }
}
