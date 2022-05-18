using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NNSharp.DataTypes;
using NNSharp.IO;
using NNSharp.Models;

namespace NeuralNetworkTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = @"neural_net.json";

            var reader = new ReaderKerasModel(filePath);
            SequentialModel model = reader.GetSequentialExecutor();

            // Then create the data to run the executer on.
            // batch: should be set in the Keras model.

            Data2D input = new Data2D(1, 1, 6, 1);

            //[-0.014571, -0.011333, -0.01646 , -0.018079, -0.019428, -0.017539]


            input[0, 0, 0, 0] = -0.014571;
            input[0, 0, 1, 0] = -0.011333;
            input[0, 0, 2, 0] = -0.01646;
            input[0, 0, 3, 0] = -0.018079;
            input[0, 0, 4, 0] = -0.019428;
            input[0, 0, 5, 0] = -0.017539;


            // Calculate the network's output.
            var output = (Data2D)model.ExecuteNetwork(input);
        }
    }
}
