using System.Drawing;
using AForge.Imaging;

namespace FacialRecog.ViolaJones.Detection
{
    /// <summary>
    ///   Object detector interface.
    /// </summary>
    public interface IObjectDetector
    {
        /// <summary>
        ///   Gets the location of the detected objects.
        /// </summary>
        Rectangle[] DetectedObjects { get; }

        /// <summary>
        ///   Process a new image scene looking for objects.
        /// </summary>
        Rectangle[] ProcessFrame(UnmanagedImage image);
    }
}
