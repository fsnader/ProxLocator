using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxLocator.Engine.Entities;

namespace ProxLocator.Engine.Processor
{
    public class MessagesProcessor : IProcessor
    {
        public List<CheckPoint> CheckPoints { get; set; }

        private readonly long _locationTag;
        private readonly long _referenceTag1;
        private readonly long _referenceTag2;
        private readonly string _checkPoint1;
        private readonly string _checkPoint2;
        private readonly object _lockObject = new object();

        private IPositionProvider _positionProvider { get; set; }

        private DateTime? _newestMessage { get; set; }
        private LocatorMessage[] _currentRow { get; set; }
        public ConcurrentQueue<LocatorMessage[]> DataSet { get; set; }

        public MessagesProcessor(long locationTag,
            long referenceTag1,
            long referenceTag2, 
            string checkPoint1,
            string checkPoint2,
            IPositionProvider positionProvider)
        {
            _locationTag = locationTag;
            _referenceTag1 = referenceTag1;
            _referenceTag2 = referenceTag2;
            _checkPoint1 = checkPoint1;
            _checkPoint2 = checkPoint2;
            _positionProvider = positionProvider;

            _currentRow = new LocatorMessage[6];
            DataSet = new ConcurrentQueue<LocatorMessage[]>();

            var tags = new List<long> { locationTag, referenceTag1, referenceTag2 };

            CheckPoints = new List<CheckPoint>()
            {
                new CheckPoint(checkPoint1, tags),
                new CheckPoint(checkPoint2, tags),
            };
        }

        public Position LastPosition { get; set; }

        /// <summary>
        /// Processes the received message
        /// </summary>
        /// <param name="message"></param>
        public void ProcessMessage(LocatorMessage message)
        {
            EnqueueMessageInCheckpoint(message);
            RefreshDataSet();
        }

        /// <summary>
        /// Inserts received message in the correct Checkpoint
        /// </summary>
        /// <param name="locatorMessage"></param>
        private void EnqueueMessageInCheckpoint(LocatorMessage locatorMessage)
        {
            var checkPoint = CheckPoints
                .SingleOrDefault(x => x.Id == locatorMessage.CheckpointId);

            checkPoint?.ReceiveMessage(locatorMessage);
        }

        /// <summary>
        /// Returns the oldest tag message received in checkpoint
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="checkpointId"></param>
        /// <returns></returns>
        private LocatorMessage DequeueTagMessageFromCheckpoint(long tagId, string checkpointId)
        {
            var checkPoint = CheckPoints
                .SingleOrDefault(x => x.Id == checkpointId);

            return checkPoint?.DequeueMessageFromTag(tagId);
        }

        /// <summary>
        /// Refreshes the dataset in case of new samples
        /// </summary>
        private void RefreshDataSet()
        {
            lock (_lockObject)
            {
                RefreshCurrentDataRow();
                if (_currentRow.Any(x => x == null)) return;
                EnqueueCurrentRow();
            }
        }

        /// <summary>
        /// Adds the current row in the queue
        /// </summary>
        private void EnqueueCurrentRow()
        {
            DataSet.Enqueue(new[]
            {
                _currentRow[0],
                _currentRow[1],
                _currentRow[2],
                _currentRow[3],
                _currentRow[4],
                _currentRow[5]
            });

            _currentRow = new LocatorMessage[6];
            LastPosition = CalculatePosition();
        }

        /// <summary>
        /// Refreshes the current data row with the newest available samples
        /// </summary>
        private void RefreshCurrentDataRow()
        {
            // Tag Móvel 1 - CheckPoint 1
            if (_currentRow[0] == null ||
                _currentRow[0].IsOlderThan(_newestMessage, -2))
            {
                _currentRow[0] = DequeueTagMessageFromCheckpoint
                    (_locationTag, _checkPoint1);
            }

            // Tag Móvel 1 - CheckPoint 2
            if (_currentRow[1] == null ||
                _currentRow[1].IsOlderThan(_newestMessage, -2))
            {
                _currentRow[1] = DequeueTagMessageFromCheckpoint
                    (_locationTag, _checkPoint2);
            }

            // Tag Fixo 1 - CheckPoint 1
            if (_currentRow[2] == null ||
                _currentRow[2].IsOlderThan(_newestMessage, -2))
            {
                _currentRow[2] = DequeueTagMessageFromCheckpoint
                    (_referenceTag1, _checkPoint1);
            }

            // Tag Fixo 1 - CheckPoint 2
            if (_currentRow[3] == null ||
                _currentRow[3].IsOlderThan(_newestMessage, -2))
            {
                _currentRow[3] = DequeueTagMessageFromCheckpoint
                    (_referenceTag1, _checkPoint2);
            }

            // Tag Fixo 2 - CheckPoint 1
            if (_currentRow[4] == null ||
                _currentRow[4].IsOlderThan(_newestMessage, -2))
            {
                _currentRow[4] = DequeueTagMessageFromCheckpoint
                    (_referenceTag2, _checkPoint1);
            }

            // Tag Fixo 2 - CheckPoint 2
            if (_currentRow[5] == null ||
                _currentRow[5].IsOlderThan(_newestMessage, -2))
            {
                _currentRow[5] = DequeueTagMessageFromCheckpoint
                    (_referenceTag2, _checkPoint2);
            }

            _newestMessage = _currentRow
                .Where(x => x != null)
                .Max(x => x.Timestamp);
        }

        /// <summary>
        /// Calculate the position using the PositionProvider
        /// </summary>
        /// <returns></returns>
        private Position CalculatePosition()
        {
            var dequeueSucess = DataSet.TryDequeue(out var dataInput);

            if (!dequeueSucess) return null;

            return _positionProvider
                .GetPosition(dataInput);
        }

        public void Clear()
        {
            _currentRow = new LocatorMessage[6];
            DataSet = new ConcurrentQueue<LocatorMessage[]>();

            foreach (var checkPoint in CheckPoints)
                checkPoint.Clear();
        }
    }
}
