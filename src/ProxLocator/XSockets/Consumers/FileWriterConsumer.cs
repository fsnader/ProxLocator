using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace XSockets.Consumers
{
    public class FileWriterConsumer : ISocketConsumer
    {
        private Socket _socket { get; set; }
        private string _filePath { get; set; }

        public FileWriterConsumer(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
        /// Set the socket of the notifier
        /// </summary>
        /// <param name="socket"></param>
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
            WriteToFile(Parse(endPoint) + "\t" + rawMessage);
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

        /// <summary>
        /// Append a string to a file
        /// </summary>
        /// <param name="text"></param>
        /// <param name="filePath"></param>
        private void WriteToFile(string text)
        {
            StreamWriter file = new StreamWriter(_filePath, true);
            file.WriteLine(String.Format("[ {0} ] : {1}", DateTime.Now, text));
            file.Close();
        }
    }
}
