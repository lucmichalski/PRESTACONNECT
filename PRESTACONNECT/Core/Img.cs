using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace PRESTACONNECT.Core
{
    public static class Img
    {
        public const string jpgExtension = ".jpg";
        public const string pngExtension = ".png";
        public const string gifExtension = ".gif";
        public static List<string> imageExtensions = getImageExtensions();
        private static List<string> getImageExtensions()
        {
            List<string> list = new List<string>();
            list.Add(jpgExtension);
            list.Add(pngExtension);
            list.Add(gifExtension);
            return list;
        }

        public static void resizeImage(Image originalFile, int canvasWidth, int canvasHeight, string newfile)
        {
            string extension = System.IO.Path.GetExtension(newfile).ToLower();

            System.Drawing.Image thumbnail = new Bitmap(canvasWidth, canvasHeight);
            System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBilinear;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            // ACTIVE IF PICTURES WITH VERY SMALL DEFINITION
            //if (ratio > 2 && originalHeight < 300 && originalWidth < 300)
            //    ratio = 2;

            int sourceWidth = originalFile.Width;
            int sourceHeight = originalFile.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)canvasWidth / (float)sourceWidth);
            nPercentH = ((float)canvasHeight / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((canvasWidth - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((canvasHeight - (sourceHeight * nPercent)) / 2);
            }
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            if (extension == jpgExtension)
            {
                graphic.Clear(Color.White);
            }
            else
            {
                graphic.Clear(Color.Transparent);
            }
            graphic.DrawImage(originalFile,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            System.Drawing.Imaging.ImageCodecInfo[] info =
                             ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality,
                             100L);

            switch (extension)
            {
                case pngExtension:
                case gifExtension:
                    thumbnail.Save(newfile, ImageFormat.Png);
                    break;
                case jpgExtension:
                default:
                    thumbnail.Save(newfile, info[1], encoderParameters);
                    break;
            }
        }
    }
}
