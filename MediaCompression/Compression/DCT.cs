using System;
using System.Threading.Tasks;

namespace Compression {
    public class Dct {
        private const double Pi = Math.PI;
        private double[,] _data;
        private double[,] _inv;

        /// <summary>
        ///     the C function as defined in the book
        /// </summary>
        /// <param name="input">value of C for function of DCT</param>
        /// <returns></returns>
        private static double C(int input) {
            if (input != 0) {
                return 1;
            }
            return Math.Sqrt(2)/2;
        }

        /// <summary>
        ///     gets U,V value for dct
        /// </summary>
        /// <param name="u">u position</param>
        /// <param name="v">v position</param>
        /// <returns></returns>
        private double GetValueForward(int u, int v) {
            double freq = 0;
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    freq +=
                        Math.Cos((2*i + 1)*u*Pi/16)*
                        Math.Cos((2*j + 1)*v*Pi/16)*
                        _data[i, j];
                }
            }

            freq *= 2*C(u)*C(v)/Math.Sqrt(8*8);
            return freq;
        }

        /// <summary>
        ///     Performs dct for one value. Used y GoBack()
        /// </summary>
        /// <param name="i">location to be looked for</param>
        /// <param name="j">part of point</param>
        /// <returns></returns>
        private double GetValueBackwards(int i, int j) {
            double intensity = 0;
            for (int u = 0; u < 8; u++) {
                for (int v = 0; v < 8; v++) {
                    intensity +=
                        C(u)*C(v)/4*
                        Math.Cos((2*i + 1)*u*Pi/16)*
                        Math.Cos((2*j + 1)*v*Pi/16)*
                        _inv[u, v];
                }
            }
            if (intensity < 0) {
                intensity = 0;
            }
            return intensity;
        }

        /// <summary>
        ///     The function that performs forward dct using Userlevel threads
        /// </summary>
        /// <param name="d">8x8 block to be worked with</param>
        /// <returns></returns>
        public double[,] Go(double[,] d) {
            _data = d;
            Task<double[,]> task = Task<double[,]>.Factory.StartNew(() => {
                double[,] output = new double[8, 8];
                for (int x = 0; x < _data.GetLength(0); x++)
                    for (int y = 0; y < _data.GetLength(1); y++) {
                        output[x, y] = GetValueForward(x, y);
                    }
                return output;
            });
            return task.Result;
        }

        /// <summary>
        ///     Function that launches use-level threads to calculate idct faster.
        /// </summary>
        /// <param name="dct">the input 8x8 block</param>
        /// <returns></returns>
        public double[,] GoBack(double[,] dct) {
            _inv = dct;
            Task<double[,]> task = Task<double[,]>.Factory.StartNew(() => {
                double[,] data = new double[8, 8];

                for (int i = 0; i < dct.GetLength(0); i++)
                    for (int j = 0; j < dct.GetLength(1); j++) {
                        data[i, j] = GetValueBackwards(i, j);
                    }

                return data;
            });
            return task.Result;
        }
    }
}