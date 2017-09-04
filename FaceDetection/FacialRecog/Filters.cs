using System.Drawing;
using AForge.Imaging.Filters;

namespace FacialRecog {
    public static class Filters {
        public static Bitmap ToGray(this Bitmap input) {
            Grayscale gray = new Grayscale(0.2125, 0.7154, 0.0721);
            return new Bitmap(gray.Apply(input));
        }

        public static Bitmap SquareIt(this Bitmap input, int sz) {
            ResizeBilinear square = new ResizeBilinear(sz, sz);
            return new Bitmap(square.Apply(input));
        }
    }
}