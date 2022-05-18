using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XSockets.Consumers
{
    public class PingConsumer : ISocketConsumer
    {
        private Socket _socket { get; set; }

        public void SetSocket(Socket socket)
        {
            _socket = socket;
        }

        /// <summary>
        /// Answer the message with itself.
        /// </summary>
        /// <param name="message">Byte Array of Message</param>
        /// <param name="endPoint">Endpoint</param>
        public void Notify(byte[] message, System.Net.IPEndPoint endPoint)
        {
            _socket.SendTo(message, endPoint);
        }
    }
}
