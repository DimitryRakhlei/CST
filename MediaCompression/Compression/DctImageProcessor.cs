using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;

namespace Compression {
    public class DctImageProcessor {
        /// <summary>
        ///     Constructor for DCT image Processor
        /// </summary>
        /// <param name="manager">The data manager that contains the program's data</param>
        public DctImageProcessor(DataManager manager) {
            Manager = manager;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public DataManager Manager { get; set; }

        
        public void TestProcess() {
            lock(Manager) {
                List<sbyte> dat = Manager.LoadedData.ToList();
                byte[] h = dat.Select(s => (byte)s).ToList().GetRange(0, 4).ToArray();
                byte[] w = dat.Select(s => (byte)s).ToList().GetRange(4, 8).ToArray();
                dat.RemoveRange(0, 8);
                Manager.LoadedData = dat.ToArray();
                
                int height = System.BitConverter.ToInt32(h, 0);
                int width = System.BitConverter.ToInt32(w, 0);
    

                Dct dct = new Dct();


                List<sbyte[]> decoded = Rle.DecodeRle(Manager.LoadedData);

                List<int[]> reverted = decoded.Select(l => l.Select(s => (int)s).ToArray()).ToList();//BitConverter.Revert(decoded);
                List<int[]> revertedlum = new List<int[]>();
                List<int[]> revertedcr = new List<int[]>();
                List<int[]> revertedcb = new List<int[]>();
                int block = reverted.Count / 6;
                for(int i = 0;i < reverted.Count;i++) {
                    if(i < block * 4) {
                        revertedlum.Add(reverted[i]);
                    } else if(i >= block * 4 && i < block * 5) {
                        revertedcr.Add(reverted[i]);
                    } else if(i >= block * 5) {
                        revertedcb.Add(reverted[i]);
                    }
                }

                

                Manager.LoadedData = null;
                decoded = null;
                reverted = null;
                GC.Collect();

                List<double[,]> lumIzig = revertedlum.Select(ZigZag.Inverse).ToList();
                List<double[,]> crIzig = revertedcr.Select(ZigZag.Inverse).ToList();
                List<double[,]> cbIzig = revertedcb.Select(ZigZag.Inverse).ToList();

                revertedlum = null;
                revertedcr = null;
                revertedcb = null;
                GC.Collect();

                //iquantize


                for(int i = 0;i < lumIzig.Count;i++) {
                    double[,] doubles = lumIzig[i];
                    Quantizer.InverseQuantizeLuminance(ref doubles);
                    lumIzig[i] = doubles;
                }

                for(int i = 0;i < crIzig.Count;i++) {
                    double[,] doubles = crIzig[i];
                    Quantizer.InverseQuantizeChrominance(ref doubles);
                    crIzig[i] = doubles;
                }

                for(int i = 0;i < cbIzig.Count;i++) {
                    double[,] doubles = cbIzig[i];
                    Quantizer.InverseQuantizeChrominance(ref doubles);
                    cbIzig[i] = doubles;
                }
                List<double[,]> idctLum = lumIzig.Select(doubles => dct.GoBack(doubles)).ToList();
                List<double[,]> idctCr = crIzig.Select(doubles => dct.GoBack(doubles)).ToList();
                List<double[,]> idctCb = cbIzig.Select(doubles => dct.GoBack(doubles)).ToList();


                lumIzig = null;
                crIzig = null;
                cbIzig = null;
                GC.Collect();

                //handling luminance

                int size = (int)Math.Sqrt(idctLum.Count);
                float[][] lumData = new float[size * 8][];
                for(int i = 0;i < size * 8;i++) {
                    lumData[i] = new float[size * 8];
                }

                int x = 0;
                int y = 0;
                foreach(double[,] doubles in idctLum) {
                    if(x >= size * 8) {
                        x = 0;
                        y += 8;
                    }
                    PutDataIntoArray(ref lumData, doubles, x, y, 8, 8);
                    x += 8;
                }
                List<float[]> lumlist = lumData.ToList();
                lumData = null;
                List<List<float>> lumout = lumlist.Select(f => f.ToList()).ToList();

                //handling cr
                size = (int)Math.Sqrt(idctCr.Count);
                float[][] crData = new float[size * 8][];
                for(int i = 0;i < size * 8;i++) {
                    crData[i] = new float[size * 8];
                }

                x = 0;
                y = 0;
                foreach(double[,] doubles in idctCr) {
                    if(x >= size * 8) {
                        x = 0;
                        y += 8;
                    }
                    PutDataIntoArray(ref crData, doubles, x, y, 8, 8);
                    x += 8;
                }
                List<float[]> crlist = crData.ToList();
                crData = null;
                List<List<float>> crout = crlist.Select(f => f.ToList()).ToList();

                //handling cb
                size = (int)Math.Sqrt(idctCb.Count);
                float[][] cbData = new float[size * 8][];
                for(int i = 0;i < size * 8;i++) {
                    cbData[i] = new float[size * 8];
                }
                x = 0;
                y = 0;
                foreach(double[,] doubles in idctCb) {
                    if(x >= size * 8) {
                        x = 0;
                        y += 8;
                    }
                    PutDataIntoArray(ref cbData, doubles, x, y, 8, 8);
                    x += 8;
                }
                List<float[]> cblist = cbData.ToList();
                cbData = null;
                List<List<float>> cbout = cblist.Select(f => f.ToList()).ToList();
                Manager.LumList = lumout;
                Manager.CrList = crout;
                Manager.CbList = cbout;

                Manager.ImageSize = size * 8 * 2;
                Manager.RealHeight = height;
                Manager.RealWidth = width;
                Manager.DisplayDecompressedData();

                Manager.box.Image = Manager.LeftImageBitmap;
            }
        }

        /// <summary>
        ///     Decompresses the *.dct file
        /// </summary>
        public void UndoProcess() {
            lock (Manager) {
                List<sbyte> dat = Manager.LoadedData.ToList();
                byte[] h = dat.Select(s => (byte)s).ToList().GetRange(0, 4).ToArray();
                byte[] w = dat.Select(s => (byte)s).ToList().GetRange(4, 8).ToArray();
                dat.RemoveRange(0, 8);
                Manager.LoadedData = dat.ToArray();


                int height = System.BitConverter.ToInt32(h, 0);
                int width = System.BitConverter.ToInt32(w, 0);

                Dct dct = new Dct();


                List<sbyte[]> decoded = Rle.DecodeRle(Manager.LoadedData);

                List<int[]> reverted = decoded.Select(l => l.Select(s => (int)s).ToArray()).ToList();//BitConverter.Revert(decoded);
                List<int[]> revertedlum = new List<int[]>();
                List<int[]> revertedcr = new List<int[]>();
                List<int[]> revertedcb = new List<int[]>();
                int block = reverted.Count/6;
                for (int i = 0; i < reverted.Count; i++) {
                    if (i < block*4) {
                        revertedlum.Add(reverted[i]);
                    }
                    else if (i >= block*4 && i < block*5) {
                        revertedcr.Add(reverted[i]);
                    }
                    else if (i >= block*5) {
                        revertedcb.Add(reverted[i]);
                    }
                }



                Manager.LoadedData = null;
                decoded = null;
                reverted = null;
                GC.Collect();

                List<double[,]> lumIzig = revertedlum.Select(ZigZag.Inverse).ToList();
                List<double[,]> crIzig = revertedcr.Select(ZigZag.Inverse).ToList();
                List<double[,]> cbIzig = revertedcb.Select(ZigZag.Inverse).ToList();

                revertedlum = null;
                revertedcr = null;
                revertedcb = null;
                GC.Collect();

                //iquantize


                for (int i = 0; i < lumIzig.Count; i++) {
                    double[,] doubles = lumIzig[i];
                    Quantizer.InverseQuantizeLuminance(ref doubles);
                    lumIzig[i] = doubles;
                }

                for (int i = 0; i < crIzig.Count; i++) {
                    double[,] doubles = crIzig[i];
                    Quantizer.InverseQuantizeChrominance(ref doubles);
                    crIzig[i] = doubles;
                }

                for (int i = 0; i < cbIzig.Count; i++) {
                    double[,] doubles = cbIzig[i];
                    Quantizer.InverseQuantizeChrominance(ref doubles);
                    cbIzig[i] = doubles;
                }
                List<double[,]> idctLum = lumIzig.Select(doubles => dct.GoBack(doubles)).ToList();
                List<double[,]> idctCr = crIzig.Select(doubles => dct.GoBack(doubles)).ToList();
                List<double[,]> idctCb = cbIzig.Select(doubles => dct.GoBack(doubles)).ToList();


                lumIzig = null;
                crIzig = null;
                cbIzig = null;
                GC.Collect();

                //handling luminance

                int size = (int) Math.Sqrt(idctLum.Count);
                float[][] lumData = new float[size*8][];
                for (int i = 0; i < size*8; i++) {
                    lumData[i] = new float[size*8];
                }

                int x = 0;
                int y = 0;
                foreach (double[,] doubles in idctLum) {
                    if (x >= size*8) {
                        x = 0;
                        y += 8;
                    }
                    PutDataIntoArray(ref lumData, doubles, x, y, 8, 8);
                    x += 8;
                }
                List<float[]> lumlist = lumData.ToList();
                lumData = null;
                List<List<float>> lumout = lumlist.Select(f => f.ToList()).ToList();

                //handling cr
                size = (int) Math.Sqrt(idctCr.Count);
                float[][] crData = new float[size*8][];
                for (int i = 0; i < size*8; i++) {
                    crData[i] = new float[size*8];
                }

                x = 0;
                y = 0;
                foreach (double[,] doubles in idctCr) {
                    if (x >= size*8) {
                        x = 0;
                        y += 8;
                    }
                    PutDataIntoArray(ref crData, doubles, x, y, 8, 8);
                    x += 8;
                }
                List<float[]> crlist = crData.ToList();
                crData = null;
                List<List<float>> crout = crlist.Select(f => f.ToList()).ToList();

                //handling cb
                size = (int) Math.Sqrt(idctCb.Count);
                float[][] cbData = new float[size*8][];
                for (int i = 0; i < size*8; i++) {
                    cbData[i] = new float[size*8];
                }
                x = 0;
                y = 0;
                foreach (double[,] doubles in idctCb) {
                    if (x >= size*8) {
                        x = 0;
                        y += 8;
                    }
                    PutDataIntoArray(ref cbData, doubles, x, y, 8, 8);
                    x += 8;
                }
                List<float[]> cblist = cbData.ToList();
                cbData = null;
                List<List<float>> cbout = cblist.Select(f => f.ToList()).ToList();
                Manager.LumList = lumout;
                Manager.CrList = crout;
                Manager.CbList = cbout;

                Manager.ImageSize = size*8*2;
                Manager.RealHeight = height;
                Manager.RealWidth = width;
                Manager.DisplayDecompressedData();


                if (Manager.box.InvokeRequired) {
                    Manager.box.Invoke((MethodInvoker) delegate {
                        Manager.rbox.Image = Manager.LeftImageBitmap;
                    });
                }
                else {
                    Manager.rbox.Image = Manager.LeftImageBitmap;
                }
            }
        }

        /// <summary>
        ///     Decompresses the MPEG file
        /// </summary>
        public void UndoRippeg() {
            lock (Manager) {
                List<byte> dat = Manager.LoadedData.Select(s => (byte)s).ToList();
                int mvsize = System.BitConverter.ToInt16(dat.GetRange(0, 4).ToArray(), 0);

                List<byte> mvlist = dat.GetRange(4, mvsize);

                List<Tuple<Point, Point>> MVs =
                    (List<Tuple<Point, Point>>) ObjectConverter.ByteArrayToObject(mvlist.ToArray());

                Manager.MVs = MVs;

                dat.RemoveRange(0, 4 + mvsize);


                byte[] h = dat.GetRange(0, 4).ToArray();
                byte[] w = dat.GetRange(4, 8).ToArray();

                dat.RemoveRange(0, 8);
                Manager.LoadedData = dat.Select(s=>(sbyte)s).ToArray();


                int height = System.BitConverter.ToInt32(h, 0);
                int width = System.BitConverter.ToInt32(w, 0);

                Dct dct = new Dct();


                List<sbyte[]> decoded = Rle.DecodeRle(Manager.LoadedData);

                List<int[]> reverted = decoded.Select(l => l.Select(s => (int)s).ToArray()).ToList(); //BitConverter.Revert(decoded);
                List<int[]> revertedlum = new List<int[]>();
                List<int[]> revertedcr = new List<int[]>();
                List<int[]> revertedcb = new List<int[]>();
                int block = reverted.Count/6;
                for (int i = 0; i < reverted.Count; i++) {
                    if (i < block*4) {
                        revertedlum.Add(reverted[i]);
                    }
                    else if (i >= block*4 && i < block*5) {
                        revertedcr.Add(reverted[i]);
                    }
                    else if (i >= block*5) {
                        revertedcb.Add(reverted[i]);
                    }
                }

                Manager.LoadedData = null;
                decoded = null;
                reverted = null;


                List<double[,]> lumIzig = revertedlum.Select(ZigZag.Inverse).ToList();
                List<double[,]> crIzig = revertedcr.Select(ZigZag.Inverse).ToList();
                List<double[,]> cbIzig = revertedcb.Select(ZigZag.Inverse).ToList();

                revertedlum = null;
                revertedcr = null;
                revertedcb = null;

                //needs work


                for (int i = 0; i < lumIzig.Count; i++) {
                    double[,] doubles = lumIzig[i];
                    Quantizer.InverseQuantizeLuminance(ref doubles);
                    lumIzig[i] = doubles;
                }

                for (int i = 0; i < crIzig.Count; i++) {
                    double[,] doubles = crIzig[i];
                    Quantizer.InverseQuantizeChrominance(ref doubles);
                    crIzig[i] = doubles;
                }

                for (int i = 0; i < cbIzig.Count; i++) {
                    double[,] doubles = cbIzig[i];
                    Quantizer.InverseQuantizeChrominance(ref doubles);
                    cbIzig[i] = doubles;
                }
                List<double[,]> idctLum = lumIzig.Select(doubles => dct.GoBack(doubles)).ToList();
                List<double[,]> idctCr = crIzig.Select(doubles => dct.GoBack(doubles)).ToList();
                List<double[,]> idctCb = cbIzig.Select(doubles => dct.GoBack(doubles)).ToList();


                lumIzig = null;
                crIzig = null;
                cbIzig = null;


                //handling luminance

                int size = (int) Math.Sqrt(idctLum.Count);
                float[][] lumData = new float[size*8][];
                for (int i = 0; i < size*8; i++) {
                    lumData[i] = new float[size*8];
                }

                int x = 0;
                int y = 0;
                foreach (double[,] doubles in idctLum) {
                    if (x >= size*8) {
                        x = 0;
                        y += 8;
                    }
                    PutDataIntoArray(ref lumData, doubles, x, y, 8, 8);
                    x += 8;
                }
                List<float[]> lumlist = lumData.ToList();
                lumData = null;
                List<List<float>> lumout = lumlist.Select(f => f.ToList()).ToList();

                //handling cr
                size = (int) Math.Sqrt(idctCr.Count);
                float[][] crData = new float[size*8][];
                for (int i = 0; i < size*8; i++) {
                    crData[i] = new float[size*8];
                }

                x = 0;
                y = 0;
                foreach (double[,] doubles in idctCr) {
                    if (x >= size*8) {
                        x = 0;
                        y += 8;
                    }
                    PutDataIntoArray(ref crData, doubles, x, y, 8, 8);
                    x += 8;
                }
                List<float[]> crlist = crData.ToList();
                crData = null;
                List<List<float>> crout = crlist.Select(f => f.ToList()).ToList();

                //handling cb
                size = (int) Math.Sqrt(idctCb.Count);
                float[][] cbData = new float[size*8][];
                for (int i = 0; i < size*8; i++) {
                    cbData[i] = new float[size*8];
                }
                x = 0;
                y = 0;
                foreach (double[,] doubles in idctCb) {
                    if (x >= size*8) {
                        x = 0;
                        y += 8;
                    }
                    PutDataIntoArray(ref cbData, doubles, x, y, 8, 8);
                    x += 8;
                }
                List<float[]> cblist = cbData.ToList();
                cbData = null;
                List<List<float>> cbout = cblist.Select(f => f.ToList()).ToList();
                Manager.LumList = lumout;
                Manager.CrList = crout;
                Manager.CbList = cbout;

                Manager.ImageSize = size*8*2;
                Manager.RealHeight = height;
                Manager.RealWidth = width;
                Manager.DisplayDecompressedData();

                Manager.box.Image = Manager.LeftImageBitmap;
            }
        }

        private void PutDataIntoArray(ref float[][] dest, double[,] data, int x, int y, int width, int height) {
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    dest[y + i][x + j] = (float) data[i, j];
                }
            }
        }


        /// <summary>
        ///     DctImageProcessor's main method.
        ///     This method will run on 8x8 chunks
        ///     and process them into double arrays by channel.
        ///     Even MPEG goes through here.
        ///     Once MPEG is compressed, we send data on
        ///     The MPEG obtains this JPEG data from Manager which is is
        ///     passed by reference and appends Vector data onto the
        ///     stream.
        /// </summary>
        public void Process() {
            lock (Manager) {
                //get data manager's ycbcr chromelist
                List<List<float>> luminanceDatalist = Manager.LumList;
                List<List<float>> crDatalist = Manager.CrList;
                List<List<float>> cbDatalist = Manager.CbList;
                List<double[,]> lumOutList = new List<double[,]>();
                List<double[,]> cbOutList = new List<double[,]>();
                List<double[,]> crOutList = new List<double[,]>();
                

                //dct
                Dct dct = new Dct();


                double[,] luminance = new double[luminanceDatalist.Count, luminanceDatalist[0].Count];
                double[,] cr = new double[crDatalist.Count, crDatalist[0].Count];
                double[,] cb = new double[cbDatalist.Count, cbDatalist[0].Count];


                for (int i = 0; i < luminanceDatalist.Count; i++) {
                    for (int j = 0; j < luminanceDatalist[0].Count; j++) {
                        luminance[i, j] = luminanceDatalist[i][j];
                    }
                }

                //quantize + dct
                List<double[,]> lumarr = luminance.ChunkMatrix(8, 8).ToList();
                foreach (double[,] d in lumarr) {
                    double[,] lumdct = dct.Go(d);
                    Quantizer.QuantizeLuminance(ref lumdct);
                    lumOutList.Add(lumdct);
                }

                for (int i = 0; i < crDatalist.Count; i++) {
                    for (int j = 0; j < crDatalist[0].Count; j++) {
                        cr[i, j] = crDatalist[i][j];
                    }
                }
                //quantize and dct cr
                List<double[,]> crarr = cr.ChunkMatrix(8, 8).ToList();
                foreach (double[,] d in crarr) {
                    double[,] crdct = dct.Go(d);
                    Quantizer.QuantizeChrominance(ref crdct);
                    crOutList.Add(crdct);
                }


                for (int i = 0; i < cbDatalist.Count; i++) {
                    for (int j = 0; j < cbDatalist[0].Count; j++) {
                        cb[i, j] = cbDatalist[i][j];
                    }
                }
                //quantize and dct cb
                List<double[,]> cbarr = cb.ChunkMatrix(8, 8).ToList();
                foreach (double[,] d in cbarr) {
                    double[,] cbdct = dct.Go(d);
                    Quantizer.QuantizeChrominance(ref cbdct);
                    cbOutList.Add(cbdct);
                }

                luminanceDatalist = null;
                crDatalist = null;
                cbDatalist = null;


                //zigzag 
                List<int[]> zigzagOutputs = lumOutList.Select(ZigZag.Run).ToList();
                zigzagOutputs.AddRange(crOutList.Select(ZigZag.Run));
                zigzagOutputs.AddRange(cbOutList.Select(ZigZag.Run));
                //List<byte[]> zaggedByteses = zigzagOutputs.Select(t => BitConverter.Convert(t.ToList())).ToList();
                List<sbyte[]> allzaggedbytes = zigzagOutputs.Select(t => t.Select(b => (sbyte) b).ToArray()).ToList();

                //encode the zigzag
                sbyte[] encoded = Rle.BetterEncode(allzaggedbytes);
                //append height and width onto output
                IEnumerable<sbyte> output = Manager.ImageHeight.Concat(encoded);
                output = Manager.ImageWidth.Concat(output);
                encoded = output.ToArray();
                //store data for MPEG (if MPEG wants it)
                Manager.EncodedBytes = encoded;
                //write data
                File.WriteAllBytes("./output.dct", encoded.Select(s => (byte)s).ToArray());
            }

        }

        public void UpdateProgress(object sender, ProgressChangedEventArgs args) {
            BackgroundWorker worker = sender as BackgroundWorker;
            
        }
    }
}