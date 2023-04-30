using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Mids_Reborn.Controls
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public partial class Page : UserControl
    {
        #region HiddenProps

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new string AccessibleDescription { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new AccessibleRole AccessibleRole { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new string AccessibleName { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public override DockStyle Dock { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new BorderStyle BorderStyle { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public override AnchorStyles Anchor { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public override bool AutoSize { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public override Size MaximumSize { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public override Size MinimumSize { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new ContextMenuStrip ContextMenuStrip { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new ImeMode ImeMode { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new Padding Padding { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new AutoSizeMode AutoSizeMode { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new MarginProperty Margin { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new Point Location { get; set; }

        #endregion

        public Page()
        {
            InitializeComponent();
        }
    }
}
