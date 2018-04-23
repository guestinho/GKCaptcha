using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace GKCaptcha
{
    public partial class DataBaseCollectorForm : Form
    {
        public DataBaseCollectorForm()
        {
            InitializeComponent();
            _next();
        }

        private Captcha _currentCaptcha;

        private void _next()
        {
            try
            {
                _currentCaptcha = GKRegistration.GetNextCaptcha();
            }
            catch (Exception e)
            {
                Thread.Sleep(100);
                MessageBox.Show(e.Message, "Error");
                _next();
                return;
            }
            imagePanel.BackgroundImage = _currentCaptcha.Image;
            imagePanel.BackgroundImageLayout = ImageLayout.Stretch;
            answerTextBox.Text = "";
            randLabel.Text = _currentCaptcha.Name;

            var img = _currentCaptcha.Image;
            img = ImageHelper.Crop(img, new Rectangle(1, 1, img.Width - 2, img.Height - 2));
            img = ImageHelper.Binarize(img);
            img = ImageHelper.RemoveLines(img);
            
            var recognizer = new CaptchaRecognizer(_currentCaptcha.Image);
            var rects = recognizer.SplitDigits();

            var drawArea = new Bitmap(img);
            hintImagePanel.BackgroundImage = drawArea;
            hintImagePanel.BackgroundImageLayout = ImageLayout.Stretch;

            Graphics g = Graphics.FromImage(drawArea);
            var pen = new Pen(Brushes.Blue);

            foreach (var rect in rects)
                g.DrawRectangle(pen, rect);
        }

        private void _save()
        {
            if (!Regex.IsMatch(answerTextBox.Text, @"^\d\d\d\d$"))
            {
                MessageBox.Show("Please recheck your answer", "Error");
                return;
            }
            _currentCaptcha.Answer = answerTextBox.Text;
            _currentCaptcha.SaveToLibrary();
            _next();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            _save();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            _next();
        }

        private void answerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _save();
            }
        }
    }
}
