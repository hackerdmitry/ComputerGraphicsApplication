using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FastBitmapLib;

namespace ComputerGraphicsApplication.Models.Filters
{
    public class ClearArtefactsFilter : AbstractFilter
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
                        var pixelsList = new List<(double, Color)>();

                        for (var i = x - 1; i <= x + 1; i++)
                        {
                            for (var j = y - 1; j <= y + 1; j++)
                            {
                                if (i == j || !IsInBorder(i, j, bitmap.Width, bitmap.Height))
                                {
                                    continue;
                                }

                                var color = fastBitmap.GetPixel(i, j);
                                var brightness = GetBrightness(color);
                                pixelsList.Add((brightness, color));
                            }
                        }

                        var medianColor = GetMedianColor(pixelsList.OrderBy(p => p.Item1).Select(p => p.Item2).ToArray());
                        fastResultBitmap.SetPixel(x, y, medianColor);
                    }
                }
            }

            return result;
        }

        private bool IsInBorder(int x, int y, int width, int height)
        {
            return x >= 0 && x < width &&
                   y >= 0 && y < height;
        }

        private double GetBrightness(Color color)
        {
            return 0.3 * color.R + 0.59 * color.G + 0.11 * color.B;
        }

        private Color GetMedianColor(Color[] orderedPixels)
        {
            var isEven = orderedPixels.Length % 2 == 0;
            var leftMiddle = orderedPixels.Length / 2;

            return isEven
                ? GetAverageColor(orderedPixels[leftMiddle - 1], orderedPixels[leftMiddle])
                : orderedPixels[leftMiddle];
        }

        private Color GetAverageColor(Color c1, Color c2)
        {
            var average = new Func<int, int, int>((o1, o2) => (o1 + o2) / 2);
            return Color.FromArgb(average(c1.R, c2.R), average(c1.G, c2.G), average(c1.B, c2.B));
        }
    }
}