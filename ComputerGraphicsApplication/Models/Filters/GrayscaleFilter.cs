using System;
using System.Drawing;
using FastBitmapLib;

namespace ComputerGraphicsApplication.Models.Filters
{
    public class GrayscaleFilter : AbstractFilter
    {
        public override Bitmap Apply(Bitmap bitmap)
        {
            var result = new Bitmap(bitmap.Width, bitmap.Height);

            using (var fastBitmap = bitmap.FastLock())
            using (var fastResultBitmap = result.FastLock())
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    for (var y = 0; y < bitmap.Height; y++)
                    {
                        var r = fastBitmap.GetPixel(x, y).R * 0.299d;
                        var g = fastBitmap.GetPixel(x, y).G * 0.587d;
                        var b = fastBitmap.GetPixel(x, y).B * 0.114d;
                        var grayscale = Normalize(0, (byte) (r + g + b), 255);

                        fastResultBitmap.SetPixel(x, y, Color.FromArgb(grayscale, grayscale, grayscale));
                    }
                }
            }

            return result;
        }

        private int Normalize(int start, int mid, int end)
        {
            return Math.Max(start, Math.Min(mid, end));
        }
    }
}