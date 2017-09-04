namespace Compression {
    public class Quantizer {
        /// <summary>
        ///     chrominance quantization
        /// </summary>
        private static double[,] ChrominanceQuantizationMatrix { get; } = {
            {17, 18, 24, 47, 99, 99, 99, 99},
            {18, 21, 26, 66, 99, 99, 99, 99},
            {24, 26, 56, 99, 99, 99, 99, 99},
            {47, 66, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99},
            {99, 99, 99, 99, 99, 99, 99, 99}
        };

        /// <summary>
        ///     luminance quantization matrix
        /// </summary>
        private static double[,] LuminanceQuantizationMatrix { get; } = {
            {16, 11, 10, 16, 24, 40, 51, 61},
            {12, 12, 14, 19, 26, 58, 60, 55},
            {14, 13, 16, 24, 40, 57, 69, 56},
            {14, 17, 22, 29, 51, 87, 80, 62},
            {18, 22, 37, 56, 68, 109, 103, 77},
            {24, 35, 55, 64, 81, 104, 113, 92},
            {49, 64, 78, 87, 103, 121, 120, 101},
            {72, 92, 95, 98, 112, 100, 103, 99}
        };

        /// <summary>
        ///     quantizes luminance
        /// </summary>
        /// <param name="data">chunk to be quantized</param>
        public static void QuantizeLuminance(ref double[,] data) {
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    data[i, j] /= LuminanceQuantizationMatrix[i, j];
                }
            }
        }

        /// <summary>
        ///     quantizes chrominance
        /// </summary>
        /// <param name="data">chunk to be quantized</param>
        public static void QuantizeChrominance(ref double[,] data) {
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    data[i, j] /= ChrominanceQuantizationMatrix[i, j];
                }
            }
        }

        /// <summary>
        ///     reverts the luminance quantization
        /// </summary>
        /// <param name="data">chunk to be iquantized</param>
        public static void InverseQuantizeLuminance(ref double[,] data) {
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    data[i, j] *= LuminanceQuantizationMatrix[i, j];
                }
            }
        }

        /// <summary>
        ///     inverses the chrominance quantization
        /// </summary>
        /// <param name="data">the chunk to be iquantized</param>
        public static void InverseQuantizeChrominance(ref double[,] data) {
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    data[i, j] *= ChrominanceQuantizationMatrix[i, j];
                }
            }
        }
    }
}