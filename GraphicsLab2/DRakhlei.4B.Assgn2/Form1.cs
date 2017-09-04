using System;
using System.Drawing;
using System.Windows.Forms;

namespace DRakhlei._4B.Assgn2 {

    public partial class Form1 : Form {

        public double width;
        public double height;
        public double  max;
        public Point midpos;
        Circle c;


        public Form1() {
            InitializeComponent();
            height = pictureBox1.Height;
            width = pictureBox1.Width;
            midpos = (new Point((int)width / 2, (int)height / 2));
            max = Math.Min(width, height);
            drawShapes();
        }

        Circle shapePos(int pos) {
            double x;
            double y;
            Circle c = null;
            double max = this.max;
            switch (pos) {
                case 0:
                     x = (midpos.X - (max) * 5.0 / 12.0);
                     y = (midpos.Y - 5.0 / 12.0 * max);
                    c = new Circle();
                    c.topLeft = new Point((int) x,(int)y);
                    c.Radius = (int)(max / 3);
                    
                    break;
                case 1:
                    x = (midpos.X - (max) * 5.0 / 12.0);
                    y = (midpos.Y + 1.0 / 12.0 * max);
                    c = new Circle();
                    c.topLeft = new Point((int)x, (int)y);
                    c.Radius = (int)(max / 3);
                    break;
                case 2:
                    x = (midpos.X + (max) * 1.0 / 12.0);
                    y = (midpos.Y - 5.0 / 12.0 * max);
                    c = new Circle();
                    c.topLeft = new Point((int)x, (int)y);
                    c.Radius = (int)(max / 3);
                    break;
                case 3:
                    x = (midpos.X + (max) * 1.0 / 12.0);
                    y = (midpos.Y + 1.0 / 12.0 * max);
                    c = new Circle();
                    c.topLeft = new Point((int)x, (int)y);
                    c.Radius = (int)(max / 3);
                    break;
                case 4:
                    x = (midpos.X - (max) * 4.0 / 12.0);
                    y = (midpos.Y - 4.0 / 12.0 * max);
                    c = new Circle();
                    c.topLeft = new Point((int)x, (int)y);
                    c.Radius = (int)(max / 6);
                    break;
                case 5:
                    x = (midpos.X - (max) * 4.0 / 12.0);
                    y = (midpos.Y + 2.0 / 12.0 * max);
                    c = new Circle();
                    c.topLeft = new Point((int)x, (int)y);
                    c.Radius = (int)(max / 6);
                    break;
                case 6:
                    x = (midpos.X + (max) * 2.0 / 12.0);
                    y = (midpos.Y - 4.0 / 12.0 * max);
                    c = new Circle();
                    c.topLeft = new Point((int)x, (int)y);
                    c.Radius = (int)(max / 6);
                    break;
                case 7:
                    x = (midpos.X + (max) * 2.0 / 12.0);
                    y = (midpos.Y + 2.0 / 12.0 * max);
                    c = new Circle();
                    c.topLeft = new Point((int)x, (int)y);
                    c.Radius = (int)(max / 6);
                    break;
            }
            return c;            
        }

        public void drawShapes() {
            Graphics g = pictureBox1.CreateGraphics();
            g.Clear(Color.Black);
            for (int i = 0; i < 8; i++) {
                c = shapePos(i);
                switch (i ) {
                    case 0:
                        g.FillEllipse(new SolidBrush(Color.Blue), c.topLeft.X, c.topLeft.Y, c.Radius, c.Radius);
                        break;
                    case 1:
                        g.FillEllipse(new SolidBrush(Color.Blue), c.topLeft.X, c.topLeft.Y, c.Radius, c.Radius);
                        break;
                    case 2:
                        g.FillEllipse(new SolidBrush(Color.Yellow), c.topLeft.X, c.topLeft.Y, c.Radius, c.Radius);
                        break;
                    case 3:
                        g.FillEllipse(new SolidBrush(Color.Yellow), c.topLeft.X, c.topLeft.Y, c.Radius, c.Radius);
                        break;
                    case 4:
                        g.FillEllipse(new SolidBrush(Color.Green), c.topLeft.X, c.topLeft.Y, c.Radius, c.Radius);
                        break;
                    case 5:
                        g.FillEllipse(new SolidBrush(Color.Red), c.topLeft.X, c.topLeft.Y, c.Radius, c.Radius);
                        break;
                    case 6:
                        g.FillEllipse(new SolidBrush(Color.Green), c.topLeft.X, c.topLeft.Y, c.Radius, c.Radius);
                        break;
                    case 7:
                        g.FillEllipse(new SolidBrush(Color.Red), c.topLeft.X, c.topLeft.Y, c.Radius, c.Radius);
                        break;
                }
            }

        }

        private void pictureBox1_Resize(object sender, EventArgs e) {
            height = pictureBox1.Height;
            width = pictureBox1.Width;
            midpos = (new Point((int)width / 2, (int)height / 2));
            max = Math.Min(width, height);
            drawShapes();
        }

        private void pictureBox1_Click(object sender, EventArgs e) {
            height = pictureBox1.Height;
            width = pictureBox1.Width;
            midpos = (new Point((int)width / 2, (int)height / 2));
            max = Math.Min(width, height);
            drawShapes();
        }

        private void pictureBox1_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
            height = pictureBox1.Height;
            width = pictureBox1.Width;
            midpos = (new Point((int)width / 2, (int)height / 2));
            max = Math.Min(width, height);
            drawShapes();
        }
    }

    class Circle {
        public Point topLeft;
        public int Radius;
    }

}
