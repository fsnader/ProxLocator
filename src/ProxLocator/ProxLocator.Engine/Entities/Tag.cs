using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ProxLocator.Engine.Filters;
using ProxLocator.Engine.Utils;

namespace ProxLocator.Engine.Entities
{
    public class Tag
    {
        public long Id { get; set; }
        public ConcurrentQueue<LocatorMessage> Messages { get; set; }
        private IFilter _filter { get; set; }

        public Tag(long id)
        {
            Id = id;
            Messages = new ConcurrentQueue<LocatorMessage>();
            _filter = new MovingAverageOfMedianFilter(10,3);
        }

        public void EnqueueMessage(LocatorMessage message)
        {
            Messages.Enqueue(FilterMessage(message));
        }

        public LocatorMessage DequeueMessage()
        {
            return Messages.TryDequeue(out var message) ?
                message : null;
        }

        /// <summary>
        /// Filter the received RSSI value
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public LocatorMessage FilterMessage(LocatorMessage message)
        {
            return _filter.Filter(message);
        }

        public void Clear()
        {
            _filter.Reset();
            Messages = new ConcurrentQueue<LocatorMessage>();
        }
    }
}
