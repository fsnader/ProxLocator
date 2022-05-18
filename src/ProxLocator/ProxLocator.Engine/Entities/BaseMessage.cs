using System;

namespace ProxLocator.Engine.Entities
{
    public class BaseMessage
    {
        public string CheckpointId { get; set; }
        public long TagId { get; set; }
        public double Rssi { get; set; }
        public DateTime Timestamp { get; set; }

        //public string Key => $"{CheckpointId}|{TagId}";
    }
}
