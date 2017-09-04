using System;

namespace Compression {
    /// <summary>
    ///     simple ycbcr wrapper class
    /// </summary>
    public class Ycbcr {
        public Ycbcr(float y, float cb, float cr) {
            Y = y;
            Cb = cb;
            Cr = cr;
        }

        public float Y { get; set; }

        public float Cb { get; set; }

        public float Cr { get; set; }

        public bool Equals(Ycbcr ycbcr) {
            return (Math.Abs(Y - ycbcr.Y) < 0.01) && (Math.Abs(Cb - ycbcr.Cb) < 0.01) &&
                   (Math.Abs(Cr - ycbcr.Cr) < 0.01);
        }
    }
}