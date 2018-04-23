using System;
using System.Collections.Generic;
using System.Drawing;
using FANNCSharp;
using FANNCSharp.Double;

namespace GKCaptcha
{
    // examples here
    // http://mac-blog.org.ua/c-fann-example-raspoznavanie-s-ispolzovaniem-neyronnoy-seti/


    class NeuralNetworkTrainer
    {
        public NeuralNet Network;
        public TrainingData Data;
        public int InputsCount;
        public int OutputsCount = 9;

        public NeuralNetworkTrainer(Image[] images, int[] answers)
        {
            InputsCount = images[0].Width*images[0].Height;

            var inputs = new List<double[]>();
            var outputs = new List<double[]>();

            for (var k = 0; k < images.Length; k++)
            {
                inputs.Add(GetRawData(images[k]));
                var res = new double[OutputsCount];
                for (var i = 0; i < res.Length; i++)
                    res[i] = answers[k] == i + 1 ? 1 : 0;
                outputs.Add(res);
            }

            Data = new TrainingData();
            Data.SetTrainData(inputs.ToArray(), outputs.ToArray());

            //3-43%
            //2 - 42%
            //1 - 45%
            //0.5 - 41%

            var layers = new List<uint>();
            layers.Add((uint)InputsCount); // inputs
            layers.Add((uint)InputsCount); // hidden
            layers.Add((uint)OutputsCount); // output
            Network = new NeuralNet(NetworkType.LAYER, layers)
            {
                LearningRate = 0.7f,
                ActivationFunctionHidden = ActivationFunction.SIGMOID,
                ActivationFunctionOutput = ActivationFunction.SIGMOID,
                ActivationSteepnessHidden = 1.0,
                ActivationSteepnessOutput = 1.0,
                TrainStopFunction = StopFunction.STOPFUNC_BIT,
                BitFailLimit = 0.01f,
                TrainingAlgorithm = TrainingAlgorithm.TRAIN_RPROP
            };

            Network.InitWeights(Data);
        }

        public static double[] GetRawData(Image image)
        {
            var bmp = image as Bitmap;
            var inputsCount = bmp.Width * bmp.Height;
            var raw = new double[inputsCount];
            for (var i = 0; i < bmp.Width; i++)
            {
                for (var j = 0; j < bmp.Height; j++)
                {
                    raw[i * bmp.Height + j] = bmp.GetPixel(i, j).ToArgb() == Color.White.ToArgb() ? 0 : 1;
                }
            }
            return raw;
        }

        public double Run(NeuralNet.TrainingCallback callback)
        {
            var lastError = 0.0;
            Network.SetCallback(callback, new object[] { });
            Network.TrainOnData(Data, 200, 5, 0.0001F);
            return lastError;
        }
    }
}
