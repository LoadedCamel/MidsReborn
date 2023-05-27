using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace Mids_Reborn.Core.Utils
{
    internal static class Helpers
    {
        public static IEnumerable<Control> GetControlHierarchy(Control root)
        {
            var queue = new Queue<Control>();
            queue.Enqueue(root);
            do
            {
                var control = queue.Dequeue();
                yield return control;
                foreach (var child in control.Controls.OfType<Control>())
                {
                    queue.Enqueue(child);
                }
            } while (queue.Count > 0);
        }

        public static IEnumerable<T> GetControlOfType<T>(Control.ControlCollection root) where T : Control
        {
            var controls = new List<T>();
            foreach (Control control in root)
            {
                if (control is T baseControl) controls.Add(baseControl);
                if (!control.HasChildren) continue;
                foreach (Control child in control.Controls)
                {
                    if (child is T childControl) controls.Add(childControl);
                }
            }

            return controls;
        }

        public static bool CompareVersions(Version versionA, Version versionB)
        {
            var comparisonResult = versionA.CompareTo(versionB);
            return comparisonResult > 0;
        }

        public static Bitmap ResizeImage(string path, Size size)
        {
            using var image = Image.FromFile(path);
            var destRect = new Rectangle(0, 0, size.Width, size.Height);
            var destImage = new Bitmap(size.Width, size.Height);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using var gfx = Graphics.FromImage(destImage);
            gfx.CompositingMode = CompositingMode.SourceCopy;
            gfx.CompositingQuality = CompositingQuality.HighQuality;
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.SmoothingMode = SmoothingMode.HighQuality;
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            using var imgAtt = new ImageAttributes();
            imgAtt.SetWrapMode(WrapMode.TileFlipXY);
            gfx.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imgAtt);
            return destImage;
        }
    }
}
