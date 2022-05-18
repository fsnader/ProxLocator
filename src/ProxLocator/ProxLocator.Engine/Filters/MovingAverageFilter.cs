using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxLocator.Engine.Entities;
using ProxLocator.Engine.Utils;

namespace ProxLocator.Engine.Filters
{
    public class MovingAverageFilter : IFilter
    {
        private List<LocatorMessage> _movAverageBuffer;
        private readonly object _bufferLocker;
        private readonly int _filteringPeriod;

        public MovingAverageFilter(int filteringPeriod)
        {
            _filteringPeriod = filteringPeriod;
            _bufferLocker = new object();
            _movAverageBuffer = new List<LocatorMessage>();
        }

        public LocatorMessage Filter(LocatorMessage message)
        {
            lock (_bufferLocker)
            {
                var elements = _movAverageBuffer
                    .TakeLast(_filteringPeriod - 1)
                    .ToList();

                elements.Add(message);

                var filteredValue = elements.Average(x => x.Rssi);

                var outputMessage = new LocatorMessage
                {
                    CheckpointId = message.CheckpointId,
                    TagId = message.TagId,
                    Timestamp = message.Timestamp,
                    Rssi = filteredValue
                };


                _movAverageBuffer.Add(outputMessage);

                _movAverageBuffer = _movAverageBuffer
                    .TakeLast(_filteringPeriod - 1)
                    .ToList();

                return outputMessage;
            }
        }

        public void Reset()
        {
            lock (_bufferLocker)
            {
                _movAverageBuffer.Clear();
            }
        }
    }
}
