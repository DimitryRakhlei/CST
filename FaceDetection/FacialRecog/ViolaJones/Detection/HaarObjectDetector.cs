using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AForge.Imaging;
using FacialRecog.ViolaJones.Detection;
using FacialRecog.ViolaJones.Detection.HaarCascade;
using FacialRecog.ViolaJones.IntegralImage;
using FacialRecog.ViolaJones.Tools;
using FacialRecog;
using FacialRecog.ViolaJones.Detection.HaarCascade.Def;

namespace FacialRecog.ViolaJones.Detection {
    /// <summary>
    ///   Object detector options for the search procedure.
    /// </summary>
    /// 
    public enum ObjectDetectorSearchMode {
        /// <summary>
        ///   Entire image will be scanned.
        /// </summary>
        /// 
        Default = 0,

        /// <summary>
        ///   Only a single object will be retrieved.
        /// </summary>
        /// 
        Single,

        /// <summary>
        ///   If a object has already been detected inside an area,
        ///   it will not be scanned twice for inner/overlapping objects.
        /// </summary>
        /// 
        NoOverlap,
    }

    /// <summary>
    ///   Object detector options for window scaling.
    /// </summary>
    /// 
    public enum ObjectDetectorScalingMode {
        /// <summary>
        ///   Will start with a big search window and
        ///   gradually scale into smaller ones.
        /// </summary>
        /// 
        GreaterToSmaller,

        /// <summary>
        ///   Will start with small search windows and
        ///   gradually scale into greater ones.
        /// </summary>
        /// 
        SmallerToGreater,
    }

    /// <summary>
    ///   Viola-Jones Object Detector based on Haar-like features.
    /// </summary>
    /// <remarks>
    /// 
    /// <para>
    ///   The Viola-Jones object detection framework is the first object detection framework
    ///   to provide competitive object detection rates in real-time proposed in 2001 by Paul
    ///   Viola and Michael Jones. Although it can be trained to detect a variety of object
    ///   classes, it was motivated primarily by the problem of face detection.</para>
    ///   
    /// <para>
    ///   The implementation of this code has used Viola and Jones' original publication, the
    ///   OpenCV Library and the Marilena Project as reference. OpenCV is released under a BSD
    ///   license, it is free for both academic and commercial use. Please be aware that some
    ///   particular versions of the Haar object detection framework are patented by Viola and
    ///   Jones and may be subject to restrictions for use in commercial applications. The code
    ///   has been implemented with full support for tilted Haar features from the ground up.</para>
    ///   
    ///  <para>
    ///     References:
    ///     <list type="bullet">
    ///       <item><description>
    ///         <a href="http://www.cs.utexas.edu/~grauman/courses/spring2007/395T/papers/viola_cvpr2001.pdf">
    ///         Viola, P. and Jones, M. (2001). Rapid Object Detection using a Boosted Cascade
    ///         of Simple Features.</a></description></item>
    ///       <item><description>
    ///         <a href="http://en.wikipedia.org/wiki/Viola-Jones_object_detection_framework">
    ///         http://en.wikipedia.org/wiki/Viola-Jones_object_detection_framework</a>
    ///       </description></item>
    ///     </list>
    ///   </para>
    /// </remarks>
    /// 
    public class HaarObjectDetector : IObjectDetector {
        private List<Rectangle> detectedObjects;
        private HaarClassifier classifier;

        private ObjectDetectorSearchMode searchMode = ObjectDetectorSearchMode.NoOverlap;
        private ObjectDetectorScalingMode scalingMode = ObjectDetectorScalingMode.GreaterToSmaller;

        //  private Rectangle searchWindow;

        private Size minSize = new Size(15, 15);
        private Size maxSize = new Size(500, 500);
        private float factor = 1.2f;
        private int channel = RGB.R;

        private Rectangle[] lastObjects;
        private int steadyThreshold = 4;

        private int baseWidth;
        private int baseHeight;

        private int lastWidth;
        private int lastHeight;
        private float[] steps;

        #region Constructors

        /// <summary>
        ///   Constructs a new Haar object detector.
        /// </summary>
        /// <param name="cascade">
        ///   The <see cref="HaarCascade"/> to use in the detector's classifier.
        ///   For the default face cascade, please take a look on
        ///   <see cref="FaceHaarCascade"/>.
        /// </param>
        /// 
        public HaarObjectDetector(HaarCascade.HaarCascade cascade)
            : this(cascade, 15) {
        }

        /// <summary>
        ///   Constructs a new Haar object detector.
        /// </summary>
        /// <param name="cascade">
        ///   The <see cref="HaarCascade"/> to use in the detector's classifier.
        ///   For the default face cascade, please take a look on
        ///   <see cref="FaceHaarCascade"/>.
        /// </param>
        /// <param name="minSize">Minimum window size to consider when searching
        /// objects. Default value is <c>15</c>.</param>
        /// <param name="searchMode">The <see cref="ObjectDetectorSearchMode"/> to use
        /// during search. Please see documentation of <see cref="ObjectDetectorSearchMode"/>
        /// for details. Default value is <see cref="ObjectDetectorSearchMode.NoOverlap"/></param>
        /// 
        public HaarObjectDetector(HaarCascade.HaarCascade cascade, int minSize, ObjectDetectorSearchMode searchMode = ObjectDetectorSearchMode.NoOverlap)
            : this(cascade, minSize, searchMode, 1.2f) {
        }

        /// <summary>
        ///   Constructs a new Haar object detector.
        /// </summary>
        /// <param name="cascade">
        ///   The <see cref="HaarCascade"/> to use in the detector's classifier.
        ///   For the default face cascade, please take a look on
        ///   <see cref="FaceHaarCascade"/>.
        /// </param>
        /// <param name="minSize">Minimum window size to consider when searching
        /// objects. Default value is <c>15</c>.</param>
        /// <param name="searchMode">The <see cref="ObjectDetectorSearchMode"/> to use
        /// during search. Please see documentation of <see cref="ObjectDetectorSearchMode"/>
        /// for details. Default value is <see cref="ObjectDetectorSearchMode.NoOverlap"/></param>
        /// <param name="scaleFactor">The re-scaling factor to use when re-scaling the window during search.</param>
        /// 
        public HaarObjectDetector(HaarCascade.HaarCascade cascade, int minSize, ObjectDetectorSearchMode searchMode,
            float scaleFactor)
            : this(cascade, minSize, searchMode, scaleFactor, ObjectDetectorScalingMode.SmallerToGreater) {
        }

        /// <summary>
        ///   Constructs a new Haar object detector.
        /// </summary>
        /// <param name="cascade">
        ///   The <see cref="HaarCascade"/> to use in the detector's classifier.
        ///   For the default face cascade, please take a look on
        ///   <see cref="FaceHaarCascade"/>.
        /// </param>
        /// <param name="minSize">Minimum window size to consider when searching
        /// objects. Default value is <c>15</c>.</param>
        /// <param name="searchMode">The <see cref="ObjectDetectorSearchMode"/> to use
        /// during search. Please see documentation of <see cref="ObjectDetectorSearchMode"/>
        /// for details. Default is <see cref="ObjectDetectorSearchMode.NoOverlap"/>.</param>
        /// <param name="scaleFactor">The scaling factor to rescale the window
        /// during search. Default value is <c>1.2f</c>.</param>
        /// <param name="scalingMode">The <see cref="ObjectDetectorScalingMode"/> to use
        /// when re-scaling the search window during search. Default is <see cref="ObjectDetectorScalingMode.SmallerToGreater"/>.</param>
        /// 
        private HaarObjectDetector(HaarCascade.HaarCascade cascade, int minSize, ObjectDetectorSearchMode searchMode,
            float scaleFactor,
            ObjectDetectorScalingMode scalingMode) {
            this.classifier = new HaarClassifier(cascade);
            this.minSize = new Size(minSize, minSize);
            this.searchMode = searchMode;
            this.ScalingMode = scalingMode;
            this.factor = scaleFactor;
            this.detectedObjects = new List<Rectangle>();

            this.baseWidth = cascade.Width;
            this.baseHeight = cascade.Height;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether this <see cref="HaarObjectDetector"/>
        ///   should scan the image using multiple threads.
        /// </summary>
        /// 
        /// <value><c>true</c> to use multiple threads; otherwise, <c>false</c>.</value>
        /// 
        private bool UseParallelProcessing { get; set; }

        /// <summary>
        ///   Minimum window size to consider when searching objects.
        /// </summary>
        /// 
        public Size MinSize {
            get { return minSize; }
            set { minSize = value; }
        }

        /// <summary>
        ///   Maximum window size to consider when searching objects.
        /// </summary>
        /// 
        public Size MaxSize {
            get { return maxSize; }
            set { maxSize = value; }
        }

        /// <summary>
        ///   Gets or sets the color channel to use when processing color images. 
        /// </summary>
        /// 
        public int Channel {
            get { return channel; }
            set { channel = value; }
        }

        /// <summary>
        ///   Gets or sets the scaling factor to rescale the window during search.
        /// </summary>
        /// 
        public float ScalingFactor {
            get { return factor; }
            set {
                if (value == factor) return;
                factor = value;
                steps = null;
            }
        }

        /// <summary>
        ///   Gets or sets the desired searching method.
        /// </summary>
        /// 
        public ObjectDetectorSearchMode SearchMode {
            get { return searchMode; }
            set { searchMode = value; }
        }

        /// <summary>
        ///   Gets or sets the desired scaling method.
        /// </summary>
        /// 
        private ObjectDetectorScalingMode ScalingMode {
            get { return scalingMode; }
            set {
                if (value == scalingMode) return;
                scalingMode = value;
                steps = null;
            }
        }

        /// <summary>
        ///   Gets the detected objects bounding boxes.
        /// </summary>
        /// 
        public Rectangle[] DetectedObjects => detectedObjects.ToArray();

        /// <summary>
        ///   Gets the internal Cascade Classifier used by this detector.
        /// </summary>
        public HaarClassifier Classifier => classifier;

        /// <summary>
        ///   Gets how many frames the object has
        ///   been detected in a steady position.
        /// </summary>
        /// <value>
        ///   The number of frames the detected object
        ///   has been in a steady position.</value>
        ///   
        public int Steady { get; private set; }

        #endregion

        // TODO This may throw errors elsewhere
        /// <summary>
        ///   Performs object detection on the given frame.
        /// </summary>
        /// 
        public Rectangle[] ProcessFrame(Bitmap frame) {
            try {
                return ProcessFrame(UnmanagedImage.FromManagedImage(frame));
            }
            catch (Exception e) {
                return null;
            }
        }

        public Rectangle[] ProcessFrame(double[,] frame) {
            try {
                return ProcessFrame(UnmanagedImage.FromManagedImage(ImageFromArray(frame)));
            } catch (Exception e) {
                return null;
            }
        }

        private Bitmap ImageFromArray(double[,] input) {
            Bitmap output = new Bitmap(input.GetLength(0), input.GetLength(1));
            unsafe
            {
                BitmapData bitmapData =
                    output.LockBits(new Rectangle(0, 0, output.Width, output.Height),
                        ImageLockMode.ReadWrite, output.PixelFormat);

                int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(output.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                Parallel.For(0, heightInPixels, y => {
                    byte* currentLine = ptrFirstPixel + y * bitmapData.Stride;
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel) {
                        int oldBlue = (int)Math.Round(input[x / bytesPerPixel, y]);
                        int oldGreen = (int)Math.Round(input[x / bytesPerPixel, y]);
                        int oldRed = (int)Math.Round(input[x / bytesPerPixel, y]);

                        currentLine[x] = (byte)oldBlue;
                        currentLine[x + 1] = (byte)oldGreen;
                        currentLine[x + 2] = (byte)oldRed;
                        currentLine[x + 3] = 255;
                    }
                });
                output.UnlockBits(bitmapData);
            }
            return output;
        }


        /// <summary>
        ///   Performs object detection on the given frame.
        /// </summary>
        /// 
        public Rectangle[] ProcessFrame(UnmanagedImage image) {
            // Creates an integral image representation of the frame
            SIntegralImage integralImage = SIntegralImage.FromBitmap(
                image, channel, classifier.Cascade.HasTiltedFeatures);

            // Creates a new list of detected objects.
            this.detectedObjects.Clear();

            int width = integralImage.Width;
            int height = integralImage.Height;

            // Update parameters only if different size
            if (steps == null || width != lastWidth || height != lastHeight)
                update(width, height);


            Rectangle window = Rectangle.Empty;

            // For each scaling step
            foreach (float scaling in steps) {
                // Set the classifier window scale
                classifier.Scale = scaling;

                // Get the scaled window size
                window.Width = (int) (baseWidth*scaling);
                window.Height = (int) (baseHeight*scaling);

                // Check if the window is lesser than the minimum size
                if (window.Width < minSize.Width && window.Height < minSize.Height &&
                    window.Width > maxSize.Width && window.Height > maxSize.Height) {
                    // If we are searching in greater to smaller mode,
                    if (scalingMode == ObjectDetectorScalingMode.GreaterToSmaller) {
                        goto EXIT; // it won't get bigger, so we should stop.
                    }
                    else {
                        continue; // continue until it gets greater.
                    }
                }


                // Grab some scan loop parameters
                int xStep = window.Width >> 3;
                int yStep = window.Height >> 3;

                int xEnd = width - window.Width;
                int yEnd = height - window.Height;

                // Check if we should run in parallel or sequential
                if (!UseParallelProcessing) {
                    // Sequential mode. Scan the integral image searching
                    // for objects in the window without parallelization.

                    // For every pixel in the window column
                    for (int y = 0; y < yEnd; y += yStep) {
                        window.Y = y;

                        // For every pixel in the window row
                        for (int x = 0; x < xEnd; x += xStep) {
                            window.X = x;

                            if (searchMode == ObjectDetectorSearchMode.NoOverlap && overlaps(window))
                                continue; // We have already detected something here, moving along.

                            // Try to detect and object inside the window
                            if (classifier.Compute(integralImage, window)) {
                                // an object has been detected
                                detectedObjects.Add(window);

                                if (searchMode == ObjectDetectorSearchMode.Single)
                                    goto EXIT; // Stop on first object found
                            }
                        }
                    }
                }

                else {
                    // Parallel mode. Scan the integral image searching
                    // for objects in the window with parallelization.
                    ConcurrentBag<Rectangle> bag = new ConcurrentBag<Rectangle>();

                    int numSteps = (int) Math.Ceiling((double) yEnd/yStep);

                    // For each pixel in the window column
                    Parallel.For(0, numSteps, (j, options) => {
                        int y = j*yStep;

                        // Create a local window reference
                        Rectangle localWindow = window;

                        localWindow.Y = y;

                        // For each pixel in the window row
                        for (int x = 0; x < xEnd; x += xStep) {
                            if (options.ShouldExitCurrentIteration) return;

                            localWindow.X = x;

                            // Try to detect and object inside the window
                            if (classifier.Compute(integralImage, localWindow)) {
                                // an object has been detected
                                bag.Add(localWindow);

                                if (searchMode == ObjectDetectorSearchMode.Single)
                                    options.Stop();
                            }
                        }
                    });

                    // If required, avoid adding overlapping objects at
                    // the expense of extra computation. Otherwise, only
                    // add objects to the detected objects collection.
                    switch (searchMode) {
                        case ObjectDetectorSearchMode.NoOverlap:
                            foreach (Rectangle obj in bag.Where(obj => !overlaps(obj)))
                                detectedObjects.Add(obj);
                            break;
                        case ObjectDetectorSearchMode.Single:
                            if (bag.TryPeek(out window)) {
                                detectedObjects.Add(window);
                                goto EXIT;
                            }
                            break;
                        default:
                            foreach (Rectangle obj in bag)
                                detectedObjects.Add(obj);
                            break;
                    }
                }
            }


            EXIT:

            Rectangle[] objects = detectedObjects.ToArray();

            checkSteadiness(objects);
            lastObjects = objects;

            return objects; // Returns the array of detected objects.
        }

        private void update(int width, int height) {
            List<float> listSteps = new List<float>();

            // Set initial parameters according to scaling mode
            if (scalingMode == ObjectDetectorScalingMode.SmallerToGreater) {
                float start = 1f;
                float stop = Math.Min(width/(float) baseWidth, height/(float) baseHeight);
                float step = factor;

                for (float f = start; f < stop; f *= step)
                    listSteps.Add(f);
            }
            else {
                float start = Math.Min(width/(float) baseWidth, height/(float) baseHeight);
                float stop = 1f;
                float step = 1f/factor;

                for (float f = start; f > stop; f *= step)
                    listSteps.Add(f);
            }

            steps = listSteps.ToArray();

            lastWidth = width;
            lastHeight = height;
        }

        private void checkSteadiness(Rectangle[] rectangles) {
            if (lastObjects == null ||
                rectangles == null ||
                rectangles.Length == 0) {
                Steady = 0;
                return;
            }

            bool equals = true;
            foreach (Rectangle current in rectangles) {
                bool found = false;
                foreach (Rectangle last in lastObjects.Where(last => current.IsEqual(last, steadyThreshold))) {
                    found = true;
                    continue;
                }

                if (found) continue;
                @equals = false;
                break;
            }

            if (equals)
                Steady++;

            else
                Steady = 0;
        }

        private bool overlaps(Rectangle rect) {
            return detectedObjects.Any(rect.IntersectsWith);
        }
    }
}
