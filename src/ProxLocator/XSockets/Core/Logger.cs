using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XSockets.Core
{
    public static class Logger
    {
        /// <summary>
        /// Saves the XSockets Log
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="filePath"></param>
        public static void Log(LogStatusEnum status, string message, string filePath = "XSocketsLog.txt")
        {
            StreamWriter file = new StreamWriter(filePath, true);
            file.WriteLine($"[ {DateTime.Now} ] | {status}: {message}");
            Console.WriteLine($"[ {DateTime.Now} ] | {status}: {message}");
            file.Close();
        }
    }
    /// <summary>
    /// Enum to indicate the log status
    /// </summary>
    public enum LogStatusEnum
    {
        Info,
        Warning,
        Error
    }
}
