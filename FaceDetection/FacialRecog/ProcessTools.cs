using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacialRecog {

    internal static class ProcessTools {
        public static List<List<T>> GetSquare<T>(this List<List<T>> data, int x, int y, int block) {
            List<List<T>> output = new List<List<T>>();
            for (int i = x; i < block + x; i++) {
                List<T> dat = new List<T>();
                for (int j = y; j < block + y; j++) {
                    dat.Add(data[i][j]);
                }
                output.Add(dat);
            }
            return output;
        }

        public static double[,] GetSquare(this double[,] input, int x, int y, int block) {
            double[,] output = new double[block, block];

            for (int i = x; i < x + block; i++) {
                for (int j = y; j < y + block; j++) {
                    output[i - x, j - y] = input[i, j];
                }
            }

            return output;
        }

        public static double[,] Transpose(this double[,] avg) {
            double[,] navg = new double[avg.GetLength(0), avg.GetLength(1)];
            for (int i = 0; i < avg.GetLength(0); i++) {
                for (int j = 0; j < avg.GetLength(1); j++) {
                    navg[i, j] = avg[j, i];
                }
            }
            return navg;
        }

        public static double[,] GetCenteredSquare(this double[,] input, int x, int y, int block) {
            double[,] output = new double[block, block];

            for (int i = x; i < x + block/2; i++) {
                for (int j = y; j < y + block/2; j++) {
                    output[i - x - block/2, j - y - block/2] = input[i, j];
                }
            }

            return output;
        }

        


        public static bool CheckSquare(this double[,] input, byte color) {
            return input.Cast<double>().Any(d => (byte) Math.Round(d) == color);
        }


        public static double[,] RotateImage(this double[,] input) {
            double [,] output = new double[input.GetLength(0),input.GetLength(1)];

            for (int y = 0; y < input.GetLength(0); y++) {
                for (int x = 0; x < input.GetLength(1); x++) {
                    output[y, x] = input[x, y];
                }
            }

            return output;
        }

        public static double[,] ImageToArray(this Image input) {
            double[,] output = new double[input.Width,input.Height];
            Bitmap inmap = new Bitmap(input);
            unsafe {
                BitmapData mapdata = inmap.LockBits(new Rectangle(0, 0, inmap.Width, inmap.Height),
                    ImageLockMode.ReadWrite,
                    inmap.PixelFormat);

                int bpp = Image.GetPixelFormatSize(mapdata.PixelFormat)/8;
                int widthInP = mapdata.Width*bpp;
                byte* pointer = (byte*)mapdata.Scan0;

                Parallel.For(0, mapdata.Height, y => {
                    byte* curline = pointer + y*mapdata.Stride;
                    for (int x = 0; x < widthInP; x+=bpp) {
                        output[x/bpp, y] = curline[x];
                    }
                });
                inmap.UnlockBits(mapdata);
            }
            return output;
        }



        public static Bitmap ImageFromArray(this double[,] input) {
            Bitmap output = new Bitmap(input.GetLength(0), input.GetLength(1));
            unsafe {
                BitmapData bitmapData =
                    output.LockBits(new Rectangle(0, 0, output.Width, output.Height),
                        ImageLockMode.ReadWrite, output.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(output.PixelFormat)/8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width*bytesPerPixel;
                byte* ptrFirstPixel = (byte*) bitmapData.Scan0;

                Parallel.For(0, heightInPixels, y => {
                    byte* currentLine = ptrFirstPixel + y*bitmapData.Stride;
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel) {
                        int oldBlue = (int)Math.Round(input[x / bytesPerPixel, y]);
                        int oldGreen = (int)Math.Round(input[x / bytesPerPixel, y]);
                        int oldRed = (int)Math.Round(input[x / bytesPerPixel, y]);

                        currentLine[x] = (byte) oldBlue;
                        currentLine[x + 1] = (byte) oldGreen;
                        currentLine[x + 2] = (byte) oldRed;
                        currentLine[x + 3] = 255;
                    }
                });
                output.UnlockBits(bitmapData);
            }
            return output;
        }

        public static double[,] HalfSize(this double[,] data) {
            double[,] output = new double[data.GetLength(0)/2,data.GetLength(1)/2];
            
            int ystep = 0;
            for (int y = 0; y < data.GetLength(0); y ++) {
                if (y%2 != 0) continue;
                int xstep = 0;
                for (int x = 0; x < data.GetLength(1); x++) {
                    if (x%2 != 0) continue;
                    output[xstep++, ystep] = data[x, y];
                }
                ystep++;
            }
            return output;
        }

        public static double[,] Upsize(this double[,] data) {
            double[,] output = new double[256,256];

            int height = data.GetLength(1);
            int width = data.GetLength(0);

            int yspace = (256 - height)/2;
            int xspace = (256 - width)/2;


            for (int y = yspace; y < yspace + height; y++) {
                for (int x = xspace; x < xspace + width; x++) {
                    output[x, y] = data[x - xspace, y - yspace];
                }
            }

            return output;
        }


        public static T[,] CreateRectangularArray<T>(this IList<IList<T>> arrays) {
            int minorLength = arrays[0].Count;
            T[,] ret = new T[arrays.Count, minorLength];
            for (int i = 0; i < arrays.Count; i++) {
                IList<T> array = arrays[i];
                if (array.Count != minorLength) {
                    throw new ArgumentException
                        ("All arrays must be the same length");
                }
                for (int j = 0; j < minorLength; j++) {
                    ret[i, j] = array[j];
                }
            }
            return ret;
        }

        public static double[,] CreateRectangularArray(this List<List<double>> arrays) {
            int minorLength = arrays[0].Count;
            double[,] ret = new double[arrays.Count, minorLength];
            for (int i = 0; i < arrays.Count; i++) {
                List<double> array = arrays[i];
                if (array.Count != minorLength) {
                    throw new ArgumentException
                        ("All arrays must be the same length");
                }
                for (int j = 0; j < minorLength; j++) {
                    ret[i, j] = array[j];
                }
            }
            return ret;
        }


        public static void MakeSquarePowerOfTwo(this BoxData data, int stride) {
            if ((data.Rightmost - data.LeftMost)%stride == 0) return;
            if ((data.Lowest - data.Highest)%stride == 0) return;

            int amount = (data.Rightmost - data.LeftMost)/stride;
            int number = stride*(amount + 1);
            number = number - (data.Rightmost - data.LeftMost);
            for (int i = 0; i < number; i++) {
                if (data.LeftMost - 1 > 0) {
                    data.LeftMost--;
                }else if (data.Rightmost + 1 < 512) {
                    data.Rightmost ++;
                }
            }

            amount = (data.Lowest - data.Highest) / stride;
            number = stride * (amount + 1);
            number = number - (data.Lowest - data.Highest);

            for (int i = 0; i < number; i++) {
                if (data.Highest - 1 > 0) {
                    data.Highest--;
                } else if (data.Lowest + 1 < 512) {
                    data.Lowest++;
                }
            }

        }

        public static void InvokeIfRequired(this ISynchronizeInvoke obj,
            MethodInvoker action) {
            if (obj.InvokeRequired) {
                var args = new object[0];
                obj.Invoke(action, args);
            } else {
                action();
            }
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
    Func<TSource, TKey> selector, IComparer<TKey> comparer = null) {
            comparer = comparer ?? Comparer<TKey>.Default;
            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator()) {
                if (!sourceIterator.MoveNext()) {
                    throw new InvalidOperationException("Sequence was empty");
                }
                TSource max = sourceIterator.Current;
                TKey maxKey = selector(max);
                while (sourceIterator.MoveNext()) {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) > 0) {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return max;
            }
        }
    }
}