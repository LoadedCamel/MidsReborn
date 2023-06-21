using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.Controls
{
    public partial class FormPages : UserControl
    {
        public event EventHandler<int>? SelectedPageChanged;
        private int _pageIndex;

        [Editor("System.ComponentModel.Design.CollectionEditor, System.Design", typeof(UITypeEditor))]
        [Category("Behavior")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<Page> Pages { get; set; }

        [Category("Behavior")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [SettingsBindable(true)]
        [DefaultValue(1)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SelectedPage
        {
            get => _pageIndex;
            set
            {
                if (value < 0) _pageIndex = 1;
                else if (value > Pages.Count) _pageIndex = 1;
                else _pageIndex = value;
                SelectedPageChanged?.Invoke(this, _pageIndex);
            }
        }

        public FormPages()
        {
            InitializeComponent();
            BorderStyle = BorderStyle.FixedSingle;
            Pages = new ObservableCollection<Page>{new() {Name = "page1"}};
            _pageIndex = 1;
            Pages.CollectionChanged += PagesOnCollectionChanged;
            SelectedPageChanged += OnSelectedPageChanged;
        }

        private void OnSelectedPageChanged(object? sender, int pageIndex)
        {
            for (var index = 0; index < Pages.Count; index++)
            {
                var page = Pages[index];
                page.Visible = index == pageIndex - 1;
            }
        }

        private void PagesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add when e.NewItems != null:
                {
                    foreach (Page page in e.NewItems)
                    {
                        if (Controls.Contains(page)) return;
                        page.Location = new Point(3, 3);
                        page.Size = ClientRectangle.Size;
                        page.Dock = DockStyle.Fill;
                        Controls.Add(page);
                        Controls.SetChildIndex(page, e.NewStartingIndex);
                        page.Visible = e.NewStartingIndex == 0;
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Remove when e.OldItems != null:
                {
                    Controls.RemoveAt(e.OldStartingIndex);
                    break;
                }
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}
