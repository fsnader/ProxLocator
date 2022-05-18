using ProxLocator.Engine.Consumer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProxLocator.Engine.Entities;
using ProxLocator.Engine.Processor;
using XSockets.Core;

namespace ProxLocator
{
    class Program
    {
        static void Main(string[] args)
        {
            FilePositionSimulator();
        }

        public static void FilePositionSimulator()
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

            var processor = new MessagesProcessor(3029387, 3026762, 3028303,
                "192.168.20.202", "192.168.20.205",
                new NeuralNetPositionProvider());

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

                processor.ProcessMessage(message);

                if (processor.LastPosition != null)
                    Console.WriteLine($"Calculada\tX: {processor.LastPosition.X:f2}\tY: {processor.LastPosition.Y:f2}\tReal\tX: {line[3]}\tY: {line[4]}");

                Thread.Sleep(10);
            }
        }

        public static void RunUdpServer()
        {
            bool shouldStop = false;

            var udpDataReceiver = new UdpDataReceiver(6777,
                new ProxLocatorConsumer(
                    new MessagesProcessor(1, 2, 3, "127.0.0.1", "127.0.0.2",
                        new RssiPositionProvider())));

            Console.Write("Press any key to start and any key to finish");
            udpDataReceiver
                .StartListening();

            //Task.Run(() =>
            //{
            //    Thread.Sleep(2000);
            //    while (!shouldStop)
            //    {
            //        foreach (var tag in tags)
            //        {
            //            var coordenadas = tag.GetActualMessages();
            //            Console.WriteLine($"Tag: {tag.Id}");

            //            if (coordenadas == null)
            //                continue;

            //            foreach (var msg in coordenadas)
            //            {
            //                Console.WriteLine($"{msg.Value.Timestamp} | {msg.Value.CheckpointId} | {msg.Value.Rssi}");
            //            }
            //        }
            //        Thread.Sleep(1000);
            //        Console.Clear();
            //    }
            //});


            Console.ReadKey();
            shouldStop = true;
            udpDataReceiver.Dispose();

            Console.ReadKey();
        }
    }
}
