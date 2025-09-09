using System.ComponentModel;
using System.Diagnostics;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.UI.Controls
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
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                FillImageCache(); // Changed to synchronous call
                Refresh();
            }
            catch (Exception ex)
            {
                // Handle exceptions (logging, notifying the user, etc.)
                Debug.WriteLine("Initialization failed: " + ex.Message);
            }
        }

        private void FillImageCache()
        {
            _imageCache ??= new Dictionary<string, Image>();
            _imageCache.Clear();

            switch (ComboType)
            {
                case ComboBoxType.Archetype:
                    // Populate from the pre-cached Archetypes dictionary
                    foreach (var kvp in AssetManager.Archetypes)
                    {
                        var archetype = DatabaseAPI.Database.Classes[kvp.Key];
                        var icon = kvp.Value;
                        if (archetype != null && icon?.Bitmap != null && !_imageCache.ContainsKey(archetype.ClassName))
                        {
                            _imageCache.Add(archetype.ClassName, new Bitmap(icon.Bitmap));
                        }
                    }
                    break;

                case ComboBoxType.Origin:
                    // Populate from the pre-cached Origins dictionary
                    foreach (var kvp in AssetManager.Origins)
                    {
                        var origin = DatabaseAPI.Database.Origins[kvp.Key];
                        var icon = kvp.Value;
                        if (origin != null && icon?.Bitmap != null && !_imageCache.ContainsKey(origin.Name))
                        {
                            _imageCache.Add(origin.Name, new Bitmap(icon.Bitmap));
                        }
                    }
                    break;
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
                if (_imageCache != null)
                {
                    switch (ComboType)
                    {
                        case ComboBoxType.Archetype:
                            if (Items[e.Index] is Archetype selectedArchetype)
                            {
                                // Use a direct dictionary lookup for the image
                                _imageCache.TryGetValue(selectedArchetype.ClassName, out comboImage);
                            }
                            break;
                        case ComboBoxType.Origin:
                            if (Items[e.Index] is string selectedOrigin)
                            {
                                // Use a direct dictionary lookup for the image
                                _imageCache.TryGetValue(selectedOrigin, out comboImage);
                            }
                            break;
                    }
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
