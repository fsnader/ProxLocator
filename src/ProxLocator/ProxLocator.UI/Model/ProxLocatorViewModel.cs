using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using ProxLocator.Engine.Consumer;
using ProxLocator.Engine.Entities;
using ProxLocator.Engine.Processor;
using ProxLocator.Engine.Utils;
using XSockets.Core;

namespace ProxLocator.UI.Model
{
    public class ProxLocatorViewModel : Observable
    {
        private ChartValues<ObservablePoint> _estimatedPosition { get; }
        private ChartValues<ObservablePoint> _realPosition { get; }

        public double LastX { get; set; }
        public double LastY { get; set; }
        public string LastTimeStamp { get; set; }

        public string LastPosition => $"({LastX} , {LastY})";

        private bool _processing { get; set; }

        private MessagesProcessor _processor { get; }

        private UdpDataReceiver _udpDataReceiver { get; set; }

        private string[][] _file { get; set; }

        public SeriesCollection SampleSeries { get; set; }


        private Visibility _startButtonVisibility;
        public Visibility StartButtonVisibility
        {
            get => _startButtonVisibility;
            set
            {
                _startButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _stopButtonVisibility;

        public Visibility StopButtonVisibility
        {
            get => _stopButtonVisibility;
            set
            {
                _stopButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        public ProxLocatorViewModel()
        {
            StartButtonVisibility = Visibility.Visible;
            StopButtonVisibility = Visibility.Collapsed;

            _estimatedPosition = new ChartValues<ObservablePoint>();
            _realPosition = new ChartValues<ObservablePoint>();

            SampleSeries = new SeriesCollection
            {
                new ScatterSeries
                {
                    Title = "Posição estimada",
                    Values = _estimatedPosition,
                    PointGeometry = DefaultGeometries.Cross,
                    LabelPoint = point => $"{point.X},{point.Y}"
                },
                new ScatterSeries
                {
                    Title = "Posição Real",
                    Values = _realPosition,
                    PointGeometry = DefaultGeometries.Circle,
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    LabelPoint = point => $"{point.X},{point.Y}"
                },
                new ScatterSeries
                {
                    Title = "Tags Fixas",
                    Values = new ChartValues<ObservablePoint>
                    {
                        new ObservablePoint(700, 0),
                        new ObservablePoint(700, 370)
                    },
                    PointGeometry = DefaultGeometries.Square,
                    LabelPoint = point => $"{point.X},{point.Y}"
                },
                new ScatterSeries
                {
                    Title = "Checkpoints",
                    Values = new ChartValues<ObservablePoint>
                    {
                        new ObservablePoint(0, 74),
                        new ObservablePoint(0, 330)
                    },
                    PointGeometry = DefaultGeometries.Square,
                    LabelPoint = point => $"{point.X},{point.Y}"
                }
            };

            _processor = new MessagesProcessor(3029387, 3026762, 3028303,
                "192.168.20.202", "192.168.20.205",
                new NeuralNetPositionProvider());

            UdpPositionCalculator();
            //FilePositionSimulator();
        }

        public void UdpPositionCalculator()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (_processing == false)
                        continue;

                    UpdateCurrentLocation();
                    Thread.Sleep(10);
                }
            });
        }

        public void FilePositionSimulator()
        {
            string path = @"data.csv";

            if (!File.Exists(path))
                return;

            _file = File
                .ReadAllLines(path)
                .Skip(1)
                .Select(x => x
                    .Split(';')
                    .Where(y => !String.IsNullOrEmpty(y))
                    .ToArray())
                .ToArray();

            Task.Run(() =>
            {
                while (true)
                {
                    if (_processing == false)
                        continue;

                    foreach (var line in _file)
                    {
                        if (_processing == false)
                            break;

                        var date = DateTime.ParseExact(line[0], "MM/dd/yyyy HH:mm:ss",
                            System.Globalization.CultureInfo.InvariantCulture);

                        var message = new LocatorMessage()
                        {
                            TagId = long.Parse(line[2]),
                            CheckpointId = line[1],
                            Rssi = Int32.Parse(line[5]),
                            Timestamp = date
                        };

                        _processor.ProcessMessage(message);
                        UpdateCurrentLocation(line);
                        Thread.Sleep(10);
                    }
                }
            });
        }

        private void UpdateCurrentLocation(string[] line = null)
        {
            if (_processor.LastPosition != null)
            {
                if (_estimatedPosition.FirstOrDefault() == null ||
                    _processor.LastPosition.X != _estimatedPosition.FirstOrDefault().X ||
                    _processor.LastPosition.Y != _estimatedPosition.FirstOrDefault().Y)
                {
                    _estimatedPosition.Clear();
                    _estimatedPosition.Add(new ObservablePoint(_processor.LastPosition.X, _processor.LastPosition.Y));

                    LastX = Math.Round(_processor.LastPosition.X, 2);
                    LastY = Math.Round(_processor.LastPosition.Y, 2);
                    LastTimeStamp = _processor.LastPosition.TimeStamp.ToString();

                    OnPropertyChanged("LastX");
                    OnPropertyChanged("LastY");
                    OnPropertyChanged("LastPosition");
                    OnPropertyChanged("LastTimeStamp");
                }

                // Shows real position if a file line[] is provided (simulation)
                if (line != null && _realPosition.FirstOrDefault() == null ||
                    double.Parse(line[3]) != _realPosition.FirstOrDefault().X ||
                    double.Parse(line[4]) != _realPosition.FirstOrDefault().Y)
                {
                    _realPosition.Clear();
                    _realPosition.Add(new ObservablePoint(double.Parse(line[3]), double.Parse(line[4])));
                }
            }
        }

        public void StartProcessing()
        {
            _processor.Clear();
            _estimatedPosition.Clear();
            _realPosition.Clear();

            _udpDataReceiver = new UdpDataReceiver(6777,
                new ProxLocatorConsumer(_processor));

            _udpDataReceiver
                .StartListening();

            _processing = true;

            StartButtonVisibility = Visibility.Collapsed;
            StopButtonVisibility = Visibility.Visible;
        }

        public void StopProcessing()
        {
            _udpDataReceiver.Dispose();

            StartButtonVisibility = Visibility.Visible;
            StopButtonVisibility = Visibility.Collapsed;
            _processing = false;
        }
    }
}
