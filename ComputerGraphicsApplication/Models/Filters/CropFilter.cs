using System;
using System.Drawing;
using System.Text.RegularExpressions;
using FastBitmapLib;

namespace ComputerGraphicsApplication.Models.Filters
{
    public class CropFilter : AbstractFilter
    {
        private Point _p1;
        private Point _p2;

        public CropFilter(string valueFilter)
        {
            var matchValue = Regex.Match(valueFilter, "^(\\d+),(\\d+),(\\d+),(\\d+)$");
            if (!matchValue.Success)
            {
                throw new ArgumentException("Invalid value");
            }

            var x1 = int.Parse(matchValue.Groups[1].Value);
            var y1 = int.Parse(matchValue.Groups[2].Value);
            var x2 = int.Parse(matchValue.Groups[3].Value);
            var y2 = int.Parse(matchValue.Groups[4].Value);

            _p1 = new Point(Math.Min(x1, x2), Math.Min(y1, y2));
            _p2 = new Point(Math.Max(x1, x2), Math.Max(y1, y2));
        }

        public override Bitmap Apply(Bitmap bitmap)
        {
            var width = _p2.X - _p1.X + 1;
            var height = _p2.Y - _p1.Y + 1;

            var result = new Bitmap(width, height);

            using (var fastBitmap = bitmap.FastLock())
            using (var fastResultBitmap = result.FastLock())
            {
                for (var x = _p1.X; x <= _p2.X; x++)
                {
                    for (var y = _p1.Y; y <= _p2.Y; y++)
                    {
                        var color = fastBitmap.GetPixel(x, y);
                        fastResultBitmap.SetPixel(x - _p1.X, y - _p1.Y, color);
                    }
                }
            }

            return result;
        }
    }
}