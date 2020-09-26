using System;
using System.Drawing;
using FastBitmapLib;

namespace ComputerGraphicsApplication.Models.Filters
{
    public class SobelFilter : AbstractFilter
    {
        public override Bitmap Apply(Bitmap bitmap)
        {
            var sx = new double[3, 3];
            sx[0, 0] = -1 + 0.7;
            sx[0, 1] = -2 + 0.7;
            sx[0, 2] = -1 + 0.7;
            sx[1, 0] = 0;
            sx[1, 1] = 0;
            sx[1, 2] = 0;
            sx[2, 0] = 1 - 0.7;
            sx[2, 1] = 2 - 0.7;
            sx[2, 2] = 1 - 0.7;

            var width = bitmap.Width;
            var height = bitmap.Height;
            using var fastBitmap = bitmap.FastLock();
            var redMatrix = GetColorMatrix(width, height, sx, (x, y) => fastBitmap.GetPixel(x, y).R);
            var greenMatrix = GetColorMatrix(width, height, sx, (x, y) => fastBitmap.GetPixel(x, y).G);
            var blueMatrix = GetColorMatrix(width, height, sx, (x, y) => fastBitmap.GetPixel(x, y).B);

            var result = new Bitmap(width, height);
            using var fastResultBitmap = result.FastLock();
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    fastResultBitmap.SetPixel(x, y, Color.FromArgb((int) redMatrix[x, y],
                                                                   (int) greenMatrix[x, y],
                                                                   (int) blueMatrix[x, y]));
                }
            }

            return result;
        }

        private static double[,] GetColorMatrix(int width, int height, double[,] sx, Func<int, int, double> getColor)
        {
            var g = new double[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    g[x, y] = getColor(x, y);
                }
            }

            var sy = MatrixTransposition(sx);
            var newG = new double[width, height];
            for (var x = sx.GetLength(0) / 2; x < width - sx.GetLength(0) / 2; x++)
            {
                for (var y = sx.GetLength(0) / 2; y < height - sx.GetLength(0) / 2; y++)
                {
                    var gx = MatrixMultiplication(GetMatrix(g, x, y, sx.GetLength(0) / 2), sx);
                    var gy = MatrixMultiplication(GetMatrix(g, x, y, sx.GetLength(0) / 2), sy);
                    newG[x, y] = Math.Clamp((int) Math.Sqrt(gx * gx + gy * gy), 0, 255);
                }
            }

            return newG;
        }

        private static double[,] GetMatrix(double[,] originalMatrix, int x, int y, int r)
        {
            var matrix = new double[r * 2 + 1, r * 2 + 1];
            for (var i = x - r; i <= x + r; i++)
            {
                for (var j = y - r; j <= y + r; j++)
                {
                    matrix[i - x + r, j - y + r] = originalMatrix[i, j];
                }
            }
            return matrix;
        }

        private static double[,] MatrixTransposition(double[,] originalMatrix)
        {
            var matrixTransposition = new double[originalMatrix.GetLength(1), originalMatrix.GetLength(0)];
            for (var i = 0; i < originalMatrix.GetLength(1); i++)
            {
                for (var j = 0; j < originalMatrix.GetLength(0); j++)
                {
                    matrixTransposition[i, j] = originalMatrix[j, i];
                }
            }
            return matrixTransposition;
        }

        private static double MatrixMultiplication(double[,] matrix1, double[,] matrix2)
        {
            var sum = 0.0;
            for (var x = 0; x < matrix1.GetLength(0); x++)
            {
                for (var y = 0; y < matrix1.GetLength(1); y++)
                {
                    sum += matrix1[x, y] * matrix2[x, y];
                }
            }
            return sum;
        }
    }
}