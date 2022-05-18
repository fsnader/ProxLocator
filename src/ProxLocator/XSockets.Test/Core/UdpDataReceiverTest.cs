using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Sockets;
using System.Text;
using XSockets.Consumers;
using XSockets.Core;

namespace XSockets.Test.Core
{
    [TestClass]
    public class UdpDataReceiverTest
    {
        [TestMethod]
        [Timeout(8000)]
        public void TestUDPClientResponse()
        {
            int clientPort = 11000;
            int serverPort = 12000;
            byte[] sendMessage = Encoding.UTF8.GetBytes("Testando o Socket.");
            byte[] receivedMessage;
            var hostIP = IPAddress.Parse("127.0.0.1");
            var ipEndpoint = new IPEndPoint(hostIP, 0);
            var client = new UdpClient(clientPort);
            var server = new UdpDataReceiver(serverPort, new PingConsumer());

            server.StartListening();
            client.Connect(hostIP, serverPort);
            client.Send(sendMessage, sendMessage.Length);
            receivedMessage = client.Receive(ref ipEndpoint);
            client.Close();
            server.Dispose();

            CollectionAssert.AreEqual(receivedMessage, sendMessage);
        }

        [TestMethod]
        [Timeout(8000)]
        public void TestUDPClientWatchdog()
        {
            int clientPort = 11000;
            int serverPort = 12000;
            byte[] sendMessage = Watchdog.PING_MSG;
            byte[] receivedMessage;
            var hostIP = IPAddress.Parse("127.0.0.1");
            var ipEndpoint = new IPEndPoint(hostIP, 0);
            var client = new UdpClient(clientPort);
            var server = new UdpDataReceiver(serverPort, new PingConsumer());

            server.StartListening();
            client.Connect(hostIP, serverPort);
            client.Send(sendMessage, sendMessage.Length);
            receivedMessage = client.Receive(ref ipEndpoint);
            client.Close();
            server.Dispose();

            CollectionAssert.AreEqual(receivedMessage, Watchdog.PONG_MSG);
        }
    }
}
