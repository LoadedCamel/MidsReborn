using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Mids_Reborn.Controls
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public partial class FormPages : UserControl
    {
        #region Declarations

        #endregion

        #region Events

        

        #endregion

        #region Designer Props - Hidden

        public override Color BackColor { get; set; } = Color.LightGray;

        #endregion

        #region Designer Props - Visible

        [Description("The pages in the FormPages control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.ComponentModel.Design.CollectionEditor, System.Design", typeof(UITypeEditor))]
        [Category("Behavior")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public PagesCollection Pages { get; set; }

        #endregion

        #region Enums

        

        #endregion

        #region TypeConverters

        

        #endregion

        #region Collections

        public class PagesCollection : List<Page>, IEnumerable
        {
            private readonly List<Page> _pages = new();

            public new IEnumerator<Page> GetEnumerator()
            {
                return ((IEnumerable<Page>)_pages).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public new void Add(Page page)
            {
                _pages.Add(page);
            }

            public new void Remove(Page page)
            {
                _pages.Remove(page);
            }

            public new void Clear()
            {
                _pages.Clear();
            }
        }

        #endregion

        #region Constructor

        public FormPages()
        {
            Pages = new PagesCollection
            {
                new Page()
            };
            InitializeComponent();
        }

        #endregion

        #region Methods

        

        #endregion

        #region Override Methods

        

        #endregion

        #region EventHandler Methods

        

        #endregion

        #region SubControls

        public sealed class Page : Panel
        {
            #region Designer Props - Hidden

            [Browsable(false)]
            [Bindable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override AnchorStyles Anchor { get; set; } = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            [Browsable(false)]
            [Bindable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override DockStyle Dock { get; set; } = DockStyle.None;

            [Browsable(false)]
            [Bindable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public new Point Location { get; set; } = new(0, 0);

            [Browsable(false)]
            [Bindable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override bool AutoSize { get; set; }

            [Browsable(false)]
            [Bindable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override AutoSizeMode AutoSizeMode { get; set; }

            [Browsable(false)]
            [Bindable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override Size MaximumSize { get; set; }

            [Browsable(false)]
            [Bindable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override Size MinimumSize { get; set; }

            #endregion

            #region Designer Props - Visible
            
            
            #endregion

            #region Constructor

            public Page()
            {
                
            }

            #endregion
        }

        #endregion

        #region SubClasses

        internal sealed class PageDesigner : ParentControlDesigner
        {
            protected override Control? GetParentForComponent(IComponent component)
            {
                var page = component as Page;
                return page;
            }
        }

        #endregion
    }
}
