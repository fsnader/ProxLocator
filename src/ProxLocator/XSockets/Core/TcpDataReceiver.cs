using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using XSockets.Consumers;

namespace XSockets.Core
{
    public class TcpDataReceiver : DataReceiver
    {
        private TcpListener _tcpListener { get; set; }
        private ISocketConsumer _consumer { get; set; }
        private List<TcpDataClient> _tcpClientsList { get; set; }

        public TcpDataReceiver(string ipAddress, int port, ISocketConsumer consumer)
        {
            var serverAddress = IPAddress.Parse(ipAddress);
            _tcpListener = new TcpListener(serverAddress, port);

            _tcpClientsList = new List<TcpDataClient>();
            _consumer = consumer;
            _tcpListener.Start();
            Logger.Log(LogStatusEnum.Info, "TCP Listener created in port " + port);
        }

        /// <summary>
        /// Start listening assyncronously to new clients and removes disconnected clients
        /// </summary>
        public void StartListening()
        {
            try
            {
                _tcpListener.BeginAcceptSocket(
                    new AsyncCallback(AcceptedSocketCallback), _tcpListener
                );
            }
            catch(Exception e)
            {
                Logger.Log(LogStatusEnum.Error, e.Message);
            }

        }

        /// <summary>
        /// Callback that handler socket TCP connections.
        /// </summary>
        /// <param name="result"></param>
        public void AcceptedSocketCallback(IAsyncResult result)
        {
            TcpListener listener = (TcpListener)result.AsyncState;
            Socket clientSocket = listener.EndAcceptSocket(result);

            var tcpClient = new TcpDataClient(
                clientSocket,
                _consumer);

            tcpClient.StartReceiving();

            _tcpClientsList
                .Add(tcpClient);

            _tcpClientsList
                .RemoveAll(x =>
                    !x.IsConnected());

            Logger.Log(LogStatusEnum.Info,
                String.Format("Total TCP clients connected: {0}",
                _tcpClientsList.Count));

            StartListening();
        }

        ///// <summary>
        ///// Disposes the Tcp Server and all clients
        ///// </summary>
        public void Dispose()
        {
            foreach (var client in _tcpClientsList)
                client.Dispose();

            _tcpClientsList.Clear();

            Logger.Log(LogStatusEnum.Info, "Trying to dispose TCP Socket.");

            _tcpListener.Server
                .Disconnect(true);

            _tcpListener.Stop();

            Logger.Log(LogStatusEnum.Info, "TCP Socket disposed.");
        }
    }
}
