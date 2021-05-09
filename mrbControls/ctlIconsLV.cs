using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mrbControls
{
    public partial class ctlIconsLV : UserControl
    {
        private List<KeyValuePair<int, string>> Columns;
        private List<List<string>> Items;
        private Dictionary<string, Image> Icons;
        private Dictionary<Point, string> AssignedIcons;
        private bool Updating;

        public ctlIconsLV()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            Updating = false;
        }

        public void SetColumns(List<KeyValuePair<int, string>> cols)
        {
            Columns = cols;
        }

        public void ClearItems()
        {
            Items = new List<List<string>>();
            if (!Updating) Refresh();
        }

        public void AddItem(List<string> item)
        {
            Items.Add(item);
            if (!Updating) Refresh();
        }

        public void AddRange(List<List<string>> items)
        {
            Items.AddRange(items);
            if (!Updating) Refresh();
        }

        public void BeginUpdate()
        {
            Updating = true;
        }

        public void EndUpdate()
        {
            if (Updating)
            {
                Updating = false;
                Refresh();
            }
        }

        public void ClearIcons()
        {
            AssignedIcons = new Dictionary<Point, string>();
            Icons = new Dictionary<string, Image>();
        }

        public void AddIcon(Image icon, string key)
        {
            Icons.Add(key, icon);
        }

        public void AssignIcon(Point coord, string iconKey)
        {
            AssignedIcons.Add(coord, iconKey);
            if (!Updating) Refresh();
        }
    }
}