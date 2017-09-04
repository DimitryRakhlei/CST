using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixedProject {
    class DFT {
        
        public float[] ParallelDft(float[] data, ref float[] real, ref float[] imag) {
            int n = data.Length;
            int m = n ;
            float [] treal = new float[n];
            float [] timag = new float[n];
            float[] result = new float[m];

            float pi_div = (float)(2.0 * Math.PI / n);
            Parallel.For(0, m,
              w => {
                  float a = w * pi_div;
                  for (int t = 0; t < n; t++) {
                      treal[w] += (float)(data[t] * Math.Cos(a * t));
                      timag[w] += (float)(data[t] * Math.Sin(a * t));
                  }
                  result[w] = (float)Math.Sqrt(treal[w] * treal[w] + timag[w] * timag[w]) / n;
              }
            );
            real = new float[treal.Length];
            imag = new float[timag.Length];
            Array.Copy(treal, real, treal.Length);
            Array.Copy(timag, imag, imag.Length);
            return result;
        }

        public float[] dft(float[] data) {
            int n = data.Length;
            int m = n;// I use m = n / 2d;
            float[] real = new float[n];
            float[] imag = new float[n];
            float[] result = new float[m];
            float pi_div = (float)(2.0 * Math.PI / n);

            for (int w = 0; w < m; w++) {
                float a = w * pi_div;
                for (int t = 0; t < n; t++) {

                    real[w] += (float)(data[t] * Math.Cos(a * t));
                    imag[w] += (float)(data[t] * Math.Sin(a * t));

                }
                result[w] = (float)(Math.Sqrt(real[w] * real[w] + imag[w] * imag[w]) / n);
            }

            return result;
        }


        public float[] idft(float[] cos, float[] sin, int len = 0) {
            if (cos.Length != sin.Length) throw new ArgumentException("cos.Length and sin.Length bust match!");

            if (len == 0)
                len = (cos.Length - 1) * 2;

            float[] output = new float[len];

            int partials = sin.Length;
            if (partials > len / 2)
                partials = len / 2;
            Parallel.For(0, partials,
             n => {
                 for (int i = 0; i < len; i++) {
                     output[i] += (float)Math.Cos(2 * Math.PI * n / len * i) * cos[n];
                     output[i] += (float)Math.Sin(2 * Math.PI * n / len * i) * sin[n];
                 }
             });
            return output;
        }


    }
}
