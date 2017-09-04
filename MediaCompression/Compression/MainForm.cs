using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Compression.Properties;
using Compression.Tests;

namespace Compression {
    /// <summary>
    ///     Media Compression class provides methods that will load and
    ///     modify an image.
    /// </summary>
    public partial class MediaCompression : Form {
        /// <summary>
        ///     Function that initializes the program.
        ///     loads the image into data manager as a byte array.
        /// </summary>
        public MediaCompression() {
            InitializeComponent();
            //init Data wrapper class
            MPEGData = new List<DataManager>();
            Manager = new DataManager();
            DataManager mpegManager1 = new DataManager();
            DataManager mpegManager2 = new DataManager();
            MPEGData.Add(mpegManager1);
            MPEGData.Add(mpegManager2);

            //init the split container
            splitContainer.SplitterWidth = 1;
            splitContainer.SplitterDistance = splitContainer.Width/2;

            contextMenuStrip1.Items.Add(new ToolStripButton("Compress Image"));
            contextMenuStrip2.Items.Add(new ToolStripButton("Set Image"));
            contextMenuStrip1.Items.Add(new ToolStripButton("Set Image"));
            contextMenuStrip1.Items.Add(new ToolStripButton("Clear"));
            contextMenuStrip2.Items.Add(new ToolStripButton("Clear"));
            contextMenuStrip2.Items.Add(new ToolStripButton("Open Compressed"));

            contextMenuStrip1.ItemClicked += HandleLeftContext;
            contextMenuStrip2.ItemClicked += HandleRightContext;

            // Tests
            //Tester.TestZigZag();
            //Tester.TestDCT_Quantize();
            Tester.TestWhole();
        }

        /// <summary>
        ///     Data manager that handles a ton of
        /// </summary>
        private DataManager Manager { get; set; }

        private List<DataManager> MPEGData { get; }

        private List<Tuple<Point, Point>> MVs { get; set; }

        public delegate void ChangeLeftPicture(Bitmap image);

        public delegate void ChangeRightPicture(Bitmap image);


        /// <summary>
        ///     Function which sets the size toolstrip to the passed in value.
        /// </summary>
        /// <param name="s">The size of the image passed in as a string.</param>
        private void SetSizeToolStrip(int s) {
            string ret = "" + s/1024 + " KB";
            SizeToolStrip.Text = Resources.MediaCompression_SetSizeToolStrip_Size__ + ret;
        }


        /// <summary>
        ///     Loads output.dct
        ///     initializes data
        ///     uses DctImageProcessor::UncoProcess
        ///     in a thread to display a JPEG image
        /// </summary>
        /// <param name="sender">c# params</param>
        /// <param name="e">c# params</param>
        private void openCompressedToolStripMenuItem_Click(object sender, EventArgs e) {
            if (FOpen.ShowDialog() != DialogResult.OK)
                return;
            try {
                Stream raw = FOpen.OpenFile();
                using (MemoryStream ms = new MemoryStream()) {
                    raw.CopyTo(ms);
                    Manager = new DataManager();

                    leftPicture.Image = null;
                    rightPicture.Image = null;
                    Manager.LoadedData = ms.ToArray().Select(s=>(sbyte)s).ToArray();
                    SetSizeToolStrip(Manager.LoadedData.Length);
                    Manager.box = leftPicture;
                    Manager.rbox = rightPicture;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(@"Error: Could not read file from disk. Original error: " + ex.Message);
            }

            DctImageProcessor processor = new DctImageProcessor(Manager);
            Task.Run(()=>processor.UndoProcess());
        }

        /// <summary>
        ///     Loads an image and uses
        ///     splits it into YCBCR arrays
        ///     subsamples CB and CR using 4:2:0
        ///     DctImageProcessor::Process function to
        ///     compress image with
        ///     DCT => QUantize => ZigZag => RLE => File (output.dct)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void compressToolStripMenuItem_Click(object sender, EventArgs e) {
            if (FOpen.ShowDialog() != DialogResult.OK)
                return;
            try {
                Image image = Image.FromStream(FOpen.OpenFile());
                Manager.SetLeftPicture(image);
                leftPicture.Image = Manager.LeftImageBitmap;
                MemoryStream ms = new MemoryStream();
                image.Save(ms, ImageFormat.Bmp);
                int bytes = (int) ms.Length;
                SetSizeToolStrip(bytes);
                ms.Close();
            }
            catch (Exception ex) {
                MessageBox.Show(@"Error: Could not read file from disk. Original error: " + ex.Message);
            }

            Manager.SplitBytesIntoColorSpaces();
            Manager.DrawYcbcrOnRightBitmap(1);
            rightPicture.Image = Manager.RightImageBitmap;
            DctImageProcessor imageProcessor = new DctImageProcessor(Manager);
            Task.Run(() => imageProcessor.Process());
        }

        private void loadSecondFileToolStripMenuItem_Click(object sender, EventArgs e) {
            if (FOpen.ShowDialog() != DialogResult.OK)
                return;
            try {
                MPEGData[1] = new DataManager();
                Image image = Image.FromStream(FOpen.OpenFile());
                MPEGData[1].SetLeftPicture(image);
                MPEGData[1].box = leftPicture;
                MPEGData[1].rbox = rightPicture;
                rightPicture.Image = MPEGData[1].LeftImageBitmap;
                MemoryStream ms = new MemoryStream();
                image.Save(ms, ImageFormat.Bmp);
                int bytes = (int) ms.Length;
                SetSizeToolStrip(bytes);
                ms.Close();
            }
            catch (Exception ex) {
                MessageBox.Show(@"Error: Could not read file from disk. Original error: " + ex.Message);
            }

            MPEGData[1].SplitBytesIntoColorSpaces();
            MPEGData[1].DrawYcbcrOnRightBitmap(1);
        }

        private void loadFirstFileToolStripMenuItem_Click(object sender, EventArgs e) {
            if (FOpen.ShowDialog() != DialogResult.OK)
                return;
            try {
                MPEGData[0] = new DataManager();
                Image image = Image.FromStream(FOpen.OpenFile());
                MPEGData[0].SetLeftPicture(image);
                MPEGData[0].box = leftPicture;
                MPEGData[0].rbox = rightPicture;
                leftPicture.Image = MPEGData[0].LeftImageBitmap;
                MemoryStream ms = new MemoryStream();
                image.Save(ms, ImageFormat.Bmp);
                int bytes = (int) ms.Length;
                SetSizeToolStrip(bytes);
                ms.Close();
            }
            catch (Exception ex) {
                MessageBox.Show(@"Error: Could not read file from disk. Original error: " + ex.Message);
            }

            MPEGData[0].SplitBytesIntoColorSpaces();
            MPEGData[0].DrawYcbcrOnRightBitmap(1);
        }

        private void MVToolStripMenuItem_Click(object sender, EventArgs e) {
            MVs = ExaustiveSearch.Search(MPEGData[0], MPEGData[1]);
        }

        /// <summary>
        ///     Compress MPEG function.
        ///     Simply performs JPEG on the first.
        ///     Calculates MVs
        ///     Converts MVs to bytes
        ///     Appends length of MVs + MVs
        ///     onto the already compressed data.
        ///     Writes to file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void compressToolStripMenuItem1_Click(object sender, EventArgs e) {

            Thread procesThread = new Thread(new DctImageProcessor(MPEGData[0]).Process);

            await Task.Run(() => {
                MVs = ExaustiveSearch.Search(MPEGData[0], MPEGData[1]);
            });

            procesThread.Start();



            List<Tuple<Point, Point>> filteredMVs = MVs.Where(mV => mV.Item1 != mV.Item2).ToList();

            byte[] data = ObjectConverter.ObjectToByteArray(filteredMVs);
            byte[] MVsize = System.BitConverter.GetBytes(data.Length);
            procesThread.Join();

            sbyte[] encoded = MPEGData[0].EncodedBytes;

            IEnumerable<byte> output = data.Concat(encoded.Select(s => (byte)s));

            output = MVsize.Concat(output);

            data = output.ToArray();


            File.WriteAllBytes("./MPEGoutput.rippeg", data);
        }

        /// <summary>
        ///     Method to decompress MPEG
        ///     opens a file selector
        ///     reads data
        ///     initializes its data
        ///     sends it off to DctImageProcessor::UntoRippeg function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void decompressToolStripMenuItem2_Click(object sender, EventArgs e) {
            if (FOpen.ShowDialog() != DialogResult.OK)
                return;
            try {
                Stream raw = FOpen.OpenFile();
                using (MemoryStream ms = new MemoryStream()) {
                    raw.CopyTo(ms);
                    MPEGData[0] = new DataManager();

                    leftPicture.Image = null;
                    rightPicture.Image = null;
                    MPEGData[0].LoadedData = ms.ToArray().Select(s => (sbyte)s).ToArray();
                    SetSizeToolStrip(MPEGData[0].LoadedData.Length);
                    MPEGData[0].box = leftPicture;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(@"Error: Could not read file from disk. Original error: " + ex.Message);
            }

            DctImageProcessor processor = new DctImageProcessor(MPEGData[0]);

            await Task.Run(() => processor.UndoRippeg());

            //now we just have to use the vector info to move blocks around.
            Bitmap newmap = new Bitmap(MPEGData[0].ImageSize, MPEGData[0].ImageSize);

            for (int i = 0; i < MPEGData[0].ImageSize; i++) {
                for (int j = 0; j < MPEGData[0].ImageSize; j++) {
                    newmap.SetPixel(i, j, MPEGData[0].UncutBitmap.GetPixel(i, j));
                }
            }

            using (Graphics g = Graphics.FromImage(newmap)) {
                Pen p2 = new Pen(Color.White, 1);
                Pen p = new Pen(Color.Black, 2);
                foreach (Tuple<Point, Point> mV in MPEGData[0].MVs) {
                    for (int i = 0; i < 16; i++) {
                        for (int j = 0; j < 16; j++) {
                            newmap.SetPixel(
                                i + mV.Item2.X, j + mV.Item2.Y,
                                MPEGData[0].UncutBitmap.GetPixel(i + mV.Item1.X, j + mV.Item1.Y)
                                );
                        }
                    }
                    g.DrawLine(p, mV.Item1, mV.Item2);
                    g.DrawLine(p2, mV.Item1, mV.Item2);
                }
            }

            rightPicture.Image = Padder.CropImage(newmap,
                new Rectangle(0, 0, MPEGData[0].RealWidth, MPEGData[0].RealHeight));
        }

        /// <summary>
        ///     Pressing the HELP toolstrip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show(
                $"JPEG \n" +
                $"Use \"Compress\" and select a file to compress. \n" +
                $"Use \"Decompress\" following that to decompress the output.dct file created. \n\n" +
                $"MPEG\n" +
                $"To compress MPEG you have to load both pictures and press Compress inside MPEG => Compres. \n" +
                $"To view just the motion vectors, load two images and press MV. \n\n" +
                $"MPEG and general compression use different data structures so they cannot open each other\'s files. \n\n" +
                $"For MPEG to work both files have to be of the same size format. (movies never have frames of different format).\n\n\n" +
                $"For easier marking look at DctImageProcessor.cs / DCT.cs",
                $"Help"
                );
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e) {
            if(FOpen.ShowDialog() != DialogResult.OK)
                return;
            try {
                Stream raw = FOpen.OpenFile();
                using(MemoryStream ms = new MemoryStream()) {
                    raw.CopyTo(ms);
                    Manager = new DataManager();

                    leftPicture.Image = null;
                    rightPicture.Image = null;
                    Manager.LoadedData = ms.ToArray().Select(s => (sbyte) s).ToArray();
                    SetSizeToolStrip(Manager.LoadedData.Length);
                    Manager.box = leftPicture;
                    Manager.rbox = rightPicture;
                }
            } catch(Exception ex) {
                MessageBox.Show(@"Error: Could not read file from disk. Original error: " + ex.Message);
            }

            DctImageProcessor processor = new DctImageProcessor(Manager);
            processor.TestProcess();
        }

        private void rightPicture_Click(object sender, EventArgs e) {
            var ev = e as MouseEventArgs;
            if (ev == null || ev.Button != MouseButtons.Right) return;
            contextMenuStrip2.Show(Cursor.Position);
        }

        private void leftPicture_Click(object sender, EventArgs e) {
            var ev = e as MouseEventArgs;
            if(ev == null || ev.Button != MouseButtons.Right) return;
            contextMenuStrip1.Show(Cursor.Position);
        }

        private void HandleLeftContext(object sender, ToolStripItemClickedEventArgs e) {
            if (e.ClickedItem.Text== @"Compress Image") {
                contextMenuStrip1.Hide();
                if(FOpen.ShowDialog() != DialogResult.OK)
                    return;
                try {
                    Image image = Image.FromStream(FOpen.OpenFile());
                    Manager.SetLeftPicture(image);
                    leftPicture.Image = Manager.LeftImageBitmap;
                    MemoryStream ms = new MemoryStream();
                    image.Save(ms, ImageFormat.Bmp);
                    int bytes = (int)ms.Length;
                    SetSizeToolStrip(bytes);
                    ms.Close();
                } catch(Exception ex) {
                    MessageBox.Show(@"Error: Could not read file from disk. Original error: " + ex.Message);
                }

                Manager.SplitBytesIntoColorSpaces();
                Manager.DrawYcbcrOnRightBitmap(1);
                rightPicture.Image = Manager.RightImageBitmap;
                DctImageProcessor imageProcessor = new DctImageProcessor(Manager);
                Task.Run(() => imageProcessor.Process());
            }else if (e.ClickedItem.Text == @"Set Image") {
                contextMenuStrip1.Hide();
                SetLeftImage();
            }
            else {
                contextMenuStrip1.Hide();
                LeftImageClear();
            }
        }

        private void HandleRightContext(object sender, ToolStripItemClickedEventArgs e) {
            if(e.ClickedItem.Text == @"Set Image") {
                contextMenuStrip2.Hide();
                SetRightImage();
            }else if (e.ClickedItem.Text == @"Open Compressed") {
                contextMenuStrip2.Hide();
                if(FOpen.ShowDialog() != DialogResult.OK)
                    return;
                try {
                    Stream raw = FOpen.OpenFile();
                    using(MemoryStream ms = new MemoryStream()) {
                        raw.CopyTo(ms);
                        Manager = new DataManager();

                        leftPicture.Image = null;
                        rightPicture.Image = null;
                        Manager.LoadedData = ms.ToArray().Select(s => (sbyte)s).ToArray();
                        SetSizeToolStrip(Manager.LoadedData.Length);
                        Manager.box = leftPicture;
                        Manager.rbox = rightPicture;
                    }
                } catch(Exception ex) {
                    MessageBox.Show(@"Error: Could not read file from disk. Original error: " + ex.Message);
                }

                DctImageProcessor processor = new DctImageProcessor(Manager);
                Task.Run(() => processor.UndoProcess());
            } else {
                contextMenuStrip2.Hide();
                RightImageClear();
            }
        }

        

        private void LeftImageClear() {
            leftPicture.Image = null;
        }
        private void RightImageClear() {
            rightPicture.Image = null;
        }
        private void SetLeftImage() {
            if(FOpen.ShowDialog() != DialogResult.OK)
                return;
            if (FOpen.FileName != null) leftPicture.Image = new Bitmap(FOpen.FileName);
        }

        private void SetRightImage() {
            if(FOpen.ShowDialog() != DialogResult.OK)
                return;
            if(FOpen.FileName != null) rightPicture.Image = new Bitmap(FOpen.FileName);
        }
    }
}