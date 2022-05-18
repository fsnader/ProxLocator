using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XSockets.Consumers
{
    public class ConsoleLogConsumer : ISocketConsumer
    {
        private Socket _socket { get; set; }

        public void SetSocket(Socket socket)
        {
            _socket = socket;
        }

        /// <summary>
        /// Receives the packet from the udpClient
        /// </summary>
        /// <param name="message">Byte Array of Message</param>
        /// <param name="endPoint">Endpoint</param>
        public void Notify(byte[] message, IPEndPoint endPoint)
        {
            string rawMessage = Encoding.ASCII.GetString(message);
            Console.WriteLine(Parse(endPoint) + "\t" + rawMessage);
        }

        /// <summary>
        /// Parse the IPEndPoint and returns the sender's IP
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        private string Parse(IPEndPoint endPoint)
        {
            return endPoint.Address.ToString() + ":" + endPoint.Port.ToString();
        }
    }
}
