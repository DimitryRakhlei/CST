using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression.Tests {
    static class Tester {
        private static double[,] testdata = new double[,] {
            {1,2,3,4,5,6,7,8 },
            {2,2,3,4,5,6,7,8 },
            {3,3,3,4,5,6,7,8 },
            {4,4,4,4,5,6,7,8 },
            {5,5,5,5,5,6,7,8 },
            {6,6,6,6,6,6,7,8 },
            {7,7,7,7,7,7,7,8 },
            {8,8,8,8,8,8,8,8 }
        };

        private static double[,] bigtestdata = new double[,] {
            {  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16 },
            {  2,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16 },
            {  3,  3,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16 },
            {  4,  4,  4,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16 },
            {  5,  5,  5,  5,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16 },
            {  6,  6,  6,  6,  6,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16 },
            {  7,  7,  7,  7,  7,  7,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16 },
            {  8,  8,  8,  8,  8,  8,  8,  8,  9, 10, 11, 12, 13, 14, 15, 16 },
            {  9,  9,  9,  9,  9,  9,  9,  9,  9, 10, 11, 12, 13, 14, 15, 16 },
            { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 11, 12, 13, 14, 15, 16 },
            { 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 12, 13, 14, 15, 16 },
            { 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 13, 14, 15, 16 },
            { 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 14, 15, 16 },
            { 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 15, 16 },
            { 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 16 },
            { 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16}
        };

        public static void print<T>(T input) {
            Console.WriteLine(input);
        }

        public static void PrintArray(this double[,] input) {
            for (int i = 0; i < input.GetLength(0); i++) {
                for (int j = 0; j < input.GetLength(1); j++) {
                    Console.Write($"{Math.Round(input[j,i])} ");
                }
                Console.WriteLine();
            }
        }
        
        public static void TestZigZag() {
            print("Zigzag Test");
            testdata.PrintArray();
            print("");
            print("Zigzag reverse");
            ZigZag.Inverse(ZigZag.Run(testdata)).PrintArray();
        }
        
        public static void TestDCT_Quantize() {
            Dct dct = new Dct();
            print("DCT Test");
            var d = dct.Go(testdata);
            print("Quantized dct");
            Quantizer.QuantizeLuminance(ref d);
            d.PrintArray();
            print("Quantized Inverse");
            Quantizer.InverseQuantizeLuminance(ref d);
            print("Dct Inverse");
            dct.GoBack(d).PrintArray();
        }

        public static void TestWhole() {
            // down sample
            List<List<double>> data = new List<List<double>>();
            for (int i = 0; i < bigtestdata.GetLength(0); i++) {
                if (i%2 != 0) continue;
                List<double> row = new List<double>();
                for (int j = 0; j < bigtestdata.GetLength(1); j++) {
                    if (j%2 != 0) continue;
                    row.Add(bigtestdata[i, j]);
                }
                data.Add(row);
            }
            double[,] narr = new double[8, 8];
            for (int i = 0; i < data.Count; i++) {
                for (int j = 0; j < data[i].Count; j++) {
                    narr[i, j] = data[i][j];
                }
            }
            print("Down Sampled");
            narr.PrintArray();

            Dct dct = new Dct();
            var dctdata = dct.Go(narr);
            print("dct go");
            dctdata.PrintArray();

            Quantizer.QuantizeLuminance(ref dctdata);
            print("quantized");
            dctdata.PrintArray();

            var zagged = ZigZag.Run(dctdata);
            var bzagged = zagged.Select(x => (sbyte) x).ToArray();
            List<sbyte[]> zaglist = new List<sbyte[]>() {bzagged};
            sbyte[] rled = Rle.BetterEncode(zaglist);
            
            // reversing
            var unrle = Rle.DecodeRle(rled);
            var unzag = ZigZag.Inverse(unrle[0].Select(x => (int)x).ToArray());
            Quantizer.InverseQuantizeLuminance(ref unzag);
            var idct = dct.GoBack(unzag);
            idct.PrintArray();
        }
    }
}
