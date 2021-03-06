﻿using System.Linq;
using ILNumerics;

namespace FacialRecog {
    public class EigenObject {
        public complex[] Values { get; set; }
        public complex[,] Vectors { get; set; }

        public EigenObject(complex[,] vec, complex[] val) {
            Vectors = vec;
            Values = val;
        }

        public EigenObject(ILArray<complex> vec, ILArray<complex> val, int width) {
            Vectors = ILArray2Array(vec, width);
            Values = ILArray2Values(val, width);
        }

        public complex[,] ILArray2Array(ILArray<complex> il, int width) {
            complex[,] result = new complex[width, width];
            complex[] ila = il.ToArray();
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < width; j++) {
                    result[i, j] = ila[i*width + j];
                }
            }
            return result;
        }

        public complex[] ILArray2Values(ILArray<complex> il, int width) {
            complex[] result = new complex[width];
            complex[] ila = il.ToArray();
            for (int i = 0; i < width; i++) {
                result[i] = ila[i*width + i];
            }
            return result;
        }
    }
}