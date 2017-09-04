using DigiProAssignment;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace FixedProject {
    public partial class Form1 : Form {

        /*
        * Init The Dlls
        *
        */
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void loadBytesToDll(IntPtr pointer, int datasize);
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int startProg(int hInstance);
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int hello();
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int postQuitMessage();
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int postRecordMessage();
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int postStopRecord();
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int postPlayMessage();
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int postPlayPause();
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getByteDataSize();
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int postVolumeUp();
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int postVolumeDown();
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void storeByteData(IntPtr pointer);
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setSampR(int samp);
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setChannels(int chan);
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void bitDepth(int sampps);
        [DllImport("RecordPlay.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setBlockAllign(int allign);

        Thread DllThread;
        /*
        *End init Dll
        *
        */

        

        //clipboard manager
        Clipper clipboard = new Clipper();

        /*

            FORM INITIALIZATION FUNCTION

        */
        public Form1() {
            InitializeComponent();
            DllThread = new Thread(new ThreadStart(launchDllThread));
            DllThread.Start();
            chart1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chart1_keyup);
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            toolStripStatusLabel1.Text = "Zoom: " + "Disabled";
            toolStripStatusLabel2.Text = "Windowing: " + "Disabled";
        }

        //current recording state
        private bool recording = false;

        private double begin;
        private double end;
        private double dftbegin;
        private double dftend;

        //file data
        protected float[] left;
        protected float[] right;
        protected byte[] buffer;
        protected byte[] bufout;
        protected float[] dft_array;
        protected float[] chunk_for_dft;
        protected byte[] copyarray;

        protected float[] real;
        protected float[] imag;
        protected float[] convolutionReal;
        protected float[] convolutionImag;
        protected float[] convolution;
        

        //dll data
        public static IntPtr pointer;
        private static int size;
        public static byte[] data;

        //file loading classes
        public WaveFormat wf;
        public WaveFileReader pcm;
        public DirectSoundOut dout;
        public WaveFileWriter wfw;
        public static String fname;

        //windowing
        private float[] window;
        private bool isTriangle = false;
        private bool isWelch = false;

        private int convStart;
        private int convEnd;

        public void TriangleWindowSamp(float N) {
            window = new float[(int)N];
            for (int i = 0; i < N; i++) {
                window[i] = 1 - Math.Abs(((i - ((N - 1) / 2)) / (N / 2)));
            }
        }
        public void WelchWindow(float N) {
            window = new float[(int)N];
            for ( int n = 0; n < N; n ++  ) {
                window[n] = 1 - (float)Math.Pow((  ( n - ( (N - 1) / 2 ) ) / ( ( N - 1 ) / 2 )  ), 2);
            }
        }


        
        public void WindowData(ref float[] chunk) {
            for (int i = 0; i < window.Length; i++) {
                chunk[i] = chunk[i] * window[i];
            }
        }

        public void convolve(ref float [] chunk) {
            if (chunk != null || chunk.Length != 0) {
                float[] temp = new float[chunk.Length];
                Array.Copy(chunk, temp, chunk.Length);

                for (int n = 0; n < chunk.Length; n++) {
                    for (int m = 0; m < convolution.Length; m++) {
                        if (m + n >= chunk.Length) {
                            temp[n] += 0;
                        }
                        else {
                            temp[n] += (chunk[n + m] * convolution[m]);
                        }
                    }
                }
                chunk = new float[temp.Length];
                Array.Copy(temp, chunk, temp.Length);
            }
        }

        
        //selection data for copying
        public int startpos = 0;
        public int endpos = 0;

        public void FormClosingEvent() {
            postQuitMessage(); 
            DllThread.Abort();
            Application.Exit();
        }

        //launch dll thread for recording / playback
        private void launchDllThread() {
            startProg(Process.GetCurrentProcess().Id);
        }

        public void BytesToNewAlloc() {
            pointer = Marshal.AllocHGlobal(buffer.Length /*+ buffer.Length / 2 */);
            try {
                // Copy the array to unmanaged memory.
                Marshal.Copy(buffer, 0, pointer, buffer.Length);
            }
            finally {
                //listen to mangled sound
               
            }
            loadBytesToDll(pointer, buffer.Length);
        }


        private void setPointer(int i) {
            pointer = Marshal.AllocHGlobal(i);
            storeByteData(pointer);
            size = i;
            buffer = new byte[size];
            data = new byte[size];
            Marshal.Copy(pointer, data, 0, size);
        }

        private void pullData() {
            Array.Copy(data, buffer, data.Length);
            wf = new WaveFormat(22050, 16, 1);
            handleDataToArrays();
            chart1.Series["LChannel"].Points.Clear();
            foreach (var point in left) {
                chart1.Series["LChannel"].Points.AddY(point);
            }
          
        }

        private void handleDataToArrays() {
            int samplesDesired = buffer.Length;
            left = new float[samplesDesired / 3];
            right = new float[samplesDesired / 3];

            if (wf.BitsPerSample == 16) {

                if (wf.Channels == 1) {
                    for (int i = 0; i < buffer.Length / 4; i++)
                        left[i] = BitConverter.ToInt16(buffer, i * 4);
                }
                else if (wf.Channels == 2) {
                    int index = 0;
                    for (int sample = 0; sample < samplesDesired / 4; sample++) {
                        left[sample] = BitConverter.ToInt16(buffer, index);
                        index += 2;
                        right[sample] = BitConverter.ToInt16(buffer, index);
                        index += 2;
                    }
                }
            }
            else if (wf.BitsPerSample == 8) {
                if (wf.Channels == 1) {
                    for (int i = 0; i < buffer.Length; i++) {
                        byte b = buffer[i];
                        left[i] = b;
                    }
                }
                else if (wf.Channels == 2) {

                }
            }
        }




        //audio processing functions
        DFT dft = new DFT();
       

    private byte[] floatToByte(float f) {
            return BitConverter.GetBytes((short)f);
        }

        private void backToByte(float[] left, float[] right) {
            bufout = new byte[(left.Length + right.Length)];
            int iterator = 0;

            for (int i = 0; i < right.Length; i += 1) {
                if (i < left.Length && i < right.Length) {
                    var arl = floatToByte(left[i]);
                    var arr = floatToByte(right[i]);
                    Buffer.BlockCopy(arr, 0, bufout, iterator++ * 2, arr.Length);
                    Buffer.BlockCopy(arl, 0, bufout, iterator++ * 2, arl.Length);
                }
            }
        }
        private void readToArrays(WaveFileReader pcm) {

            wf = pcm.WaveFormat;
            fname = wf.ToString();
            int samplesDesired = (int)pcm.Length;

            buffer = new byte[samplesDesired];
            left = new float[samplesDesired / 2];
            right = new float[samplesDesired / 2];

            int bytesRead = pcm.Read(buffer, 0, samplesDesired);

            if (wf.BitsPerSample == 16) {

                if (wf.Channels == 1) {
                    for (int i = 0; i < buffer.Length / 4; i++)
                        left[i] = BitConverter.ToInt16(buffer, i * 4);
                }
                else if (wf.Channels == 2) {
                    int index = 0;
                    for (int sample = 0; sample < bytesRead / 4; sample++) {
                        left[sample] = BitConverter.ToInt16(buffer, index);
                        index += 2;
                        right[sample] = BitConverter.ToInt16(buffer, index);
                        index += 2;
                    }
                }
            }
            else if (wf.BitsPerSample == 8) {
                if (wf.Channels == 1) {

                }
                else if (wf.Channels == 2) {

                }
            }
        }

        private void WriteToFile() {
            if (wf.Channels == 1) {
                WaveFormat nwf = new WaveFormat(22050, 16, 1);
                File.Delete("a.wav");
                WaveFileWriter writer = new WaveFileWriter("a.wav", wf);
                writer.Write(buffer, 0, buffer.Length);
                writer.Flush();
            }
            else {
                backToByte(left, right);
                WaveFormat nwf = new WaveFormat(22050, 8, 1);
                File.Delete("a.wav");
                WaveFileWriter writer = new WaveFileWriter("a.wav", wf);
                writer.Write(bufout, 0, bufout.Length);
                writer.Flush();
            }

        }


        //data input decision making
        private void inputData(WaveFileReader p) {
            pcm = p;
            readToArrays(pcm);
        }


        private void RakhleiApp_FormClosed(object sender, FormClosedEventArgs e) {
            FormClosingEvent();
        }



        int nearestEvenInt(int to) {
            return (to % 2 == 0) ? to : (to + 1);
        }


        //data management

        public void copyDataSection(int beg, int end) {
            copyarray = new byte[(end - beg)];
            if (end > buffer.Length) {
                end = buffer.Length - 1;
            }
            else if (end < 0) {
                end = 1;
            }
            if (beg > buffer.Length) {
                beg = buffer.Length - 1;
            }
            else if (beg < 0) {
                beg = 1;
            }
            Buffer.BlockCopy(buffer, beg, copyarray, 0, ((end - 1) - beg));
            clipboard.toClipboard(copyarray);
        }

        public void cutDataSection(int beg, int end) {
            copyarray = new byte[Math.Abs(end - beg)];
            if ( end > buffer.Length ) {
                end = buffer.Length - 1;
            }else if ( end < 0 ) {
                end = 1;
            }
            if ( beg  > buffer.Length) {
                beg = buffer.Length - 1;
            }else if (beg < 0) {
                beg = 1;
            }
            Array.Copy(buffer, beg, copyarray, 0, Math.Abs(end - beg) );

            var tempList = buffer.ToList();
            tempList.RemoveRange(beg, Math.Abs(end - beg));
            buffer = tempList.ToArray();

            wf = new WaveFormat(22050, 16, 1);

            BytesToNewAlloc();
            handleDataToArrays();

            clipboard.toClipboard(copyarray);

            chart1.Series["LChannel"].Points.Clear();
            foreach (var point in left) {
                chart1.Series["LChannel"].Points.AddY(point);
            }

        }

        public void pasteDataSection(int pos) {
            var tempdist = buffer.ToList<byte>();
            if (end > buffer.Length) {
                end = buffer.Length - 1;
            }
            else if (end < 0) {
                end = 1;
            }
            if (pos > buffer.Length) {
                pos = buffer.Length - 1;
            }
            else if (pos < 0) {
                pos = 1;
            }
            // var tempsource = copyarray.ToList<byte>();
            var tempsource = clipboard.fromClipboard().ToList();
            tempdist.InsertRange(pos, tempsource);

            buffer = tempdist.ToArray();

            wf = new WaveFormat(22050, 16, 1);


            BytesToNewAlloc();
            handleDataToArrays();

            chart1.Series["LChannel"].Points.Clear();
            foreach (var point in left) {
                chart1.Series["LChannel"].Points.AddY(point);
            }
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            FOpen.Filter = "Wave File (*.wav)|*.wav";
            if (FOpen.ShowDialog() == DialogResult.OK) {
                pcm = new WaveFileReader(FOpen.FileName);
                readToArrays(pcm);
                BytesToNewAlloc();
                
                foreach(var point in left) {
                    chart1.Series["LChannel"].Points.AddY(point);
                }
                               
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            postPlayMessage();
        }

        private void button2_Click(object sender, EventArgs e) {
            postPlayPause();
        }

        private void button3_Click(object sender, EventArgs e) {
            if (recording) {
                button2.Enabled = true;
                button1.Enabled = true;
                recording = !recording;
                postStopRecord();
                setPointer(getByteDataSize() + 1);
                pullData();

            }
            else {
                button2.Enabled = false;
                button1.Enabled = false;
                recording = !recording;
                postRecordMessage();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            WriteToFile();
        }

        private void chart1_keyup (object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Z) {
                chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = !chart1.ChartAreas[0].AxisX.ScaleView.Zoomable;
                if (!chart1.ChartAreas[0].AxisX.ScaleView.Zoomable) {
                    toolStripStatusLabel1.Text = "Zoom: " + "Disabled";
                }
                else {
                    toolStripStatusLabel1.Text = "Zoom: " + "Enabled";
                }
            }
            else if (e.KeyCode == Keys.C) {
                copyDataSection(nearestEvenInt((int)begin * 4), nearestEvenInt((int)end * 4));
            }
            else if (e.KeyCode == Keys.X) {
                cutDataSection(nearestEvenInt((int)begin * 4), nearestEvenInt((int)end * 4));
            } else if ( e.KeyCode == Keys.V ) {
                pasteDataSection(nearestEvenInt((int)begin * 4));
            }

        }

        private void chart1_SelectionRangeChanging(object sender, System.Windows.Forms.DataVisualization.Charting.CursorEventArgs e) {
            if (e.NewSelectionStart < 0) {
                e.NewSelectionStart = 0;
            }
            if (e.NewSelectionEnd > left.Length) {
                e.NewSelectionEnd = left.Length;
            }

            begin = e.NewSelectionStart;
            end = e.NewSelectionEnd;

            toolStripStatusLabel3.Text = "S: " + (int)begin + " E: " + (int)end;
        }

        private void triangleToolStripMenuItem_Click(object sender, EventArgs e) {
            toolStripStatusLabel2.Text = "Windowing: " + "Triangle";
            isWelch = false;
            isTriangle = !isTriangle;
            if (triangleToolStripMenuItem.Checked) {
                triangleToolStripMenuItem.Checked = false;
                parzenToolStripMenuItem.Checked = false;
                parzenToolStripMenuItem.Enabled = true;
                toolStripStatusLabel2.Text = "Windowing: " + "Disabled";
            }
            else {
                triangleToolStripMenuItem.Checked = true;
                parzenToolStripMenuItem.Checked = false;
                parzenToolStripMenuItem.Enabled = false;
            }


        }

        private void parzenToolStripMenuItem_Click(object sender, EventArgs e) {
            toolStripStatusLabel2.Text = "Windowing: " + "Welch";
            isWelch =  !isWelch;
            isTriangle = false; ;
            if (parzenToolStripMenuItem.Checked) {
                parzenToolStripMenuItem.Checked = false;
                triangleToolStripMenuItem.Checked = false;
                triangleToolStripMenuItem.Enabled = true;
                toolStripStatusLabel2.Text = "Windowing: " + "Disabled";
            }
            else {
                parzenToolStripMenuItem.Checked = true;
                triangleToolStripMenuItem.Checked = false;
                triangleToolStripMenuItem.Enabled = false;
            }
        }

        private void threadedToolStripMenuItem_Click(object sender, EventArgs e) {
            chunk_for_dft = left.Skip((int)begin).Take(Math.Abs((int)end - (int)begin)).ToArray();
            //dft_array = dft(chunk_for_dft);

            if ( isWelch ) {
                WelchWindow(Math.Abs((int)end - (int)begin));
                WindowData(ref chunk_for_dft);
                dft_array = dft.ParallelDft(chunk_for_dft, ref real, ref imag);

                convStart = (int)begin;
                convEnd = (int)end;
            }
            else if ( isTriangle ) {
                TriangleWindowSamp(Math.Abs((int)end - (int)begin));
                WindowData(ref chunk_for_dft);
                dft_array = dft.ParallelDft(chunk_for_dft, ref real, ref imag);

                convStart = (int)begin;
                convEnd = (int)end;

            }
            else {
                dft_array = dft.ParallelDft(chunk_for_dft, ref real, ref imag);

                convStart = (int)begin;
                convEnd = (int)end;
            }

            chart3.Series[0].Points.Clear();
            foreach (var point in dft_array) {
                chart3.Series[0].Points.AddY(point);
            }

        }

        private void standardToolStripMenuItem_Click(object sender, EventArgs e) {
            chunk_for_dft = left.Skip((int)begin).Take(Math.Abs((int)end - (int)begin)).ToArray();
            //dft_array = dft(chunk_for_dft);

            if (isWelch) {
                WelchWindow(Math.Abs((int)end - (int)begin));
                WindowData(ref chunk_for_dft);
                dft_array = dft.dft(chunk_for_dft);
            }
            else if (isTriangle) {
                TriangleWindowSamp(Math.Abs((int)end - (int)begin));
                WindowData(ref chunk_for_dft);
                dft_array = dft.dft(chunk_for_dft);
            }
            else { dft_array = dft.dft(chunk_for_dft); }

            chart3.Series[0].Points.Clear();
            foreach (var point in dft_array) {
                chart3.Series[0].Points.AddY(point);
            }
        }

        private void lowPassToolStripMenuItem_Click(object sender, EventArgs e) {
            if ( !(dft_array == null) ) {
                
            } 
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            copyDataSection(nearestEvenInt((int)begin * 2), nearestEvenInt((int)end * 2));
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
            cutDataSection(nearestEvenInt((int)begin * 2), nearestEvenInt((int)end * 2));
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
            pasteDataSection(nearestEvenInt((int)begin * 2));
        }

        private void chart3_SelectionRangeChanging(object sender, System.Windows.Forms.DataVisualization.Charting.CursorEventArgs e) {
            e.NewSelectionStart = 0;
            dftbegin = e.NewSelectionStart;
            dftend = e.NewSelectionEnd;
            
        }

        private void button4_Click(object sender, EventArgs e) {
            chunk_for_dft = new float[dft_array.Length];
            Array.Copy(dft_array, chunk_for_dft, dft_array.Length);
            for ( int i = 0; i < (int)chart3.ChartAreas[0].AxisX.Maximum - 1; i++) {
                if ( i == 0) {

                }else if (i < (int)chart3.ChartAreas[0].AxisX.Maximum/2) {
                    if (i > (int)dftend) {
                        chunk_for_dft[i] = 0;
                    }else if (i < (int)dftend) {
                        
                    }
                }else if (i >= (int)chart3.ChartAreas[0].AxisX.Maximum/2) {
                    if (i < ((int)chart3.ChartAreas[0].AxisX.Maximum - (int)dftend)) {
                        chunk_for_dft[i] = 0;
                    }
                    if ( i > ((int)chart3.ChartAreas[0].AxisX.Maximum - (int)dftend)) {
                        
                    }
                } 
            }
            chart3.Series[0].Points.Clear();
            foreach (var point in chunk_for_dft) {
                chart3.Series[0].Points.AddY(point);
            }
        }

        private void button5_Click(object sender, EventArgs e) {
            chunk_for_dft = new float[dft_array.Length];
            Array.Copy(dft_array, chunk_for_dft, dft_array.Length);

            chart3.Series[0].Points.Clear();
            foreach (var point in chunk_for_dft) {
                chart3.Series[0].Points.AddY(point);
            }
        }

        private void button6_Click(object sender, EventArgs e) {
            convolutionReal = new float[(int)chart3.ChartAreas[0].AxisX.Maximum];
            convolutionImag = new float[(int)chart3.ChartAreas[0].AxisX.Maximum];

            for (int i = 0; i < (int)chart3.ChartAreas[0].AxisX.Maximum - 1; i++) {
                if (i == 0) {
                    convolutionImag[i] = -1;
                    convolutionReal[i] = 1;
                }
                else if (i < (int)chart3.ChartAreas[0].AxisX.Maximum / 2) {
                    if (i > (int)dftend) {
                        convolutionReal[i] = 0;
                        convolutionImag[i] = 0;
                    }
                    else if (i < (int)dftend) {
                        convolutionImag[i] = -1;
                        convolutionReal[i] = 1;
                    }
                }
                else if (i > (int)chart3.ChartAreas[0].AxisX.Maximum / 2) {
                    if ( i == (int)chart3.ChartAreas[0].AxisX.Maximum) {
                        convolutionImag[i] = -1;
                        convolutionReal[i] = 1;
                    }
                    else if (i <= ((int)chart3.ChartAreas[0].AxisX.Maximum - (int)dftend)) {
                        convolutionImag[i] = 0;
                        convolutionReal[i] = 0;
                    }
                    else if (i > ((int)chart3.ChartAreas[0].AxisX.Maximum - (int)dftend)) {
                        convolutionImag[i] = -1;
                        convolutionReal[i] = 1;
                    }
                }
            }
            Tabs.SelectedIndex = 0;

            convolution = dft.idft(convolutionReal, convolutionImag, (int)chart3.ChartAreas[0].AxisX.Maximum);

            float[] temparr = new float[chunk_for_dft.Length];
            Array.Copy(chunk_for_dft, temparr, chunk_for_dft.Length);

            byte [] tempbyte = new byte[chunk_for_dft.Length * 2];

            convolve(ref temparr);


            for (int i = 0; i < temparr.Length; i++) {
                byte[] tmp = floatToByte(temparr[i]);
                Buffer.BlockCopy(tmp, 0, tempbyte, i * 2, 2);
            }
             
            for (int i = convStart*4; (i - convStart * 4) < tempbyte.Length; i++) {
                buffer[ i ] = tempbyte[ (i-convStart * 4) ];
            }
            BytesToNewAlloc();
            handleDataToArrays();

            chart1.Series["LChannel"].Points.Clear();
            foreach (var point in left) {
                chart1.Series["LChannel"].Points.AddY(point);
            }
        }
    }
}
