using ProxLocator.Engine.Utils;
using System;

namespace ProxLocator.Engine.Entities
{
    public class LocatorMessage : BaseMessage
    {
        public LocatorMessage() { }

        public LocatorMessage(string rawMessage, string ip)
        {
            rawMessage.ThrowIfNull();
            ip.ThrowIfNull();

            CheckpointId = ip;
            TagId = Convert.ToInt64(rawMessage.Substring(0, 10));
            //Rssi = Convert.ToInt32(rawMessage.Substring(10, 6));
            Rssi = Convert.ToDouble(rawMessage.Substring(13, 3));
            Timestamp = DateTime.Now;
        }

        public bool IsOlderThan(DateTime? date, int secondsOffset = 0)
        {
            return date.HasValue &&
                   Timestamp < date.Value.AddSeconds(secondsOffset);
        }
    }
}
