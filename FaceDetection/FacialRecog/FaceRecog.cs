using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Math.Metrics;
using AForge.Video;
using AForge.Video.DirectShow;
using FacialRecog.ViolaJones.Detection;
using ILNumerics;
using ILNumerics.Native;

namespace FacialRecog {
    public partial class FaceRecog : Form {


        private VideoCaptureDevice _capdev;
        private Counter Counter { get; set; }
        private Counter CheckFaceCounter { get; set; }

        Comparer<Size> RectangleComparer = Comparer<Size>.Create((s1, s2) =>
                            (s1.Width * s1.Height).CompareTo(s2.Width * s2.Height));

        private FrameData FData { get; }
        private bool FrameSkip { get; set; } = true;
        private bool NewFrame { get; set; }
        //private bool NSKip1 { get; set; } = false;
        private BoxData Boxdata { get; set; } = new BoxData();
        private Rectangle[] _faces;

        #region eigen variables
        private List<double> AVGCorrelations;
        private double[,] AvgSquared;
        public const double SAME_FACE_THRESH = 7.0;
        public const double FACE_THRESH = 16000;
        private const int FACES_PER_PERSON = 1;
        private double[,] avg;
        private double[] comp;
        private double[][,] difLib;
        //private int display;
        private double[][,] eigFaces;
        private double faceSpace;
        private double[][,] lib;
        private Bitmap libBmp;
        private double[][] libWeights;
        private Bitmap mainBmp;
        private double[,] recon;
        private int LibCount;
        #endregion


        readonly HaarObjectDetector _faceDetector =
            new HaarObjectDetector(new ViolaJones.Detection.HaarCascade.Def.FaceHaarCascade(), 50);


        /// <summary>
        /// entey point to form
        /// </summary>
        public FaceRecog() {
            InitializeComponent();

            foreach (FilterInfo videoCaptureDevice in InfoCollection) {
                streamDeviceChoiceBox.Items.Add(videoCaptureDevice.Name);
            }
            streamDeviceChoiceBox.SelectedIndex = 1;

            foreach (
                VideoCapabilities format in new VideoCaptureDevice(InfoCollection[streamDeviceChoiceBox.SelectedIndex]
                    .MonikerString).VideoCapabilities) {
                formatsBox.Items.Add(format.FrameSize + " : " + format.AverageFrameRate + "FPS");
            }
            formatsBox.SelectedIndex = 1;

            FData = new FrameData();

            Counter = new Counter(0, 20);
            CheckFaceCounter = new Counter(0, 10);

            LibCount = LoadLibrary(@"../../ImageLib/cfol/",
                512, 512, FACES_PER_PERSON);

            avg = ImageTool.GetAvg(lib);

            difLib = ImageTool.GetDifferenceArray(lib, avg);

            EigenObject eigVects = ImageTool.GetEigen(ImageTool.GetA(lib));

            ImageTool.normalize(eigVects.Vectors);

            eigFaces = ImageTool.getEigenFaces(eigVects.Vectors, difLib);

            libWeights = new double[lib.Length][];

            for (int i = 0; i < lib.Length; i++) {
                libWeights[i] = ImageTool.getWeights(eigFaces, lib[i], avg);
                // test - eigen then Sigma weight * each eigen
            }

            Image image = Image.FromFile(@"../../ImageLib/cfol/0101.bmp");
            QuickFaceDetect((Bitmap) image);
        }


     

        private static FilterInfoCollection InfoCollection => new FilterInfoCollection(FilterCategory.VideoInputDevice);

        /// <summary>
        ///     Event to record and process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventargs"></param>
        private void video_newFrame(object sender, NewFrameEventArgs eventargs) {

            FrameSkip = !FrameSkip;
            if (!FrameSkip) return;

            
            #region setting new and prev frames
            if (display.Image != null) {
                Invoke(new MethodInvoker(delegate { display.Image.Dispose(); }));
            }

            if (FData.NFrame != null) {
                lock (FData) {
                    FData.PFrame = new Bitmap(FData.NFrame);
                }
            }
            #endregion

            #region processing images (512x512) and grayscale
        

            FData.NFrame = new Bitmap(eventargs.Frame);
            
            ConvertBitmapToGray(FData.NFrame);
            double[,] tempBitmap = new Bitmap(FData.NFrame).ImageToArray();
            #endregion

            #region face detection using haar

            if (CheckFaceCounter.Inc()) {
                _faces = ProcessImage(tempBitmap);
            }
            else {
                if (_faces == null || _faces.Length < 1) return;
                Bitmap newmap = new Bitmap(tempBitmap.ImageFromArray());
                using (Graphics g = Graphics.FromImage(newmap)) {
                    g.DrawRectangles(Pens.Red, _faces);
                }

                if (CheckForFace(tempBitmap.ImageFromArray(), _faces.MaxBy(x => x.Width * x.Height))) {
                    
                }
                
                display.Image = new Bitmap(newmap);
            }

            #endregion

          
        }

        private bool CheckForFace(Bitmap input, Rectangle box) {
            bool ret =  QuickFaceDetect(input.ImageToArray().
                GetSquare(box.X, box.Y, Math.Max(box.Width, box.Height)).ImageFromArray(), 0);
            return ret;
        }

        private bool QuickFaceDetect(Bitmap input, int param) {
            Task<double[]> dat =
                Task<double[]>.Factory.StartNew(() => LookForOneFace(input.SquareIt(256).ImageToArray()));
            double[] result = dat.Result;
            if (!(result.Min() < 8)) return false;
            faceImage.Image = lib[result.ToList().IndexOf(result.Min())].ImageFromArray();
            return true;
        }

        private void QuickFaceDetect(Bitmap first) {
            Task<double[]> dat =
                Task<double[]>.Factory.StartNew(() => LookForOneFace(first.SquareIt(256).ImageToArray()));
            double[] result = dat.Result;
            if (result.Min() < 8)
                faceImage.Image = lib[result.ToList().IndexOf(result.Min())].ImageFromArray();
        }


        double[] LookForOneFace(double[,] data) {
            double[,] img = data.RotateImage();
            closestFace.Image = img.ImageFromArray();
            double[] weights = ImageTool.getWeights(eigFaces, img, avg);
            double[] cmp = ImageTool.compareWeigths(libWeights, weights);

            return cmp;
        }


        /// <summary>
        /// Sets pixels in bitmap to gray, async.
        /// </summary>
        /// <param name="input"></param>
        private void ConvertBitmapToGray(Bitmap input) {
            unsafe {
                BitmapData bitmapData = input.LockBits(new Rectangle(0, 0, input.Width, input.Height),
                    ImageLockMode.ReadWrite, input.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(input.PixelFormat)/8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width*bytesPerPixel;
                byte* ptrFirstPixel = (byte*) bitmapData.Scan0;

                Parallel.For(0, heightInPixels, y => {
                    byte* currentLine = ptrFirstPixel + y*bitmapData.Stride;
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel) {
                        float oldBlue = currentLine[x + 1];
                        float oldGreen = currentLine[x + 2];
                        float oldRed = currentLine[x + 3];

                        byte value = (byte) Math.Round((double) (0.299*oldRed + 0.587*oldGreen + 0.114*oldBlue));

                        currentLine[x + 3] = 255;
                        currentLine[x + 1] = value;
                        currentLine[x + 2] = value;
                        currentLine[x] = value;
                    }
                });
                input.UnlockBits(bitmapData);
            }
        }


        /// <summary>
        ///     Stop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e) {
            if (_capdev == null) return;
            if (_capdev.IsRunning)
                _capdev.SignalToStop();
        }

        /// <summary>
        ///     Start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e) {
            _capdev = new VideoCaptureDevice(InfoCollection[streamDeviceChoiceBox.SelectedIndex].MonikerString);
            _capdev.VideoResolution = _capdev.VideoCapabilities[formatsBox.SelectedIndex];
            _capdev.NewFrame += video_newFrame;
            _capdev.Start();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private static List<List<double>> Differences(Bitmap first, Bitmap second) {
            List<List<double>> output = new List<List<double>>();
            for (int h = 0; h < first.Width; h++) {
                List<double> row = new List<double>();
                for (int w = 0; w < first.Height; w++) {
                    row.Add(0);
                }
                output.Add(row);
            }
            unsafe {
                //first
                BitmapData bitmapData =
                    first.LockBits(new Rectangle(0, 0, first.Width, first.Height),
                        ImageLockMode.ReadWrite, first.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(first.PixelFormat)/8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width*bytesPerPixel;
                byte* ptrFirstPixel = (byte*) bitmapData.Scan0;


                //second
                BitmapData bitmapData2 =
                    second.LockBits(new Rectangle(0, 0, second.Width, second.Height),
                        ImageLockMode.ReadWrite, second.PixelFormat);

                int bytesPerPixel2 = Image.GetPixelFormatSize(second.PixelFormat)/8;
                int heightInPixels2 = bitmapData2.Height;
                int widthInBytes2 = bitmapData2.Width*bytesPerPixel2;
                byte* ptrFirstPixel2 = (byte*) bitmapData2.Scan0;


                Parallel.For(0, heightInPixels, y => {
                    byte* currentLine = ptrFirstPixel + y*bitmapData.Stride;
                    byte* currentLine2 = ptrFirstPixel2 + y*bitmapData2.Stride;
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel) {
                        output[x/bytesPerPixel][y] = currentLine[x + 1] - currentLine2[x + 1];
                    }
                });
                first.UnlockBits(bitmapData);
                second.UnlockBits(bitmapData2);
            }

            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="size"></param>
        private Tuple<Bitmap, bool> FindDifferences(Image first, Image second, int size) {
            Bitmap C = new Bitmap(first);
            Bitmap R = new Bitmap(second);
            BoxData tbd = new BoxData(Boxdata);

            List<List<double>> diffs = Differences(C, R);
            Bitmap output = new Bitmap(size, size);

            unsafe {
                BitmapData bitmapData =
                    output.LockBits(new Rectangle(0, 0, output.Width, output.Height),
                        ImageLockMode.ReadWrite, output.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(output.PixelFormat)/8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width*bytesPerPixel;
                byte* ptrFirstPixel = (byte*) bitmapData.Scan0;

                Boxdata = new BoxData();

                Parallel.For(0, heightInPixels, y => {
                    byte* currentLine = ptrFirstPixel + y*bitmapData.Stride;
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel) {
                        int value = (int) Math.Round(diffs[x/4][y]);
                        value = value > 10 ? 255 : 0;
                        //diffs[x/4][y] = value;

                        currentLine[x + 3] = 255;
                        currentLine[x + 1] = (byte) value;
                        currentLine[x + 2] = (byte) value;
                        currentLine[x] = (byte) value;

                        if (value != 255) continue;

                        if (x/4 < Boxdata.LeftMost)
                            Boxdata.LeftMost = x/4;
                        else if (x/4 > Boxdata.Rightmost)
                            Boxdata.Rightmost = x/4;

                        if (y < Boxdata.Highest)
                            Boxdata.Highest = y;
                        else if (y > Boxdata.Lowest)
                            Boxdata.Lowest = y;
                    }
                });

                Boxdata.MakeSquarePowerOfTwo(16);

                output.UnlockBits(bitmapData);
            }

            bool found = IsolateFaces(first, diffs);

            

            return new Tuple<Bitmap, bool>(output, found);
        }


        /// <summary>
        ///  Performs a search on squares of data to isolate objects
        /// </summary>
        /// <param name="first"></param>
        /// <param name="diffs"></param>
        /// <returns></returns>
        private bool IsolateFaces(Image first, List<List<double>> diffs) {
            /*double[,] searchBlock = diffs.CreateRectangularArray();
            List<double[,]> data = new List<double[,]>();
            List<double[]> weightList = new List<double[]>();

            for (int x = 0; x < searchBlock.GetLength(0); x += 256) {
                for (int y = 0; y < searchBlock.GetLength(1); y += 256) {
                    double[,] block = searchBlock.GetSquare(x, y, 256);
                    data.Add(block);
                }
            }

            foreach (Task<double[]> task in data.Select(block => new Task<double[]>(() => {
                double[] weights = ImageTool.getWeights(eigFaces, block, avg);
                double[] temp = ImageTool.compareWeigths(libWeights, weights);
                return temp;
            }))) {
                task.Start();
                weightList.Add(comp = task.Result);
            }

            List<double> avgs = weightList.Select(w => w.Min()).ToList();*/


            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="subSet"></param>
        /// <returns></returns>
        private int LoadLibrary(string directory, int width, int height, int subSet) {
            string[] images = Directory.GetFiles(@directory, "*.bmp");


            if (subSet < 1)
                subSet = 1;
            lib = new double[images.Length][,];
            int i = 0;
            foreach (string image in images) {
                lib[i++] = ImageTool.GetArray(new Bitmap(image));
            }
            if (subSet > 1)
                lib = ImageTool.avgSubsets(lib, subSet);
            return images.Length/subSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FaceRecog_FormClosing(object sender, FormClosingEventArgs e) {
            Environment.Exit(Environment.ExitCode);
        }


        private void ProcessUsingLockbitsAndUnsafeAndParallel(Bitmap processedBitmap) {
            unsafe {
                BitmapData bitmapData =
                    processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height),
                        ImageLockMode.ReadWrite, processedBitmap.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(processedBitmap.PixelFormat)/8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width*bytesPerPixel;
                byte* ptrFirstPixel = (byte*) bitmapData.Scan0;

                Parallel.For(0, heightInPixels, y => {
                    byte* currentLine = ptrFirstPixel + y*bitmapData.Stride;
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel) {
                        int oldBlue = currentLine[x];
                        int oldGreen = currentLine[x + 1];
                        int oldRed = currentLine[x + 2];

                        currentLine[x] = (byte) oldBlue;
                        currentLine[x + 1] = (byte) oldGreen;
                        currentLine[x + 2] = (byte) oldRed;
                    }
                });
                processedBitmap.UnlockBits(bitmapData);
            }
        }

        /// <summary>
        /// Process an image and obtain haar features
        /// </summary>
        /// <param name="input">Image to have face detection performed on.</param>
        /// <returns>Rectangle[] which represents all detected faces</returns>
        private Rectangle[] ProcessImage(double[,] input) {
            try {
                Task<Rectangle[]> getFaces = Task.Run(() => _faceDetector.ProcessFrame(input));
                return getFaces.Result;
            }
            catch (Exception e) {
                // ignored
            }
            return null;
        }

        private int snapcount = 0;
        private void SnapShotButton_Click(object sender, EventArgs e) {
            FData.NFrame?.Save(Application.ExecutablePath + snapcount++ + ".bmp");
        }
    }


    /*    /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <returns></returns>
        private bool IsolateFaces(Image first) {
            double[,] img = null;
            bool output = false;
            if (!Counter.Inc()) return false;
            Bitmap facemap = null;
            Bitmap tempfirst = new Bitmap(first);
            try {
                Bitmap dest = new Bitmap(
                    256,
                    256
                    );

                img = new double[dest.Width, dest.Height];
                unsafe
                {
                    BitmapData sourceBitData = tempfirst.LockBits(
                        new Rectangle(0, 0, tempfirst.Width, tempfirst.Height),
                        ImageLockMode.ReadWrite,
                        tempfirst.PixelFormat);
                    BitmapData destBitData = dest.LockBits(
                        new Rectangle(0, 0, dest.Width, dest.Height),
                        ImageLockMode.ReadWrite,
                        dest.PixelFormat
                        );

                    int BPP = Image.GetPixelFormatSize(tempfirst.PixelFormat) / 8;

                    int bboxWidth = Boxdata.Rightmost - Boxdata.LeftMost;

                    Point destPos = new Point(Boxdata.LeftMost + bboxWidth / 3, Boxdata.Highest);

                    byte* ptrSrc = (byte*)sourceBitData.Scan0;
                    byte* ptrDest = (byte*)destBitData.Scan0;

                    Parallel.For(0, destBitData.Height, y => {
                        byte* srcLine = ptrSrc + (y + destPos.Y + 50) * sourceBitData.Stride;
                        byte* destLine = ptrDest + y * destBitData.Stride;
                        for (int x = 0; x < destBitData.Width * BPP; x += BPP) {
                            destLine[x] = srcLine[x + destPos.X * BPP];
                            destLine[x + 1] = srcLine[x + destPos.X * BPP + 1];
                            destLine[x + 2] = srcLine[x + destPos.X * BPP + 2];
                            destLine[x + 3] = srcLine[x + destPos.X * BPP + 3];

                            img[x / BPP, y] = destLine[x];
                        }
                    });

                    tempfirst.UnlockBits(sourceBitData);
                    dest.UnlockBits(destBitData);
                    facemap = new Bitmap(dest);

                    Task<double[]> task = new Task<double[]>(() => {
                        img = img.Upsize();
                        double[] weights = ImageTool.getWeights(eigFaces, img, avg);
                        double[] temp = ImageTool.compareWeigths(libWeights, weights);
                        return temp;
                    });
                    task.Start();
                    comp = task.Result;
                    output = comp.Min() < 8;

                    if (faceImage.InvokeRequired) {
                        faceImage.Invoke(new MethodInvoker(
                            delegate () { faceImage.Image = img.ImageFromArray(); }));
                    } else {
                        faceImage.Image = img.ImageFromArray();
                    }

                    if (output)
                        if (closestFace.InvokeRequired) {
                            closestFace.Invoke(new MethodInvoker(
                                delegate () { closestFace.Image = lib[comp.ToList().IndexOf(comp.Min())].ImageFromArray(); }));
                        } else {
                            closestFace.Image = lib[comp.ToList().IndexOf(comp.Min())].ImageFromArray();
                        }
                }
            } catch (ArgumentException) {
            }

            return output;
        }*/


    class DataBlock {
        public Point Pos { get; set; }
        public double[,] Block { get; set; }
        public int Size { get; set; }

        public DataBlock(Point p, double[,] b, int s) {
            Pos = p;
            Block = b;
            Size = s;
        }
    }
}
