using System.Collections.Generic;

namespace Compression {
    internal static class SquareIsolator {
        /// <summary>
        ///     Creates a square out of a data souce
        /// </summary>
        /// <typeparam name="T">what to chunk</typeparam>
        /// <param name="data">what to chunk</param>
        /// <param name="x">where to chunk x</param>
        /// <param name="y">where to chunk y</param>
        /// <param name="block">how much to chunk</param>
        /// <returns>chunk</returns>
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
    }
}