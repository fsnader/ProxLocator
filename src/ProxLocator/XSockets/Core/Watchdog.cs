using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XSockets.Core
{
    public static class Watchdog
    {
        private static string PING_MSG_STR = "X_SKT_PING";
        private static string PONG_MSG_STR = "X_SKT_PONG";

        public static byte[] PING_MSG
        {
            get
            {
                return Encoding.ASCII.GetBytes(PING_MSG_STR);
            }
        }

        public static byte[] PONG_MSG
        {
            get
            {
                return Encoding.ASCII.GetBytes(PONG_MSG_STR);
            }
        }

        public static bool IsWatchdogMessage(byte[] message)
        {
            return message.SequenceEqual(PING_MSG);
        }
    }
}
