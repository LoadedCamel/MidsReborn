using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.UIv2.Controls
{
    public partial class AoCombo : ComboBox
    {
        #region Enums
        public enum ComboBoxType
        {
            Archetype,
            Origin
        }
        #endregion

        #region Properties

        [Description("Type of the ComboBox")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ComboBoxType ComboType { get; set; }

        [Description("The font to be used on the label of the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Font Font { get; set; } = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold);

        [Description("Highlight color for selected item")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(typeof(Color), "Dodger Blue")]
        public Color HighlightColor { get; set; }

        [Description("Indicates the list that this control will use to get its items.")]
        [Category("Data")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new object? DataSource
        {
            get => _dataSource;
            set
            {
                _dataSource = value;
                base.DataSource = _dataSource;

                if (Items.Count <= 0) return;
                SelectedIndex = 0;
                OnSelectionChangeCommitted(EventArgs.Empty);
            }
        }

        private object? _dataSource;
        private Dictionary<string, Image>? _imageCache;
        
        #endregion

        public AoCombo()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            InitializeComponent();
            DrawMode = DrawMode.OwnerDrawVariable;
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                await FillImageCacheAsync();
                Invoke(Refresh);
            }
            catch (Exception ex)
            {
                // Handle exceptions (logging, notifying the user, etc.)
                Debug.WriteLine("Initialization failed: " + ex.Message);
            }
        }

        private async Task FillImageCacheAsync()
        {
            var imagePaths = new List<string>();
            switch (ComboType)
            {
                case ComboBoxType.Archetype:
                    imagePaths = await I9Gfx.LoadArchetypes();
                    break;
                case ComboBoxType.Origin:
                    imagePaths = await I9Gfx.LoadOrigins();
                    break;
            }

            _imageCache ??= new Dictionary<string, Image>();
            _imageCache.Clear();
            foreach (var imagePath in imagePaths)
            {
                if (string.IsNullOrWhiteSpace(imagePath) || _imageCache.ContainsKey(imagePath)) continue;
                var image = Image.FromFile(imagePath);
                _imageCache.Add(imagePath, image);
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0 && e.Index < Items.Count)
            {
                e.Graphics.FillRectangle((e.State & DrawItemState.Selected) == DrawItemState.Selected ? new SolidBrush(HighlightColor) : new SolidBrush(BackColor), e.Bounds);
                Image? comboImage = null;
                switch (ComboType)
                {
                    case ComboBoxType.Archetype:
                        if (Items[e.Index] is Archetype selectedArchetype)
                        {
                            comboImage = _imageCache?.FirstOrDefault(x => x.Key.Contains(selectedArchetype.ClassName)).Value;
                        }
                        break;
                    case ComboBoxType.Origin:
                        if (Items[e.Index] is string selectedOrigin)
                        {
                            comboImage = _imageCache?.FirstOrDefault(x => x.Key.Contains(selectedOrigin)).Value;
                        }
                        
                        break;
                }

                if (comboImage != null)
                {
                    e.Graphics.DrawImage(comboImage, e.Bounds.Left, e.Bounds.Top);
                    e.Graphics.DrawString(GetItemText(Items[e.Index]), Font, new SolidBrush(e.ForeColor), e.Bounds.Left + comboImage.Width + 5, e.Bounds.Top);
                }
                else
                {
                    e.Graphics.DrawString(GetItemText(Items[e.Index]), Font, new SolidBrush(e.ForeColor), e.Bounds.Left, e.Bounds.Top);
                }
            }

            base.OnDrawItem(e);
        }
    }
}
