using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Recognize {
    internal class Detector {
        public Detector(bool isDefault = false) {
            string path = @"";
            if (isDefault) {
                path = @"..\..\data\classifiers\face\haarcascade_frontalface_alt.xml";
            }
            else {
                using (FileDialog selectCascadeDialog = new OpenFileDialog()) {
                    if (selectCascadeDialog.ShowDialog() == DialogResult.OK) {
                        path = selectCascadeDialog.FileName;
                    }
                }
            }
            _cls = new CascadeClassifier(path);
        }

        public Rectangle[] Scan(Image<Gray, byte> target) {
            return _cls.DetectMultiScale(target);
        }

        public List<Image<Gray, byte>> CropRegionOfInterest(Image<Gray, byte> input, Rectangle[] regionsOfInterest) {
            List<Image<Gray, byte>> output = new List<Image<Gray, byte>>();
            foreach (Rectangle rect in regionsOfInterest) {
                input.ROI = rect;
                output.Add(input.Copy());
            }
            return output;
        }

        #region Variables

        public int Found { get; private set; } = 0;
        private readonly CascadeClassifier _cls;

        #endregion
    }
}