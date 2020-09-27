using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using ComputerGraphicsApplication.Interfaces;

namespace ComputerGraphicsApplication.Services
{
    public class ImageService : IService
    {
        public Image GetImageFromStream(Stream stream)
        {
            return Image.FromStream(stream);
        }

        public Bitmap GetBitmapFromStream(Stream stream)
        {
            var image = GetImageFromStream(stream);
            return new Bitmap(image);
        }

        public Bitmap NormalizeImage(Image image)
        {
            var width = image.Width;
            var height = image.Height;

            if (width <= 500 && height <= 500)
            {
                return new Bitmap(image);
            }

            var maxSide = Math.Max(width, height);
            var factor = maxSide / 500d;

            width = (int) Math.Round(width / factor);
            height = (int) Math.Round(height / factor);

            return ResizeImage(image, width, height);
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}