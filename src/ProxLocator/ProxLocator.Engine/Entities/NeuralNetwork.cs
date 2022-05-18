using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NNSharp.DataTypes;
using NNSharp.IO;
using NNSharp.Models;

namespace ProxLocator.Engine.Entities
{
    public class NeuralNetwork
    {
        private readonly SequentialModel _model;

        public NeuralNetwork(string jsonPath)
        {
            var shortRangeReader = new ReaderKerasModel(jsonPath);
            _model = shortRangeReader.GetSequentialExecutor();
        }

        public double[] Predict(double[] input)
        {
            var inputLenght = input.Length;

            var inputData = new Data2D(1, 1, inputLenght, 1);

            for (var i = 0; i < inputLenght; i++)
                inputData[0, 0, i, 0] = input[i];

            var outputData = (Data2D)_model.ExecuteNetwork(inputData);

            var output = new List<double>();
            for (var i = 0; i < outputData.GetDimension().c; i++)
                output.Add(outputData[0,0,i,0]);

            return output.ToArray();
        }

    }
}
