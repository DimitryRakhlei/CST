using System;
using System.Collections.Generic;
using System.Text;

namespace asgn5v1 {
    class Matrix {
        public double[,] Identity4X4() {
            double[,] output = {
                {1, 0, 0, 0},
                {0, 1, 0, 0},
                {0, 0, 1, 0},
                {0, 0, 0, 1}
            };

            return output;
        }

        public double[,] ShearXYZ(double amount, int type) {
            double[,] output = Identity4X4();

            switch (type) {
                case 1:
                    output[1, 0] = amount;
                    break;
                case 2:

                    break;
                case 3:

                    break;
            }

            return output;
        }
      
        public double[,] RotationXYZ(double angle, int type) {
            double[,] identity = Identity4X4();
            if (type == 3) { //z
                identity[0, 0] = (float) Math.Cos(angle);
                identity[0, 1] = (float) -Math.Sin(angle);
                identity[1, 0] = (float) Math.Sin(angle);
                identity[1, 1] = (float) Math.Cos(angle);
            }else if (type == 1) { //x
                identity[1, 1] = (float)Math.Cos(angle);
                identity[1, 2] = (float)-Math.Sin(angle);
                identity[2, 1] = (float)Math.Sin(angle);
                identity[2, 2] = (float)Math.Cos(angle);
            }else if (type == 2) { //y
                identity[0, 0] = (float)Math.Cos(angle);
                identity[0, 2] = (float)-Math.Sin(angle);
                identity[2, 0] = (float)Math.Sin(angle);
                identity[2, 2] = (float)Math.Cos(angle);
            }else if(type == -3) { //z
                identity[0, 0] = (float)Math.Cos(angle);
                identity[0, 1] = (float)Math.Sin(angle);
                identity[1, 0] = (float)-Math.Sin(angle);
                identity[1, 1] = (float)Math.Cos(angle);
            } else if(type == -1) { //x
                identity[1, 1] = (float)Math.Cos(angle);
                identity[1, 2] = (float)Math.Sin(angle);
                identity[2, 1] = (float)-Math.Sin(angle);
                identity[2, 2] = (float)Math.Cos(angle);
            } else if(type == -2) { //y
                identity[0, 0] = (float)Math.Cos(angle);
                identity[0, 2] = (float)Math.Sin(angle);
                identity[2, 0] = (float)-Math.Sin(angle);
                identity[2, 2] = (float)Math.Cos(angle);
            }
            return identity;
        }

        public double[,] Translation(double x, double y, double z) {
            double[,] identity = Identity4X4();
            identity[3, 0] = x;
            identity[3, 1] = y;
            identity[3, 2] = z;

            return identity;
        }

        public double[,] Scaling(double x, double y, double z) {
            double[,] identity = Identity4X4();

            identity[0, 0] = x;
            identity[1, 1] = y;
            identity[2, 2] = z;


            return identity;
        }

        public double[,] Invert(double x, double y, double z) {
            double[,] identity = Identity4X4();

            identity[0, 0] = x;
            identity[1, 1] = y;
            identity[2, 2] = z;

            return identity;
        }

    }
}
