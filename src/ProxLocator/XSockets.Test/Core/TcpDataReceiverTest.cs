using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Sockets;
using System.Text;
using XSockets.Consumers;
using XSockets.Core;

namespace XSockets.Test.Core
{
    [TestClass]
    public class TcpDataReceiverTest
    {
        [TestMethod]
        [Timeout(8000)]
        public void TestTCPClientResponse()
        {
            int clientPort = 15000;
            int serverPort = 15001;
            byte[] sendMessage = Encoding.UTF8.GetBytes("Testando o Socket.");
            byte[] receivedMessage = new byte[sendMessage.Length];
            string hostIP = "127.0.0.1";
            var hostIPAddress = IPAddress.Parse(hostIP);
            var ipEndpoint = new IPEndPoint(hostIPAddress, clientPort);
            var client = new TcpClient(ipEndpoint);
            var server = new TcpDataReceiver(hostIP, serverPort, new PingConsumer());
            NetworkStream stream;

            server.StartListening();
            client.Connect(hostIPAddress, serverPort);
            stream = client.GetStream();
            stream.Write(sendMessage, 0, sendMessage.Length);
            stream.Read(receivedMessage, 0, receivedMessage.Length);
            client.Close();

            CollectionAssert.AreEqual(receivedMessage, sendMessage);
        }

        [TestMethod]
        [Timeout(8000)]
        public void TestTCPClientWatchdog()
        {
            int clientPort = 15000;
            int serverPort = 15001;
            byte[] sendMessage = Watchdog.PING_MSG;
            byte[] receivedMessage = new byte[sendMessage.Length];
            string hostIP = "127.0.0.1";
            var hostIPAddress = IPAddress.Parse(hostIP);
            var ipEndpoint = new IPEndPoint(hostIPAddress, clientPort);
            var client = new TcpClient(ipEndpoint);
            var server = new TcpDataReceiver(hostIP, serverPort, new PingConsumer());
            NetworkStream stream;

            server.StartListening();
            client.Connect(hostIPAddress, serverPort);
            stream = client.GetStream();
            stream.Write(sendMessage, 0, sendMessage.Length);
            stream.Read(receivedMessage, 0, receivedMessage.Length);
            client.Close();

            CollectionAssert.AreEqual(receivedMessage, Watchdog.PONG_MSG);
        }
    }
}
