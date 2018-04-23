using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using FANNCSharp.Double;

namespace GKCaptcha
{
    class CaptchaRecognizer
    {
        public int DigitWidth = 18;
        public int DigitHeight = 26;
        public int DigitsCount = 4;
        public static Random Random = new Random();
        public static string NetworkPath = @"G:\GKData\test_network.txt";
        public Rectangle[] Rectangles;
        public Image[] Images;
        public static int MaxConsecutiveFailures = 70000;

        public Image Target;
        private int[,] _sum;
        private static NeuralNet _network;

        public CaptchaRecognizer(Image img)
        {
            img = ImageHelper.Crop(img, new Rectangle(1, 1, img.Width - 2, img.Height - 2));
            img = ImageHelper.Binarize(img);
            img = ImageHelper.RemoveLines(img);

            Target = img;
            _sum = new int[Target.Width + 1, Target.Height + 1];
            var bmp = (Bitmap) Target;
            for(var x = 0; x < Target.Width; x++)
                for (var y = 0; y < Target.Height; y++)
                    _sum[x + 1, y + 1] = (bmp.GetPixel(x, y).ToArgb() != Color.White.ToArgb() ? 1 : 0)
                                         + _sum[x, y + 1]
                                         + _sum[x + 1, y]
                                         - _sum[x, y];
        }

        private int _getSum(int x, int y)
        {
            return _sum[x + DigitWidth, y + DigitHeight]
                   - _sum[x, y + DigitHeight]
                   - _sum[x + DigitWidth, y]
                   + _sum[x, y];
        }

        public Rectangle[] SplitDigits()
        {
            var population = new GeneticPopolation(this, 30);
            var fitnesses = new List<double>();
            for (var i = 0; i < 400; i++)
            {
                population.DoIteration();
                fitnesses.Add(population.Individuals[0].Fitness());
                if (i >= 5 && Math.Abs(fitnesses[i - 5] - fitnesses[i]) < 1e-4)
                    break;
            }
            return population.Individuals[0].GetRectangles();
        }

        public class CantSplitDigitsException : Exception 
        {
            
        }

        public class RecognizeResult
        {
            public string Result;
            public double[] Probabilities;

            public double Probability
            {
                get
                {
                    return Probabilities.Aggregate<double, double>(1, (current, p) => current*p);
                }
            }
        }

        public RecognizeResult Recognize()
        {
            if (_network == null && File.Exists(NetworkPath))
            {
                _network = new NeuralNet(NetworkPath);
            }

            Rectangles = SplitDigits();
            Images = new Image[Rectangles.Length];
            var result = new RecognizeResult {Probabilities = new double[DigitsCount]};
            for (var j = 0; j < Rectangles.Length; j++)
            {
                Images[j] = ImageHelper.Crop(Target, Rectangles[j]);
                result.Result += _compute(Images[j], out result.Probabilities[j]);
            }
            return result;
        }

        private double[] _computeProbabs(Image image)
        {
            return _network.Run(NeuralNetworkTrainer.GetRawData(image));
        }

        private int _compute(Image image, out double probability)
        {
            var probabs = _computeProbabs(image);
            var sel = 1;
            for (var i = 1; i < probabs.Length; i++)
                if (probabs[i] > probabs[sel])
                    sel = i;
            probability = probabs[sel];
            return sel + 1;
        }

        private class GeneticPopolation
        {
            public List<GeneticIndividual> Individuals = new List<GeneticIndividual>();
            private CaptchaRecognizer _recognizer;
            public int MaxSize;

            public GeneticPopolation(CaptchaRecognizer recognizer, int maxSize)
            {
                _recognizer = recognizer;
                MaxSize = maxSize;
                var consecutiveInvalidCount = 0;
                while(Individuals.Count < MaxSize)
                {
                    var nxt = GeneticIndividual.Mutation(new GeneticIndividual(_recognizer, true));
                    if (!nxt.IsInvalid())
                    {
                        consecutiveInvalidCount = 0;
                        Individuals.Add(nxt);
                    }
                    else
                    {
                        consecutiveInvalidCount++;
                        if (consecutiveInvalidCount > MaxConsecutiveFailures)
                            throw new CantSplitDigitsException();
                    }
                }
            }

            public void DoIteration()
            {
                int size = Individuals.Count;
                for (var i = 0; i < size; i++)
                {
                    for (var j = 0; j < i; j++)
                        Individuals.Add(GeneticIndividual.Crossover(Individuals[i], Individuals[j]));
                    for (var j = 0; j < 100; j++)
                        Individuals.Add(GeneticIndividual.Mutation(Individuals[i]));
                }
                Individuals = Individuals
                    .Where(x => !x.IsInvalid())
                    .OrderBy(x => x.Fitness())
                    .Reverse()
                    .Take(MaxSize)
                    .ToList();
            }
        }

        private class GeneticIndividual
        {
            public GeneticIndividualItem[] Items;
            private CaptchaRecognizer _recognizer;

            public GeneticIndividual(CaptchaRecognizer recognizer, bool init = false)
            {
                _recognizer = recognizer;
                if (init)
                {
                    Items = Enumerable.Range(0, _recognizer.DigitsCount)
                        .Select(GenerateRandomItem)
                        .ToArray();
                }
                else
                {
                    Items = new GeneticIndividualItem[_recognizer.DigitsCount];
                }
            }

            public GeneticIndividualItem GenerateRandomItem(int pos)
            {
                var d = _recognizer.Target.Width/_recognizer.DigitsCount;
                var ret = new GeneticIndividualItem(_recognizer,
                    d*pos + (d - _recognizer.DigitWidth)/2,
                    (_recognizer.Target.Height - _recognizer.DigitHeight)/2
                    );
                ret = GeneticIndividualItem.Mutation(ret);
                return ret;
            }

            public static GeneticIndividual Crossover(GeneticIndividual a, GeneticIndividual b)
            {
                var ret = new GeneticIndividual(a._recognizer);
                for (var i = 0; i < a.Items.Length; i++)
                    ret.Items[i] = GeneticIndividualItem.Crossover(a.Items[i], b.Items[i]);
                return ret;
            }

            public static GeneticIndividual Mutation(GeneticIndividual a)
            {
                var ret = new GeneticIndividual(a._recognizer);
                var idx = Random.Next(a._recognizer.DigitsCount);
                for (var i = 0; i < a.Items.Length; i++)
                    ret.Items[i] = i == idx 
                        ? GeneticIndividualItem.Mutation(a.Items[i])
                        : a.Items[i];
                return ret;
            }

            public double Fitness()
            {
                double sum = 0;
                foreach (var i in Items)
                    sum += i.Fitness();
                return sum;
            }

            public bool IsInvalid()
            {
                
                for (var i = 1; i < Items.Length; i++)
                {
                    // пересекаются
                    if (Items[i].X < Items[i - 1].X + _recognizer.DigitWidth)
                        return true;
                    // слишком далеко
                    if (Items[i].X - _recognizer.DigitWidth > Items[i - 1].X + _recognizer.DigitWidth)
                        return true;
                }
                for(var i = 0; i < Items.Length; i++)
                    if (Items[i].Fitness() < 0.05)
                        return true;
                return false;
            }

            public Rectangle[] GetRectangles()
            {
                return Items.Select(x => x.GetRectangle()).ToArray();
            }
        }

        private class GeneticIndividualItem
        {
            public int X, Y;
            private CaptchaRecognizer _recognizer;

            public GeneticIndividualItem(CaptchaRecognizer recognizer, int x, int y)
            {
                _recognizer = recognizer;
                X = x;
                Y = y;
            }

            public double Fitness()
            {
                var cnt = _recognizer._getSum(X, Y);
                return (double) cnt/(_recognizer.DigitWidth*_recognizer.DigitHeight);
            }

            public static GeneticIndividualItem Crossover(GeneticIndividualItem a, GeneticIndividualItem b)
            {
                return new GeneticIndividualItem(a._recognizer, (a.X + b.X) / 2, (a.Y + b.Y) / 2);
            }

            public static GeneticIndividualItem Mutation(GeneticIndividualItem a)
            {
                var maxDeltaX = 20;
                var maxDeltaY = 5;
                var dx = Random.Next(maxDeltaX * 2 + 1) - maxDeltaX;
                var dy = Random.Next(maxDeltaY * 2 + 1) - maxDeltaY;
                return new GeneticIndividualItem(a._recognizer, 
                    Math.Min(Math.Max(0, a.X + dx), a._recognizer.Target.Width - a._recognizer.DigitWidth),
                    Math.Min(Math.Max(0, a.Y + dy), a._recognizer.Target.Height - a._recognizer.DigitHeight)
                    );
            }

            public Rectangle GetRectangle()
            {
                return new Rectangle(X, Y, _recognizer.DigitWidth, _recognizer.DigitHeight);
            }
        }
    }

}
