using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using Mids_Reborn.Core.Theming;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls
{
    [DefaultEvent("Click")]
    public sealed partial class MidsVectorButton : UserControl
    {
        #region Enums

        // Defines the button's primary behavior: a standard button or a toggle switch.
        public enum ButtonTypes
        {
            Normal,
            Toggle,
            Cycle
        }

        // Defines the possible states when the button is in Toggle mode.
        public enum States
        {
            ToggledOff,
            ToggledOn,
            Indeterminate
        }

        // Defines which mouse button triggers the toggle action.
        public enum MouseClicks
        {
            LeftButton,
            RightButton
        }

        #endregion

        #region Private Fields

        private string? _text; 
        private ButtonTypes _buttonType = ButtonTypes.Normal;
        private States _state = States.ToggledOff;
        private bool _isThreeState;

        private readonly StringCollection _cycleStates = new();
        private int _currentCycleIndex;

        private MouseClicks _toggleActivation = MouseClicks.LeftButton;
        private int _cornerRadius = 8;
        private bool _displayVertically;
        private bool _lock;


        private bool _isHovering;
        private bool _isPressed;

        #endregion

        #region Events

        [Category("Property Changed")]
        [Description("Occurs when the cycle state has changed.")]
        public event EventHandler? CycleStateChanged;

        #endregion

        #region Properties

        [Category("Appearance")]
        [Description("The text displayed on the control.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string? Text
        {
            get => _text ??= base.Text;
            set { _text = value; Invalidate(); }
        }

        [Category("Behavior")]
        [Description("Determines the behavior of the control.")]
        [DefaultValue(ButtonTypes.Normal)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ButtonTypes ButtonType
        {
            get => _buttonType;
            set { _buttonType = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Indicates the state of the control when in toggle mode.")]
        [DefaultValue(States.ToggledOff)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public States ToggleState
        {
            get => _state;
            set { _state = value; Invalidate(); }
        }

        [Category("Behavior")]
        [Description("Indicates whether the button allows three states (ToggledOff, ToggledOn, Indeterminate) instead of two.")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ThreeState
        {
            get => _isThreeState;
            set { _isThreeState = value; Invalidate(); }
        }

        [Category("Behavior")]
        [Description("Indicates which mouse button activates the toggle mechanism.")]
        [DefaultValue(MouseClicks.LeftButton)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public MouseClicks ToggleActivation
        {
            get => _toggleActivation;
            set { _toggleActivation = value; }
        }

        [Category("Appearance")]
        [Description("The radius of the control's corners. A higher value results in a more rounded shape.")]
        [DefaultValue(8)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value; Invalidate(); }
        }

        [Category("Behavior")]
        [Description("Determines if aspects of the control should be displayed vertically.")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool DisplayVertically
        {
            get => _displayVertically;
            set { _displayVertically = value; Invalidate(); }
        }

        [Category("Behavior")]
        [Description("Turns the locking mechanism on/off for the component, preventing it from switching states.")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Lock
        {
            get => _lock;
            set { _lock = value; Invalidate(); }
        }

        [Category("Behavior")]
        [Description("The list of text states for the button to cycle through when ButtonType is 'Cycle'.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public StringCollection? CycleStates => _cycleStates;

        [Category("Behavior")]
        [Description("The current index in the CycleStates list.")]
        [DefaultValue(0)]
        [Browsable(false)]
        public int CurrentCycleIndex
        {
            get => _currentCycleIndex;
            set
            {
                if (_cycleStates != null && value >= 0 && value < _cycleStates.Count)
                {
                    _currentCycleIndex = value;
                    OnCycleStateChanged();
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public string? CurrentCycleStateText =>
            (_cycleStates.Count > 0 && _currentCycleIndex >= 0 && _currentCycleIndex < _cycleStates.Count)
                ? _cycleStates[_currentCycleIndex]
                : null;

        #endregion

        #region Theme and Text Properties

        private ButtonTheme CurrentTheme
        {
            get
            {
                if (DesignMode)
                {
                    return ThemeManager.DesignTime.Button;
                }
                return ThemeManager.CurrentTheme?.Button ?? ThemeManager.DesignTime.Button;
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class StateText
        {
            public string? ToggledOff { get; set; } = "Off";
            public string? ToggledOn { get; set; } = "On";
            public string? Indeterminate { get; set; } = "Ind";
            public override string ToString() => "Toggle State Texts";
        }

        [Category("Appearance")]
        [Description("The text to be used by the control when in toggle mode.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public StateText ToggleText { get; set; }

        #endregion

        public MidsVectorButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint, true);

            BackColor = Color.Transparent;
            Size = new Size(100, 30);

            ToggleText = new StateText();
            InitializeComponent();

            if (!DesignMode) ThemeManager.ThemeChanged += Invalidate;
        }

        #region Event Handlers

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovering = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovering = false;
            _isPressed = false; // Ensure pressed state is cleared if mouse leaves
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (Lock) return; // If locked, do nothing.

            _isPressed = true;

            var activationButton = (ToggleActivation == MouseClicks.LeftButton) ? MouseButtons.Left : MouseButtons.Right;
            if (ButtonType == ButtonTypes.Toggle)
            {
                if (e.Button == activationButton)
                {
                    if (ThreeState)
                    {
                        ToggleState = ToggleState switch
                        {
                            States.ToggledOff => States.ToggledOn,
                            States.ToggledOn => States.Indeterminate,
                            States.Indeterminate => States.ToggledOff,
                            _ => States.ToggledOff
                        };
                    }
                    else
                    {
                        ToggleState = (ToggleState == States.ToggledOff) ? States.ToggledOn : States.ToggledOff;
                    }
                }
            }
            else if (ButtonType == ButtonTypes.Cycle) 
            {
                if (e.Button == activationButton && CycleStates is { Count: > 0 })
                {
                    _currentCycleIndex = (_currentCycleIndex + 1) % CycleStates.Count;
                    OnCycleStateChanged();
                }
            }


            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isPressed = false;
            Invalidate();
        }

        private void OnCycleStateChanged()
        {
            CycleStateChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Drawing Logic

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var theme = CurrentTheme;
            Color topColor, bottomColor, borderColor, textColor, textOutlineColor;
            float borderWidth, textOutlineWidth;
            string? textToDraw;

            if (ButtonType == ButtonTypes.Toggle)
            {
                textToDraw = ToggleState switch
                {
                    States.ToggledOn => ToggleText.ToggledOn,
                    States.Indeterminate => ToggleText.Indeterminate,
                    _ => ToggleText.ToggledOff,
                };
            }
            else if (ButtonType == ButtonTypes.Cycle)
            {
                textToDraw = CurrentCycleStateText;
            }
            else
            {
                textToDraw = Text;
            }

            bool isToggledOn = ButtonType == ButtonTypes.Toggle && (ToggleState == States.ToggledOn || ToggleState == States.Indeterminate);

            if (isToggledOn)
            {
                borderColor = theme.ToggledBorderColor;
                borderWidth = theme.ToggledBorderWidth;
                topColor = theme.ToggledGradientTop;
                bottomColor = theme.ToggledGradientBottom;
                textColor = theme.ToggledTextColor;
                textOutlineColor = theme.ToggledTextOutlineColor;
                textOutlineWidth = theme.TextOutlineWidth;
            }
            else
            {
                borderColor = theme.Border;
                borderWidth = 1f;
                textColor = theme.ForeColor;
                textOutlineColor = theme.TextOutlineColor;
                textOutlineWidth = theme.TextOutlineWidth;

                if (_isPressed && !Lock)
                {
                    topColor = theme.PressedGradientTop;
                    bottomColor = theme.PressedGradientBottom;
                }
                else if (_isHovering && !Lock)
                {
                    topColor = theme.HoverGradientTop;
                    bottomColor = theme.HoverGradientBottom;
                }
                else
                {
                    topColor = theme.GradientTop;
                    bottomColor = theme.GradientBottom;
                }
            }

            if (DisplayVertically)
            {
                e.Graphics.TranslateTransform(Width / 2f, Height / 2f);
                e.Graphics.RotateTransform(90);
                e.Graphics.TranslateTransform(-Height / 2f, -Width / 2f);
            }

            var buttonBounds = DisplayVertically
                ? new Rectangle(0, 0, Height - 1, Width - 1)
                : new Rectangle(0, 0, Width - 1, Height - 1);

            var textBounds = DisplayVertically
                ? new Rectangle(0, 0, Height, Width)
                : new Rectangle(0, 0, Width, Height);

            using (var path = GetRoundedRect(buttonBounds, _cornerRadius))
            {
                using (var brush = new LinearGradientBrush(textBounds, topColor, bottomColor, 90f))
                {
                    e.Graphics.FillPath(brush, path);
                }

                bool shouldDrawGlowBorder = (_isPressed | _isHovering && !Lock) || isToggledOn;
                if (shouldDrawGlowBorder)
                {
                    using var pen = new Pen(borderColor, borderWidth);
                    e.Graphics.DrawPath(pen, path);
                }
            }

            if (!string.IsNullOrEmpty(textToDraw))
            {
                using var textPath = new GraphicsPath();
                using var stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                float emSize = Font.SizeInPoints * e.Graphics.DpiY / 72f;
                textPath.AddString(textToDraw, Font.FontFamily, (int)Font.Style, emSize, textBounds, stringFormat);

                // 1. Draw the outline first.
                using (var outlinePen = new Pen(textOutlineColor, textOutlineWidth))
                {
                    outlinePen.LineJoin = LineJoin.Round;
                    e.Graphics.DrawPath(outlinePen, textPath);
                }

                // 2. Fill the text on top of the outline.
                using (var textBrush = new SolidBrush(textColor))
                {
                    e.Graphics.FillPath(textBrush, textPath);
                }
            }
        }

        // Helper method to create a rounded rectangle path
        private GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = radius * 2;
            if (diameter > bounds.Width) diameter = bounds.Width;
            if (diameter > bounds.Height) diameter = bounds.Height;

            var arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        #endregion
    }
}
