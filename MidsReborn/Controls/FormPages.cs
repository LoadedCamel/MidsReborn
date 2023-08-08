using System;
using System.ComponentModel;
using System.Windows.Forms;
using MetaControls;
using MetaControls.Designer;

namespace Mids_Reborn.Controls
{
    [Designer(typeof(FormPagesDesigner))]
    public partial class FormPages : UserControl
    {
        #region Events

        public delegate void SelectedIndexChangedHandler(object? sender, int pageIndex);

        [Category("Behavior")]
        [Description("Occurs when the SelectedIndex value changes.")]
        public event SelectedIndexChangedHandler? SelectedIndexChanged;

        #endregion

        private int _selectedPageIndex = -1;

        [Category("Behavior")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<Page> Pages { get; set; }

        /// <summary>
        /// Gets or Sets the index of the currently selected page, if set to -1 then no page is selected.
        /// </summary>
        [Category("Behavior")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [SettingsBindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SelectedIndex
        {
            get => _selectedPageIndex;
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                if (value != -1 && value >= Pages.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _selectedPageIndex = value;
                SelectedIndexChanged?.Invoke(this, _selectedPageIndex);
            }
        }

        /// <summary>
        /// Gets or Sets the currently selected page.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Page? SelectedPage
        {
            get
            {
                var index = _selectedPageIndex;
                return index == -1 ? null : Pages[index];
            }
            set
            {
                var index = FindPage(value);
                _selectedPageIndex = index;
            }
        }

        public FormPages()
        {
            InitializeComponent();
            Load += OnLoad;
            Pages = new BindingList<Page>();
            Pages.ListChanged += PagesOnListChanged;
            SelectedIndexChanged += OnSelectedIndexChanged;
        }

        #region Navigational Methods

        /// <summary>
        /// Moves to the page specified by its index.
        /// </summary>
        public void NavigateTo(int index)
        {
            if (index < 0 || index > Pages.Count - 1) return;
            if (Pages[index].Visible) return;
            SelectedIndex = index;
            // for (var i = 0; i < Pages.Count - 1; i++)
            // {
            //     if (i == index)
            //     {
            //         SelectedIndex = i;
            //     }
            // }
        }

        /// <summary>
        /// Moves to the next page.
        /// </summary>
        public void NavigateForward()
        {
            if (_selectedPageIndex < 0 || _selectedPageIndex >= Pages.Count - 1) return;
            SelectedIndex++;
        }

        /// <summary>
        /// Moves to the previous page.
        /// </summary>
        public void NavigateBackward()
        {
            if (_selectedPageIndex <= 0) return;
            SelectedIndex--;
        }

        #endregion


        private int FindPage(Page? page)
        {
            if (Pages.Count == 0) return -1;
            for (var i = 0; i < Pages.Count; i++)
            {
                if (Pages[i].Equals(page))
                {
                    return i;
                }
            }

            return -1;
        }

        private void OnSelectedIndexChanged(object? sender, int pageIndex)
        {
            if (Pages.Count <= 0) return;
            if (pageIndex <= -1) return;
            for (var index = 0; index < Pages.Count - 1; index++)
            {
                Pages[index].Visible = index == pageIndex;
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            foreach (var page in Pages)
            {
                page.Size = ClientRectangle.Size;
                page.Dock = DockStyle.Fill;
            }
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            if (e.Control is not Page page) return;
            if (SelectedPage == page)
            {
                SelectedPage = Pages.Count > 0 ? Pages[^1] : null;
            }
            if (Pages.Contains(page))
            {
                Pages.Remove(page);
            }
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            foreach (var page in Pages)
            {
                page.Location = ClientRectangle.Location;
                page.Size = ClientRectangle.Size;
                page.Dock = DockStyle.Fill;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            foreach (var page in Pages)
            {
                page.Location = ClientRectangle.Location;
                page.Size = ClientRectangle.Size;
                page.Dock = DockStyle.Fill;
            }
        }

        private void PagesOnListChanged(object? sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    var page = Pages[e.NewIndex];
                    page.Location = ClientRectangle.Location;
                    page.Size = ClientRectangle.Size;
                    page.Dock = DockStyle.Fill;
                    Controls.Add(page);
                    if (Pages.Count < 2)
                    {
                        SelectedPage = page;
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    if (e.NewIndex < Controls.Count && Controls[e.NewIndex] is Page pageToRemove)
                    {
                        if (SelectedPage == pageToRemove)
                        {
                            SelectedPage = Pages.Count > 0 ? Pages[^1] : null;
                        }

                        Controls.Remove(pageToRemove);
                    }

                    break;
                case ListChangedType.ItemMoved:
                    break;
                case ListChangedType.ItemChanged:
                    break;
            }
        }
    }
}