using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Compression {
    internal static class Padder {
        /// <summary>
        ///     Pads the image so it can be worked with
        /// </summary>
        /// <param name="originalImage">Image to be padded</param>
        /// <returns></returns>
        public static Image PadImage(Image originalImage) {
            int largestDimension = Math.Max(originalImage.Height, originalImage.Width);
            while (largestDimension%64 != 0) {
                largestDimension++;
            }
            Size squareSize = new Size(largestDimension, largestDimension);
            Bitmap squareImage = new Bitmap(squareSize.Width, squareSize.Height);
            using (Graphics graphics = Graphics.FromImage(squareImage)) {
                graphics.FillRectangle(Brushes.White, 0, 0, squareSize.Width, squareSize.Height);
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                graphics.DrawImage(originalImage, /*(squareSize.Width/2) - (originalImage.Width/2)*/ 0,
                    /*(squareSize.Height/2) - (originalImage.Height/2)*/ 0, originalImage.Width, originalImage.Height);
            }
            return squareImage;
        }

        /// <summary>
        ///     Crops the image back down to remove the border
        /// </summary>
        /// <param name="source">image to crop</param>
        /// <param name="section">crop params</param>
        /// <returns></returns>
        public static Bitmap CropImage(Bitmap source, Rectangle section) {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }
    }
}