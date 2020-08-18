using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Hero_Designer
{
    public class ListBoxT<T>
    {
        private readonly ListBox _lb;

        public ListBoxT(ListBox lb)
        {
            _lb = lb;
        }

        public T SelectedItem
        {
            get => (T) _lb.SelectedItem;
            set => _lb.SelectedItem = value;
        }

        public void AddItem(T item)
        {
            _lb.Items.Add(item);
        }
    }

    public class ComboBoxT<T>
    {
        public ComboBoxT(ComboBox cb)
        {
            Value = cb;
        }

        public T SelectedItem
        {
            get => (T) Value.SelectedItem;
            set => Value.SelectedItem = value;
        }

        public Rectangle Bounds => Value.Bounds;
        public int Count => Value.Items.Count;
        public IReadOnlyCollection<T> Items => new ReadOnlyCollection<T>(Value.Items.Cast<T>().ToList());

        public bool Enabled
        {
            get => Value.Enabled;
            set => Value.Enabled = value;
        }

        public int SelectedIndex
        {
            get => Value.SelectedIndex;
            set => Value.SelectedIndex = value;
        }

        private ComboBox Value { get; }

        public T this[int x]
        {
            get => (T) Value.Items[x];
            set => Value.Items[x] = value;
        }

        public void BeginUpdate()
        {
            Value.BeginUpdate();
        }

        public void Clear()
        {
            Value.Items.Clear();
        }

        public void AddRange(IEnumerable<T> items)
        {
            Value.SuspendLayout();
            Value.Items.AddRange(items.Cast<object>().ToArray());
            Value.ResumeLayout();
        }

        public void EndUpdate()
        {
            Value.EndUpdate();
        }
    }

    public static class Extensions
    {
        public static void EventHandlerWithCatch(this IComponent _, Action f, string titlingOpt = null,
            string captionOpt = null)
        {
            ExecuteWithCatchMessage(f, titlingOpt, captionOpt);
        }

        // execute immediately with a catch
        private static void ExecuteWithCatchMessage(this Action f, string titlingOpt = null, string captionOpt = null)
        {
            try
            {
                f();
            }
            catch (Exception ex)
            {
                while (ex.InnerException?.Message != null)
                    ex = ex.InnerException;
                MessageBox.Show(string.IsNullOrWhiteSpace(titlingOpt) ? ex.Message : titlingOpt + ":" + ex.Message,
                    captionOpt ?? ex.GetType().Name);
            }
        }

        // this could be chained indefinitely so... be careful with it
        // defer the execution until later
        public static Action WithCatchMessage(this Action f, string titling, string captionOpt = null)
        {
            return () => ExecuteWithCatchMessage(f, titling, captionOpt);
        }

        // does not handle the possibility this is a child control, and a parent is in design mode
        public static bool IsInDesignMode(this Control c)
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime || c?.Site?.DesignMode == true;
        }
    }
}