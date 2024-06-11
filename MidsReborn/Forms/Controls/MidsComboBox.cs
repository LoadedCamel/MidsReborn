using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public sealed partial class MidsComboBox : UserControl
    {
        private readonly ListBox _listBox;
        private readonly Form _dropDownForm;
        private readonly Color _heroColor = Color.DodgerBlue;
        private readonly Color _villainColor = Color.FromArgb(255, 50, 50);

        private bool _isDroppedDown;

        public event EventHandler? SelectedIndexChanged;

        public MidsComboBox()
        {
            InitializeComponent();

            _dropDownForm = new Form();
            _dropDownForm.Paint += DropDownForm_Paint;

            _listBox = new ListBox
            {
                Visible = false,
                DrawMode = DrawMode.OwnerDrawFixed,
                BorderStyle = BorderStyle.None,
                ItemHeight = 18
            };

            _listBox.Paint += ListBox_Paint;
            _listBox.DrawItem += ListBox_DrawItem;
            _listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            _listBox.MouseMove += ListBox_MouseMove;
            _listBox.MouseClick += ListBox_MouseClick;

            Size = new Size(200, label.Height + 6);
            FontChanged += (_, _) => { label.Font = _listBox.Font = Font; };
            BackColorChanged += (_, _) => { label.BackColor = _listBox.BackColor = BackColor; };
            ForeColorChanged += (_, _) => { label.ForeColor = _listBox.ForeColor = ForeColor; };

            SelectedIndexChanged += (_, _) =>
            {
                label.Text = _listBox.SelectedItem.ToString();
            };
        }

        [Category("Appearance")]
        public Color BorderColor { get; set; } = Color.DodgerBlue;

        [Category("Appearance")]
        public int BorderWidth { get; set; } = 1;

        [Category("Appearance")]
        public Color HighlightColor { get; set; } = Color.Gold;

        [Category("Appearance")]
        public Color HighlightTextColor { get; set; } = Color.Black;

        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(true)]
        [MergableProperty(false)]
        public ListBox.ObjectCollection Items => _listBox.Items;

        [Category("Data")]
        [AttributeProvider(typeof(IListSource))]
        [DefaultValue(null)]
        public object? DataSource
        {
            get => _listBox.DataSource;
            set
            {
                _listBox.DataSource = value;
                _listBox.BindingContext = new BindingContext();
            }
        }

        [Category("Data")]
        [Browsable(true)]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string? DisplayMember
        {
            get => _listBox.DisplayMember;
            set => _listBox.DisplayMember = value;
        }

        [Category("Data")]
        [Browsable(true)]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string? ValueMember
        {
            get => _listBox.ValueMember;
            set => _listBox.ValueMember = value;
        }

        [Category("Data")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object? SelectedValue
        {
            get => _listBox.SelectedValue;
            set => _listBox.SelectedValue = value;
        }

        [Category("Data")]
        [Bindable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object? SelectedItem
        {
            get => _listBox.SelectedItem;
            set => _listBox.SelectedItem = value;
        }

        [Category("Data")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get => _listBox.SelectedIndex;
            set
            {
                if (value < -1 || value >= _listBox.Items.Count) return;
                _listBox.SelectedIndex = value;
            }
        }

        public void ResetBindings(bool metaDataChanged = true)
        {
            if (_listBox.DataSource is BindingSource bindingSource)
            {
                bindingSource.ResetBindings(metaDataChanged);
            }
        }

        public void ApplyHeroBorder(bool villain)
        {
            BorderColor = villain switch
            {
                false => _heroColor,
                true => _villainColor
            };
            Invalidate(true);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (ParentForm == null) return;
            ParentForm.FormClosed += ParentForm_FormClosed;
        }

        private void ParentForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            HideDropDown();
            _dropDownForm.Dispose();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            label.Width = Width - dropDownButton.Width - 6;
            dropDownButton.Location = new Point(Width - dropDownButton.Width - 3, 3);
            label.Height = Height - 6;
            dropDownButton.Height = Height - 6;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using var borderPen = new Pen(BorderColor, BorderWidth);
            e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
        }

        private void DropDownButton_Paint(object sender, PaintEventArgs e)
        {
            var buttonRect = dropDownButton.ClientRectangle;
            const int arrowWidth = 10;
            const int arrowHeight = 5;
            var arrowX = (buttonRect.Width - arrowWidth) / 2;
            var arrowY = (buttonRect.Height - arrowHeight) / 2;

            using Brush brush = new SolidBrush(HighlightColor);
            Point[] arrowPoints = {
                new(arrowX, arrowY),
                new(arrowX + arrowWidth, arrowY),
                new(arrowX + arrowWidth / 2, arrowY + arrowHeight)
            };
            e.Graphics.FillPolygon(brush, arrowPoints);

            // Suppress the focus rectangle
            ControlPaint.DrawFocusRectangle(e.Graphics, Rectangle.Empty);
        }

        private void DropDownButton_Click(object sender, EventArgs e)
        {
            if (!_isDroppedDown)
            {
                ShowDropDown();
            }
            else
            {
                HideDropDown();
            }
        }

        private void ShowDropDown()
        {
            _dropDownForm.FormBorderStyle = FormBorderStyle.None;
            _dropDownForm.StartPosition = FormStartPosition.Manual;
            _dropDownForm.ShowInTaskbar = false;
            _dropDownForm.TopMost = true;
            _dropDownForm.Size = new Size(Width, _listBox.Height);
            _dropDownForm.Controls.Add(_listBox);

            var location = PointToScreen(new Point(0, Height));
            _dropDownForm.Location = location;
            _dropDownForm.BackColor = BackColor;
            _dropDownForm.ForeColor = ForeColor;

            // Ensure ListBox height is recalculated properly
            RecalculateListBoxHeight();

            _listBox.Location = new Point(1, 1);
            _listBox.Width = _dropDownForm.Width - 2;
            _listBox.Visible = true;
            _dropDownForm.Show();

            _isDroppedDown = true;
        }

        private void HideDropDown()
        {
            _dropDownForm.Hide();
            _isDroppedDown = false;
        }

        private void RecalculateListBoxHeight()
        {
            // Ensure the calculation is completed before continuing
            var itemCount = _listBox.Items.Count;
            var visibleItemCount = Math.Min(itemCount, 10); // Display up to 10 items
            _listBox.Height = visibleItemCount * _listBox.ItemHeight;
            _dropDownForm.Height = _listBox.Height + 2; // Adjust the form height accordingly
        }

        private void ListBox_MouseMove(object? sender, MouseEventArgs e)
        {
            var index = _listBox.IndexFromPoint(e.Location);
            if (index >= 0 && index < _listBox.Items.Count)
            {
                _listBox.SelectedIndex = index;
            }
        }

        private void ListBox_MouseClick(object? sender, MouseEventArgs e)
        {
            var index = _listBox.IndexFromPoint(e.Location);
            if (index < 0 || index >= _listBox.Items.Count) return;
            _listBox.SelectedIndex = index;
            HideDropDown();
        }

        private void ListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            Debug.WriteLine($"ListBox_SelectedIndexChanged: { _listBox.SelectedItem}");
            if (_listBox.SelectedIndex >= 0) // Ensure valid index
            {
                SelectedIndexChanged?.Invoke(this, e);
            }
        }

        private void ListBox_Paint(object? sender, PaintEventArgs e)
        {
            using var borderPen = new Pen(BorderColor, BorderWidth);
            e.Graphics.DrawRectangle(borderPen, 1, 1, _listBox.Width - 2, _listBox.Height - 2);
        }

        private void ListBox_DrawItem(object? sender, DrawItemEventArgs e)
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
                TextRenderer.DrawText(e.Graphics, Items[e.Index].ToString(), Font, e.Bounds, HighlightTextColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(BackColor), e.Bounds);
                TextRenderer.DrawText(e.Graphics, Items[e.Index].ToString(), Font, e.Bounds, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }

            e.DrawFocusRectangle();
        }

        private void DropDownForm_Paint(object? sender, PaintEventArgs e)
        {
            using var borderPen = new Pen(BorderColor, BorderWidth);
            e.Graphics.DrawRectangle(borderPen, 0, 0, _dropDownForm.Width - 1, _dropDownForm.Height - 1);
        }
    }
}
