using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Recognize {
    public partial class Recognize : Form {
        #region components

        private Detector _featureDetector;

        #endregion

        public Recognize() {
            _featureDetector = new Detector(true);
            InitializeComponent();
        }

        private void loadImageButton_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.SafeFileName == null) return;
            if (dialog.ShowDialog() != DialogResult.OK) return;
            if (dialog.SafeFileName != null) {
                _curremtImage = new Image<Gray, byte>(dialog.FileName);
                currentImagePBox.Image = _curremtImage.ToBitmap();
                _origSize = _curremtImage.Size;
            }
            else MessageBox.Show(@"The file selection failed. Try again.");
        }

        private Image GetDisplayImage(Image<Gray, byte> input, Rectangle[] detectedRectangles) {
            foreach (Rectangle rect in detectedRectangles) {
                input.Draw(rect, new Gray(0), 2);
            }
            return input.ToBitmap();
        }

        private void detectButton_Click(object sender, EventArgs e) {
            // Scan the current image and obtain rectangles for everything that matches a face
            Task<Rectangle[]> task = new Task<Rectangle[]>(() => _featureDetector.Scan(_curremtImage));
            task.Start();
            Task.WaitAll(task);
            Rectangle[] ROI = task.Result;
            Image labeled_image = GetDisplayImage(_curremtImage.Copy(), ROI);

            // Slice the current image into small images based on obtained Regions of Interest
            _detectedFaces = _featureDetector.CropRegionOfInterest(_curremtImage, ROI);

            // Store the cropped images in the combo box and label them via ids
            int facenum = 0;
            List<ComboboxItem> items = _detectedFaces.Select(item => new ComboboxItem {
                Text = "Face: " + facenum++,
                Value = item
            }).ToList();

            // Restore Region of Interest to the whole image
            if (_curremtImage.IsROISet) {
                _curremtImage.ROI = new Rectangle(0, 0, _origSize.Width, _origSize.Height);
            }

            // The selected item of combo box will display the first element in combobox ( labeled_image)
            items.Insert(0,
                new ComboboxItem {Text = "Original Image", Value = new Image<Gray, byte>((Bitmap) labeled_image)});
            detectedFacesCombo.DataSource = items;
            detectedFacesCombo.Enabled = true;
            detFacesLabel.Enabled = true;
        }

        private void changeClassifierButton_Click(object sender, EventArgs e) {
            _featureDetector = new Detector(false);
            detectedFacesCombo.Enabled = false;
            detFacesLabel.Enabled = false;
            detectedFacesCombo.DataSource = new List<ComboboxItem>();
            _detectedFaces = new List<Image<Gray, byte>>();
        }

        private void detectedFacesCombo_SelectedIndexChanged(object sender, EventArgs e) {
            currentImagePBox.Image =
                ((Image<Gray, byte>) ((ComboboxItem) detectedFacesCombo.SelectedItem).Value).ToBitmap();
        }

        #region data

        private Image<Gray, byte> _curremtImage;
        private List<Image<Gray, byte>> _detectedFaces;
        private Size _origSize;

        #endregion
    }

    public class ComboboxItem {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString() {
            return Text;
        }
    }
}