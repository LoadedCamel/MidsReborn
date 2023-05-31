using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    [DefaultEvent("SelectedPowersIndexChanged")]
    public sealed partial class PowersCombo : UserControl
    {
        #region Properties

        private Color _backColor = Color.Black;
        private Color _borderColor = Color.DodgerBlue;
        private Color _foreColor = Color.WhiteSmoke;
        private Color _highlightColor = Color.Goldenrod;
        private Color _highlightTextColor = Color.Black;
        private int _borderSize = 1;
        private string? _text;

        private Color IconColor => BorderColor;

        [Category("Appearance")]
        public new Color BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                _button.BackColor = value;
                _label.BackColor = value;
            }
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                _button.Invalidate();
            }
        }

        [Category("Appearance")]
        public new Color ForeColor
        {
            get => _foreColor;
            set
            {
                _foreColor = value;
                _label.ForeColor = value;
                _comboBox.ForeColor = value;
                _label.Invalidate();
                _comboBox.Invalidate();
            }
        }

        [Category("Appearance")]
        public Color HighlightColor
        {
            get => _highlightColor;
            set
            {
                _highlightColor = value;
                _comboBox.Invalidate();
            }
        }

        [Category("Appearance")]
        public Color HighlightTextColor
        {
            get => _highlightTextColor;
            set
            {
                _highlightTextColor = value;
                _comboBox.Invalidate();
            }
        }

        [Category("Appearance")]
        public int BorderWidth
        {
            get => _borderSize;
            set
            {
                _borderSize = value;
                Invalidate(true);
            }
        }

        [Category("Appearance")]
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                _comboBox.Font = value;
                _label.Font = value;
                _comboBox.Invalidate();
                _label.Invalidate();
            }
        }

        [Category("Appearance")]
        [Browsable(true)]
        public new string Text
        {
            get => _label.Text = _text??base.Text;
            set
            {
                _text = value;
                _label.Text = value;
                _label.Invalidate();
            }
        }

        [Category("Appearance")]
        public ComboBoxStyle DropDownStyle
        {
            get => _comboBox.DropDownStyle;
            set
            {
                if (_comboBox.DropDownStyle != ComboBoxStyle.Simple)
                {
                    _comboBox.DropDownStyle = value;
                }
                _comboBox.Invalidate();
            }
        }

        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(true)]
        [MergableProperty(false)]
        public ComboBox.ObjectCollection Items => _comboBox.Items;

        [Category("Data")]
        [AttributeProvider(typeof(IListSource))]
        [DefaultValue(null)]
        public object DataSource
        {
            get => _comboBox.DataSource;
            set => _comboBox.DataSource = value;
        }

        [Category("Misc")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Localizable(true)]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get => _comboBox.AutoCompleteCustomSource;
            set => _comboBox.AutoCompleteCustomSource = value;
        }

        [Category("Misc")]
        [Browsable(true)]
        [DefaultValue(AutoCompleteSource.None)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteSource AutoCompleteSource
        {
            get => _comboBox.AutoCompleteSource;
            set => _comboBox.AutoCompleteSource = value;
        }

        [Category("Misc")]
        [Browsable(true)]
        [DefaultValue(AutoCompleteMode.None)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteMode AutoCompleteMode
        {
            get => _comboBox.AutoCompleteMode;
            set => _comboBox.AutoCompleteMode = value;
        }

        [Category("Data")]
        [Bindable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedItem
        {
            get => _comboBox.SelectedItem;
            set => _comboBox.SelectedItem = value;
        }

        [Category("Data")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get => _comboBox.SelectedIndex;
            set => _comboBox.SelectedIndex = value;
        }

        [Category("Data")]
        [Browsable(true)]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string DisplayMember
        {
            get => _comboBox.DisplayMember;
            set => _comboBox.DisplayMember = value;
        }

        [Category("Data")]
        [Browsable(true)]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string? ValueMember
        {
            get => _comboBox.ValueMember;
            set => _comboBox.ValueMember = value;
        }

        #endregion

        #region SubControls

        private readonly ComboBox _comboBox;
        private readonly Label _label;
        private readonly Button _button;

        #endregion

        public event EventHandler? SelectedPowersIndexChanged;

        public PowersCombo()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
            _comboBox = new ComboBox();
            _button = new Button();
            _label = new Label();
            SuspendLayout();


            //ComboBox
            _comboBox.BackColor = BackColor;
            _comboBox.Font = Font;
            _comboBox.ForeColor = ForeColor;
            _comboBox.DropDownWidth = Width;
            _comboBox.DrawMode = DrawMode.OwnerDrawVariable;
            _comboBox.DrawItem += ComboBoxOnDrawItem;
            _comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            _comboBox.TextChanged += ComboBoxOnTextChanged;

            //Button
            _button.Dock = DockStyle.Right;
            _button.FlatStyle = FlatStyle.Flat;
            _button.FlatAppearance.BorderSize = 0;
            _button.BackColor = BackColor;
            _button.Size = new Size(30, 30);
            _button.Cursor = Cursors.Hand;
            _button.Click += ButtonOnClick;
            _button.Paint += ButtonOnPaint;

            //Label
            _label.Dock = DockStyle.Fill;
            _label.AutoSize = false;
            _label.BackColor = BackColor;
            _label.ForeColor = ForeColor;
            _label.TextAlign = ContentAlignment.MiddleLeft;
            _label.Padding = new Padding(8, 0, 0, 0);
            _label.Font = Font;
            _label.MouseDown += SurfaceOnMouseDown;
            _label.MouseEnter += SurfaceOnMouseEnter;
            _label.MouseLeave += SurfaceOnMouseLeave;
            _label.MouseWheel += SurfaceOnMouseWheel;

            //Control
            Controls.Add(_label);
            Controls.Add(_button);
            Controls.Add(_comboBox);
            MinimumSize = new Size(150, 30);
            Size = new Size(150, 30);
            Padding = new Padding(BorderWidth);
            base.BackColor = BorderColor;
            ResumeLayout();
            Load += OnLoad;
            InitializeComponent();
        }

        private void SurfaceOnMouseDown(object? sender, MouseEventArgs e)
        {
            OnMouseDown(e);
            if (e.Button != MouseButtons.Left || e.Clicks != 1) return;
            _comboBox.Select();
            if (_comboBox.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                _comboBox.DroppedDown = true;
            }
        }

        private void SurfaceOnMouseWheel(object? sender, MouseEventArgs e)
        {
            OnMouseWheel(e);
            if (e.Delta > 0)
            {
                if (_comboBox.SelectedIndex > 0)
                {
                    _comboBox.SelectedIndex--;
                }
            }
            else
            {
                if (_comboBox.SelectedIndex < Items.Count - 1)
                {
                    _comboBox.SelectedIndex++;
                }
            }
        }

        private void ComboBoxOnDrawItem(object? sender, DrawItemEventArgs e)
        {
            e.Graphics.CompositingMode = CompositingMode.SourceCopy;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            if (e.Index < 0 || e.Index >= Items.Count) return;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(HighlightColor), e.Bounds);
                TextRenderer.DrawText(e.Graphics, Items[e.Index].ToString(), Font, e.Bounds, HighlightTextColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
            else if ((e.State & DrawItemState.NoFocusRect) == DrawItemState.NoFocusRect ||
                     (e.State & DrawItemState.NoAccelerator) == DrawItemState.NoAccelerator)
            {
                e.Graphics.FillRectangle(new SolidBrush(BackColor), e.Bounds);
                TextRenderer.DrawText(e.Graphics, Items[e.Index].ToString(), Font, e.Bounds, ForeColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
            else
            {
                TextRenderer.DrawText(e.Graphics, Items[e.Index].ToString(), Font, e.Bounds, ForeColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }

            e.DrawFocusRectangle();
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            AdjustComboToFit();
        }

        private void AdjustComboToFit()
        {
            _comboBox.Width = Width - 2;
            _comboBox.Location = new Point
            {
                X = 1,
                Y = _label.Bottom - _comboBox.Height
            };
        }

        private void SurfaceOnMouseLeave(object? sender, EventArgs e)
        {
            OnMouseLeave(e);
        }

        private void SurfaceOnMouseEnter(object? sender, EventArgs e)
        {
            //OnMouseEnter(e);
            _comboBox.Focus();
        }

        private void ButtonOnPaint(object? sender, PaintEventArgs e)
        {
            var icoSize = new Size(14, 6);
            var rect = new Rectangle((_button.Width - icoSize.Width) / 2, (_button.Height - icoSize.Height) / 2, icoSize.Width, icoSize.Height);
            using var path = new GraphicsPath();
            using var pen = new Pen(IconColor, 2);
            e.Graphics.CompositingMode = CompositingMode.SourceCopy;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            path.AddLine(rect.X, rect.Y, rect.X + icoSize.Width / 2, rect.Bottom);
            path.AddLine(rect.X + icoSize.Width / 2, rect.Bottom, rect.Right, rect.Y);
            e.Graphics.DrawPath(pen, path);
        }

        private void ButtonOnClick(object? sender, EventArgs e)
        {
            _comboBox.Select();
            _comboBox.DroppedDown = true;
        }

        private void ComboBoxOnTextChanged(object? sender, EventArgs e)
        {
            _label.Text = _comboBox.Text;
        }

        private void ComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            SelectedPowersIndexChanged?.Invoke(this, EventArgs.Empty);
            _label.Text = _comboBox.Text;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustComboToFit();
        }
    }
}
