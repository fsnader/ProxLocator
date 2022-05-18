using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NNSharp.DataTypes;
using NNSharp.IO;
using NNSharp.Models;
using ProxLocator.Engine.Entities;

namespace ProxLocator.Engine.Processor
{
    public class NeuralNetPositionProvider : IPositionProvider
    {
        //private readonly SequentialModel _distanceClassificationModel;
        //private readonly SequentialModel _shortDistanceModel;
        //private readonly SequentialModel _longDistanceModel;
        //private readonly SequentialModel _anyDistanceModel;
        private readonly NeuralNetwork _distanceClassificationModel;
        private readonly NeuralNetwork _shortDistanceModel;
        private readonly NeuralNetwork _longDistanceModel;
        private readonly NeuralNetwork _anyDistanceModel;

        private const double DistanceNorm = 27552;
        private const double RssiNorm = 3706;

        public NeuralNetPositionProvider()
        {
            _shortDistanceModel = new NeuralNetwork(@"nn_1.json");
            _anyDistanceModel = new NeuralNetwork(@"nn_2.json");
            _longDistanceModel = new NeuralNetwork(@"nn_3.json");
            _distanceClassificationModel = new NeuralNetwork(@"nn_dist.json");
        }

        public Position GetPosition(LocatorMessage[] dataRow)
        {
            var positionClassification = _distanceClassificationModel
                .Predict(dataRow.Take(2)
                    .Select(x => x.Rssi / RssiNorm)
                    .ToArray())
                .First();

            var positionInput = dataRow
                .Select(x => x.Rssi / RssiNorm)
                .ToArray();

            double[] output;

            if (positionClassification <= 0.35)
            {
                output = _shortDistanceModel
                    .Predict(positionInput);
            }
            else if (positionClassification > 0.35 && positionClassification <= 0.65)
            {
                output = _anyDistanceModel
                    .Predict(positionInput);
            }
            else
            {
                output = _longDistanceModel
                    .Predict(positionInput);
            }

            var position = new Position
            {
                X = output[0] * DistanceNorm,
                Y = output[1] * DistanceNorm,
                TimeStamp = DateTime.Now
            };

            return position;
        }
    }
}
