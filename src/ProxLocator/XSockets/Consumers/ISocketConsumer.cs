using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace XSockets.Consumers
{
    public interface ISocketConsumer
    {
        /// <summary>
        /// Sets the socket to allow the consumer to send messages back to the original sender
        /// </summary>
        /// <param name="socket"></param>
        void SetSocket(Socket socket);

        /// <summary>
        /// Method called when a new packet is received from the Socket
        /// </summary>
        /// <param name="message"></param>
        /// <param name="endPoint"></param>
        void Notify(byte[] message, IPEndPoint endPoint);
    }
}
