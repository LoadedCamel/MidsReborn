using System.Collections;
using System.Collections.Generic;
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
    }
}
