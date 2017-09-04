using System.Drawing;

namespace Compression {
    /// <summary>
    ///     Simply the RGB class wrapper
    /// </summary>
    public class Rgb {
        public Rgb(Color c) {
            R = c.R;
            G = c.G;
            B = c.B;
        }

        public Rgb(byte r, byte g, byte b) {
            R = r;
            G = g;
            B = b;
        }

        public byte R { get; private set; }

        public byte G { get; private set; }

        public byte B { get; private set; }

        public bool Equals(Rgb rgb) {
            return (R == rgb.R) && (G == rgb.G) && (B == rgb.B);
        }

        public Color ToColor() {
            return Color.FromArgb(R, G, B);
        }
    }
}