using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ProxLocator.Engine.Entities;
using ProxLocator.Engine.Utils;
using XSockets.Consumers;
using ProxLocator.Engine.Processor;

namespace ProxLocator.Engine.Consumer
{
    public class ProxLocatorConsumer : ISocketConsumer
    {
        private Socket _socket;
        private IProcessor _processor;

        public ProxLocatorConsumer(IProcessor processor)
        {
            _processor = processor;
        }

        public void SetSocket(Socket socket)
        {
            _socket = socket;
        }

        public void Notify(byte[] message, IPEndPoint endPoint)
        {
            string rawMessage = Encoding
                .ASCII.GetString(message);

            var locatorMessage = new LocatorMessage(rawMessage,
                endPoint.ToEndpointString());

            _processor.ProcessMessage(locatorMessage);
        }
    }
}
