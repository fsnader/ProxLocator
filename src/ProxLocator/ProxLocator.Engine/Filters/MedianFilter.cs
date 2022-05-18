using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxLocator.Engine.Entities;
using ProxLocator.Engine.Utils;

namespace ProxLocator.Engine.Filters
{
    public class MedianFilter : IFilter
    {
        private List<LocatorMessage> _medianBuffer;
        private readonly object _bufferLocker;
        private readonly int _filteringPeriod;

        public MedianFilter(int filteringPeriod)
        {
            _filteringPeriod = filteringPeriod;
            _bufferLocker = new object();
            _medianBuffer = new List<LocatorMessage>();
        }

        public LocatorMessage Filter(LocatorMessage message)
        {
            lock (_bufferLocker)
            {
                var elements = _medianBuffer
                    .TakeLast(_filteringPeriod - 1)
                    .ToList();

                elements.Add(message);

                var filteredValue = elements.Median(x => x.Rssi);

                var outputMessage = new LocatorMessage
                {
                    CheckpointId = message.CheckpointId,
                    TagId = message.TagId,
                    Timestamp = message.Timestamp,
                    Rssi = filteredValue
                };

                _medianBuffer.Add(message);

                _medianBuffer = _medianBuffer
                    .TakeLast(_filteringPeriod - 1)
                    .ToList();

                return outputMessage;
            }
        }

        public void Reset()
        {
            lock (_bufferLocker)
            {
                _medianBuffer.Clear();
            }
        }
    }
}
