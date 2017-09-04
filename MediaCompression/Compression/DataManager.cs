using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Cyotek.Windows.Forms;

namespace Compression {
    /// <summary>
    ///     Data manager class is used to house all of the data and functions related to the data from this class.
    /// </summary>
    public class DataManager {
        private bool _imageIsSplit; // prevents unneeded processor usage

        private bool _subSampled420; // boolean to prevent extra subsampling

        public sbyte[] EncodedBytes { get; set; } // bytes as they are encoded by the program

        public List<Tuple<Point, Point>> MVs { set; get; } // Motion vectors as they are pulled out of a file.

        public Bitmap UncutBitmap { get; set; } //sometimes need one that is old size

        public List<List<float>> LumList { get; set; } = new List<List<float>>(); // list of Luma values
        public List<List<float>> CrList { get; set; } = new List<List<float>>(); // list of CR values
        public List<List<float>> CbList { get; set; } = new List<List<float>>(); // list of CB values
        private List<Color> ColorList { get; set; } = new List<Color>();
        //list of color objects (image without formatting)
        public sbyte[] LoadedData { get; set; } //data loaded from a file
        public Bitmap LeftImageBitmap { get; set; } // bitmap to be placed in the left slot
        public Bitmap RightImageBitmap { get; private set; } //bitmap to be placed in the right slow
        public int ImageSize { get; set; } // image size 
        public sbyte[] ImageWidth { get; set; } // byte format of current width
        public sbyte[] ImageHeight { get; set; } //byte format of current height
        public int RealWidth { get; set; } // real width of image
        public int RealHeight { get; set; } // Real height of image

        public ImageBox box { get; set; }
        public ImageBox rbox { get; set; }

        /// <summary>
        ///     Sets the left picture to the image passed in.
        ///     Clears ColorList and Chrome list.
        ///     Sets the bitmap representing the right image
        ///     to be the same as the left image.
        /// </summary>
        /// <param name="i"></param>
        public void SetLeftPicture(Image i) {
            ColorList = new List<Color>();

            // ChromeList = new List<List<Ycbcr>>();
            LumList = new List<List<float>>();
            CrList = new List<List<float>>();
            CbList = new List<List<float>>();

            ImageHeight = System.BitConverter.GetBytes(i.Height).Select(b => (sbyte)b).ToArray();
            ImageWidth = System.BitConverter.GetBytes(i.Width).Select(b => (sbyte)b).ToArray();

            LeftImageBitmap = new Bitmap(Padder.PadImage(i));
            RightImageBitmap = new Bitmap(LeftImageBitmap);
            _subSampled420 = false;
            _imageIsSplit = false;
        }

        /// <summary>
        ///     converts Color to RGB
        /// </summary>
        /// <param name="c">The color to convert.</param>
        /// <returns></returns>
        private static Rgb ToRgb(Color c) {
            return new Rgb(c);
        }


        /// <summary>
        ///     Loops through all the pixels and converts them to ycbcr
        /// </summary>
        public void SplitBytesIntoColorSpaces() {
            if (_imageIsSplit) return;
            for (int x = 0; x < LeftImageBitmap.Width; x++) {
                //var innerlist = new List<Ycbcr>();
                List<float> innerY = new List<float>();
                List<float> innerCr = new List<float>();
                List<float> innerCb = new List<float>();
                for (int y = 0; y < LeftImageBitmap.Height; y++) {
                    Rgb color = ToRgb(LeftImageBitmap.GetPixel(x, y));
                    //innerlist.Add(RgbtoYCbCr(color));
                    innerY.Add(RgbtoYCbCrWorking(color).Y);
                    innerCr.Add(RgbtoYCbCrWorking(color).Cr);
                    innerCb.Add(RgbtoYCbCrWorking(color).Cb);
                }
                //ChromeList.Add(innerlist);
                LumList.Add(innerY);
                CrList.Add(innerCb);
                CbList.Add(innerCr);
            }
            _imageIsSplit = true;
        }

        /// <summary>
        ///     Converts a the Ycbcr array to rgb
        ///     based on the type passed in.
        ///     Performs up-sampling
        /// </summary>
        /// <param name="type">
        ///     0: Brightness
        ///     1: Chroma red
        ///     2: Chroma blue
        ///     4: Full
        /// </param>
        private void CombineYcbcrToRgb(int type) {
            ColorList = new List<Color>();
            List<List<float>> newcr = new List<List<float>>(CrList.Count*2);
            List<List<float>> newcb = new List<List<float>>(CbList.Count*2);

            foreach (List<float> t in CrList) {
                List<float> row = new List<float>();
                foreach (float t1 in t) {
                    row.Add(t1);
                    row.Add(t1);
                }
                newcr.Add(row);
                newcr.Add(row);
            }

            foreach (List<float> t in CbList) {
                List<float> row = new List<float>();
                foreach (float t1 in t) {
                    row.Add(t1);
                    row.Add(t1);
                }
                newcb.Add(row);
                newcb.Add(row);
            }

            for (int x = 0; x < newcr.Count; x ++) {
                for (int y = 0; y < newcr[x].Count; y++) {
                    ColorList.Add(
                        YcbcrToRgbWorking(new Ycbcr(LumList[x][y], newcb[x][y], newcr[x][y]), type).ToColor());
                }
            }
        }

        /// <summary>
        ///     Loops through all of the pixels in a bitmap and sets them to the
        ///     converted ycbcr values.
        /// </summary>
        private void ColorListToRightBitmap() {
            int i = 0;

            RightImageBitmap = new Bitmap(LeftImageBitmap);

            for (int x = 0; x < LeftImageBitmap.Width; x ++) {
                for (int y = 0; y < LeftImageBitmap.Height; y ++) {
                    if (i < ColorList.Count)
                        RightImageBitmap.SetPixel(x, y, ColorList[i]);
                    i++;
                }
            }
        }

        /// <summary>
        ///     Puts data from ColorList array onto the LeftImageBitmap
        /// </summary>
        private void ColorListToLeftBitmap() {
            int i = 0;

            for (int x = 0; x < LeftImageBitmap.Width; x ++) {
                for (int y = 0; y < LeftImageBitmap.Height; y ++) {
                    if (i < ColorList.Count)
                        LeftImageBitmap.SetPixel(x, y, ColorList[i]);
                    i++;
                }
            }
        }


        /// <summary>
        ///     4:2:0 subsampling
        /// </summary>
        private void SubSample420() {
            List<List<float>> tempCrArray = new List<List<float>>(CrList.Count/2);
            List<List<float>> tempCbArray = new List<List<float>>(CbList.Count/2);

            for (int x = 0; x < CrList.Count/2; x++) {
                List<float> rowCr = new List<float>();
                List<float> rowCb = new List<float>();
                for (int y = 0; y < CrList.Count/2; y++) {
                    rowCb.Add(0);
                    rowCr.Add(0);
                }
                tempCrArray.Add(rowCr);
                tempCbArray.Add(rowCb);
            }


            for (int x = 0, x2 = 0; x < CrList.Count; x += 2, x2 ++) {
                if (x2 >= tempCrArray.Count) continue;
                List<float> crrow = tempCrArray[x2];
                List<float> cbrow = tempCbArray[x2];
                for (int y = 0, y2 = 0; y < CrList[x].Count; y += 2, y2++) {
                    if (y2 >= crrow.Count) continue;
                    crrow[y2] = CrList[x][y];
                    cbrow[y2] = CbList[x][y];
                }
            }

            CrList = new List<List<float>>(tempCbArray);
            CbList = new List<List<float>>(tempCrArray);

            _subSampled420 = true;
        }



        /// <summary>
        ///     method called from the outside to set the right bitmap
        ///     as the converted rgb values of the ycbcr
        /// </summary>
        /// <param name="choice"> choice for CombineYcbcrToRgb(int)</param>
        public void DrawYcbcrOnRightBitmap(int choice) {
            if (!_subSampled420)
                SubSample420();
            CombineYcbcrToRgb(choice);
            ColorListToRightBitmap();
        }

        /// <summary>
        ///     Displays data that we get from the compressed file.
        /// </summary>
        public void DisplayDecompressedData() {
            LeftImageBitmap = new Bitmap(ImageSize, ImageSize);
            CombineYcbcrToRgb(4);
            ColorListToLeftBitmap();
            UncutBitmap = new Bitmap(LeftImageBitmap);
            LoadedData = null;
            LumList = null;
            CrList = null;
            CbList = null;
            ColorList = null;
            LeftImageBitmap = Padder.CropImage(LeftImageBitmap, new Rectangle(0, 0, RealWidth, RealHeight));
        }


        /// <summary>
        ///     The One conversion from RGB to YCBCR that works.
        /// </summary>
        /// <param name="rgb">The RGB value to transform</param>
        /// <returns></returns>
        private static Ycbcr RgbtoYCbCrWorking(Rgb rgb) {
            float y = (byte) (16 + 0.257*rgb.R + 0.504*rgb.G + 0.0988*rgb.B);
            float cb = (byte) (128 - 0.148*rgb.R - 0.2916*rgb.G + 0.4398*rgb.B);
            float cr = (byte) (128 + 0.439*rgb.R - 0.368*rgb.G - 0.0718*rgb.B);

            return new Ycbcr(y, cb, cr);
        }


        /// <summary>
        ///     The Working version of YCBCR to RGB conversion.
        ///     Transforms one value of YCBCR to RGB
        /// </summary>
        /// <param name="ycbcr">FROM</param>
        /// <param name="choice">TO</param>
        /// <returns></returns>
        private static Rgb YcbcrToRgbWorking(Ycbcr ycbcr, int choice) {
            int r = (int) (1.164*(ycbcr.Y - 16) + 1.596*(ycbcr.Cr - 128));
            int g = (int) (1.164*(ycbcr.Y - 16) - 0.813*(ycbcr.Cr - 128) - 0.392*(ycbcr.Cb - 128));
            int b = (int) (1.164*(ycbcr.Y - 16) + 2.017*(ycbcr.Cb - 128));

            r = Math.Max(0, Math.Min(255, r));
            g = Math.Max(0, Math.Min(255, g));
            b = Math.Max(0, Math.Min(255, b));
            return new Rgb((byte) r, (byte) g, (byte) b);
        }
    }
}