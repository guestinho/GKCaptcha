using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.Util;
using Emgu.CV.Structure;

namespace GKCaptcha
{
    public partial class Form1 : Form
    {
        public int CaptchaWidth = 120 * 3;
        public int CaptchaHeight = 40 * 3;

        public Form1()
        {
            InitializeComponent();
            timer1.Start();
            var samples = CaptchasLibrary.GetRecognizedCaptchas();

            for (var i = 0; i < Math.Min(20, samples.Count); i++)
            {
                Controls.Add(new Panel
                {
                    Location = new Point(0, (CaptchaHeight + 3) * i),
                    Name = "imagePanel",
                    Size = new Size(CaptchaWidth, CaptchaHeight),
                    TabIndex = 0,
                    BackgroundImage = samples[i].Image,
                    BackgroundImageLayout = ImageLayout.Stretch
                });

                var img = samples[i].Image;
                img = ImageHelper.Crop(img, new Rectangle(1, 1, img.Width - 2, img.Height - 2));
                //img = ImageHelper.Binarize(img);
                //img = ImageHelper.Blur(img);
                img = ImageHelper.EdgeDetector(img);

                Controls.Add(new Panel
                {
                    Location = new Point(CaptchaWidth + 3, (CaptchaHeight + 3)*i),
                    Name = "imagePanel",
                    Size = new Size(CaptchaWidth, CaptchaHeight),
                    TabIndex = 0,
                    BackgroundImage = img,
                    BackgroundImageLayout = ImageLayout.Stretch
                });

                img = ImageHelper.RemoveLines2(img);
                Controls.Add(new Panel
                {
                    Location = new Point((CaptchaWidth + 3) * 2, (CaptchaHeight + 3) * i),
                    Name = "imagePanel",
                    Size = new Size(CaptchaWidth, CaptchaHeight),
                    TabIndex = 0,
                    BackgroundImage = img,
                    BackgroundImageLayout = ImageLayout.Stretch
                });


                var recognizer = new CaptchaRecognizer(samples[i].Image);
                var rects = recognizer.SplitDigits();

                var dividedPanel = new Panel
                {
                    Location = new Point((CaptchaWidth + 3) * 3, (CaptchaHeight + 3) * i),
                    Name = "imagePanel",
                    Size = new Size(recognizer.Target.Width, recognizer.Target.Height),
                    TabIndex = 0,
                    BackgroundImageLayout = ImageLayout.Stretch
                };

                var drawArea = new Bitmap(recognizer.Target);
                dividedPanel.BackgroundImage = drawArea;

                Graphics g = Graphics.FromImage(drawArea);
                var pen = new Pen(Brushes.Blue);

                foreach (var rect in rects)
                {
                    g.DrawRectangle(pen, rect);
                }
                Controls.Add(dividedPanel);
            }
        }

        private void teachButton_Click(object sender, EventArgs e)
        {
            _terminate = false;
            _error = 1.0;
            _progress = 0;
            _terminated = false;
            _controlError = -1;
            _updateProgress();

            new Thread(() =>
            {
                var samples = CaptchasLibrary.GetRecognizedCaptchas();
                var digitImages = new List<Image>();
                var digitResults = new List<int>();

                int controlStart = (int)(samples.Count*0.8);
                for (var i = 0; i < controlStart; i++)
                {
                    var recognizer = new CaptchaRecognizer(samples[i].Image);
                    var rectangles = recognizer.SplitDigits();

                    for (var j = 0; j < rectangles.Length; j++)
                    {
                        digitImages.Add(ImageHelper.Crop(recognizer.Target, rectangles[j]));
                        digitResults.Add(int.Parse(samples[i].Answer.Substring(j, 1)));
                    }
                }
                var teacher = new NeuralNetworkTrainer(digitImages.ToArray(), digitResults.ToArray());

                teacher.Run((net, data, epochs, reports, er, u, userData) =>
                {
                    _error = net.MSE;
                    _progress = 1.0*u/epochs;
                    if (_terminate)
                        return -1;
                    return 0;
                });
                teacher.Network.Save(CaptchaRecognizer.NetworkPath);

                var controlTotal = 0;
                var controlPasses = 0;
                for (var i = controlStart; i < samples.Count; i++)
                {
                    var rec = new CaptchaRecognizer(samples[i].Image);
                    var res = rec.Recognize();
                    controlTotal++;
                    if (res.Result == samples[i].Answer)
                        controlPasses++;
                }
                _controlError = 1.0*controlPasses/controlTotal;
                _error = teacher.Network.MSE;
                _terminate = true;
                _terminated = true;
                _progress = 1;
            }).Start();
        }

        private double _progress;
        private double _error;
        private bool _terminate = true;
        private bool _terminated = true;
        private double _controlError = -1;

        private void addSamplesButton_Click(object sender, EventArgs e)
        {
            new DataBaseCollectorForm().Show();
        }

        private RecognizerServer _server;

        private void startServerButton_Click(object sender, EventArgs e)
        {
            if (_server == null)
            {
                startServerButton.BackColor = Color.GreenYellow;
                _server = new RecognizerServer();
            }
        }

        private void _updateProgress()
        {
            teachProgressBar.Value = (int)Math.Round(_progress * 100);
            errorLabel.Text = "error: " + Math.Round(_error, 6);
            if (_controlError >= 0)
                errorLabel.Text += " (" + Math.Round(_controlError*100) + "%)";
            if (_terminate)
                teachButton.BackColor = DefaultBackColor;
            else
                teachButton.BackColor = Color.GreenYellow;

            if (_terminate && !_terminated)
                teachStopButton.BackColor = Color.Red;
            else
                teachStopButton.BackColor = DefaultBackColor;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _updateProgress();
        }

        private void teachStopButton_Click(object sender, EventArgs e)
        {
            _terminate = true;
        }
    }
}
