using System;
using System.Collections.Generic;

namespace Compression {
    internal static class MatrixExtensions {
        /// <summary>
        ///     Creates a chunk of a matrix as specified.
        /// </summary>
        /// <typeparam name="T">Type to be converted</typeparam>
        /// <param name="inputMatrix">matrix to be chunked</param>
        /// <param name="chunkWidth">how wide will the chunk be</param>
        /// <param name="chunkHeight">how high will the chunk be</param>
        /// <returns>returns the chunk created</returns>
        public static IEnumerable<T[,]> ChunkMatrix<T>(this T[,] inputMatrix, int chunkWidth, int chunkHeight) {
            int inputWidth = inputMatrix.GetLength(0);
            int inputHeight = inputMatrix.GetLength(1);

            for (int i = 0; i < inputWidth; i += chunkWidth) {
                for (int j = 0; j < inputHeight; j += chunkHeight) {
                    T[,] chunk = new T[chunkWidth, chunkHeight];
                    for (int k = 0; k < chunkWidth; k++) {
                        int sourceIndex = i*inputWidth + k*inputWidth + j;
                        int destinationIndex = k*chunkHeight;
                        Array.Copy(inputMatrix, sourceIndex, chunk, destinationIndex, chunkHeight);
                    }
                    yield return chunk;
                }
            }
        }
    }
}