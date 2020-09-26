using System;
using System.Drawing;
using FastBitmapLib;

namespace ComputerGraphicsApplication.Models.Filters
{
    public class RotateFilter : AbstractFilter
    {
        private int _degreeRotate;

        public RotateFilter(string valueFilter)
        {
            _degreeRotate = int.Parse(valueFilter);
        }

        public override Bitmap Apply(Bitmap bitmap)
        {
            var result = new Bitmap(bitmap.Width, bitmap.Height);

            var degree = _degreeRotate * Math.PI / 180;

            var halfWidth = bitmap.Width / 2;
            var halfHeight = bitmap.Height / 2;

            using (var fastBitmap = bitmap.FastLock())
            using (var fastResultBitmap = result.FastLock())
            {
                for (var x = -halfWidth; x < halfWidth; x++)
                {
                    for (var y = -halfHeight; y < halfHeight; y++)
                    {
                        var xs = (int) (x * Math.Cos(degree) - y * Math.Sin(degree) + halfWidth);
                        var ys = (int) (x * Math.Sin(degree) + y * Math.Cos(degree) + halfHeight);

                        if (xs >= 0 && xs < bitmap.Width &&
                            ys >= 0 && ys < bitmap.Height)
                        {
                            fastResultBitmap.SetPixel(xs, ys, fastBitmap.GetPixel(x + halfWidth, y + halfHeight));
                        }
                    }
                }
            }

            return result;
        }
    }
}