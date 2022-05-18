using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxLocator.Engine.Entities;
using ProxLocator.Engine.Filters;
using ProxLocator.Engine.Processor;

namespace ProxLocator.Tests
{
    [TestClass]
    public class BufferingTests
    {
        private MessagesProcessor _processor { get; set; }


        [TestInitialize]
        public void TestInitialization()
        {
            _processor = new MessagesProcessor(1, 2, 3,
                "127.0.0.1", "127.0.0.2",
                new RssiPositionProvider());
        }

        [TestMethod]
        public void ShouldProcessPosition()
        {
            var message1 = new LocatorMessage("0000000001000-50", "127.0.0.1");
            var message2 = new LocatorMessage("0000000001000-66", "127.0.0.2");

            Thread.Sleep(3000);

            var message3 = new LocatorMessage("0000000002000-50", "127.0.0.1");
            var message4 = new LocatorMessage("0000000002000-50", "127.0.0.2");

            var message5 = new LocatorMessage("0000000003000-50", "127.0.0.1");
            var message6 = new LocatorMessage("0000000003000-50", "127.0.0.2");

            var message7 = new LocatorMessage("0000000001000-43", "127.0.0.1");
            var message8 = new LocatorMessage("0000000001000-28", "127.0.0.2");

            _processor.ProcessMessage(message1);
            _processor.ProcessMessage(message2);
            _processor.ProcessMessage(message3);
            _processor.ProcessMessage(message4);
            _processor.ProcessMessage(message5);
            _processor.ProcessMessage(message6);
            _processor.ProcessMessage(message7);
            _processor.ProcessMessage(message8);

            Assert.IsNotNull(_processor.LastPosition);
        }

        [TestMethod]
        public void FilterSignal()
        {
            string path = @"data.csv";

            if (!File.Exists(path))
                return;

            var file = File
                .ReadAllLines(path)
                .Skip(1)
                .Select(x => x
                    .Split(';')
                    .Where(y => !String.IsNullOrEmpty(y))
                    .ToArray())
                .ToArray();

            var outputFile = new List<string>();
            outputFile.Add("raw;median;average;median_average");

            var movAverageFilter = new MovingAverageFilter(10);
            var medianFilter = new MedianFilter(3);
            var movAverageOfMedianFilter = new MovingAverageOfMedianFilter(10, 3);

            foreach (var line in file)
            {
                var date = DateTime.ParseExact(line[0], "MM/dd/yyyy HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture);

                var message = new LocatorMessage()
                {
                    TagId = long.Parse(line[2]),
                    CheckpointId = line[1],
                    Rssi = Int32.Parse(line[5]),
                    Timestamp = date
                };

                var movAveragMedian = movAverageOfMedianFilter.Filter(message);
                var movAverage = movAverageFilter.Filter(message);
                var median = medianFilter.Filter(message);

                outputFile.Add($"{message.Rssi};{median.Rssi};{movAverage.Rssi};{movAveragMedian.Rssi}");
            }

           File.WriteAllLines(@"WriteLines.csv", outputFile);

        }
    }
}
