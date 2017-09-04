using System;

namespace Compression {
    internal static class ZigZag {
        /// <summary>
        ///     Order in which zigzag is done
        /// </summary>
        private static int[] Zagorder { get; } = {
            0, 1, 5, 6, 14, 15, 27, 28,
            2, 4, 7, 13, 16, 26, 29, 42,
            3, 8, 12, 17, 25, 30, 41, 43,
            9, 11, 18, 24, 31, 40, 44, 53,
            10, 19, 23, 32, 39, 45, 52, 54,
            20, 22, 33, 38, 46, 51, 55, 60,
            21, 34, 37, 47, 50, 56, 59, 61,
            35, 36, 48, 49, 57, 58, 62, 63
        };

        /// <summary>
        ///     Performs zigzag on an 8x8 block
        /// </summary>
        /// <param name="input">8x8 block</param>
        /// <returns></returns>
        public static int[] Run(double[,] input) {
            int[] ret = new int[64];
            int choice = 0;
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    ret[Zagorder[choice++]] = (int) Math.Round(input[j, i]);
                }
            }
            return ret;
        }

        /// <summary>
        ///     undoes the zigzag
        /// </summary>
        /// <param name="input">8x8 block</param>
        /// <returns></returns>
        public static double[,] Inverse(int[] input) {
            double[,] ret = new double[8, 8];
            int choice = 0;
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    ret[j, i] = input[Zagorder[choice++]];
                }
            }
            return ret;
        }
    }
}