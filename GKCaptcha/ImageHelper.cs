using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI.GLView;

namespace GKCaptcha
{
    class ImageHelper
    {
        public static Image Binarize(Image img)
        {
            var image = new Image<Gray, Byte>((Bitmap)img);
            CvInvoke.Threshold(image, image, 180, 255, ThresholdType.Binary);
            return image.ToBitmap();
        }

        public static Image Binarize2(Image img)
        {
            var image = new Image<Gray, Byte>((Bitmap)img);
            
            //CvInvoke.AdaptiveThreshold(image, image, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 5, 0);
            CvInvoke.Threshold(image, image, 230, 255, ThresholdType.Binary);
            return image.ToBitmap();
        }


        public static Image Crop(Image img, Rectangle cropArea)
        {
            var bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        public static Image EdgeDetector(Image img)
        {
            var image = new Image<Gray, Byte>((Bitmap)img);

            image = image.Canny(3, 3);

            return image.ToBitmap();
        }

        public static Image RemoveLines(Image img)
        {
            var image = new Image<Gray, Byte>((Bitmap)img);
            
            _dilate(image, 2, 3);
            _erode(image, 2, 2);

            //_erode(image, 2, 2);

            return image.ToBitmap();
        }

        public static Image RemoveLines2(Image img)
        {
            var image = new Image<Gray, Byte>((Bitmap)img);

            _dilate(image, 3, 5);
            //CvInvoke.GaussianBlur(image, image, new Size(3, 3), 1, 1);
            _erode(image, 3, 3);
            //CvInvoke.Threshold(image, image, 220, 255, ThresholdType.Binary);

            //var hsv = CvInvoke.cvCreateImage(CvInvoke.cvGetSize(image), IplDepth.IplDepth_8U, 1);
            //var h = CvInvoke.cvCreateImage(CvInvoke.cvGetSize(image), IplDepth.IplDepth_8U, 1);
            //var s = CvInvoke.cvCreateImage(CvInvoke.cvGetSize(image), IplDepth.IplDepth_8U, 1);
            //var v = CvInvoke.cvCreateImage(CvInvoke.cvGetSize(image), IplDepth.IplDepth_8U, 1);
            //CvInvoke.CvtColor(image, hsv, ColorConversion.Bgr2Hsv);

            return image.ToBitmap();
        }

        public static Image Blur(Image img)
        {
            var image = new Image<Bgr, Byte>((Bitmap)img);

            //CvInvoke.GaussianBlur(image, image, new Size(3, 3), 1, 1);

            return image.ToBitmap();
        }

        private static void _erode(Image<Gray, Byte> image, int w, int h)
        {
            var element = new Image<Gray, Byte>(w, h, new Gray(1));

            CvInvoke.Erode(image, image, element, new Point(-1, -1), 1, BorderType.Constant,
                CvInvoke.MorphologyDefaultBorderValue);
        }

        private static void _dilate(Image<Gray, Byte> image, int w, int h)
        {
            var element = new Image<Gray, Byte>(w, h, new Gray(1));

            CvInvoke.Dilate(image, image, element, new Point(-1, -1), 1, BorderType.Constant,
                CvInvoke.MorphologyDefaultBorderValue);
        }
    }
}
