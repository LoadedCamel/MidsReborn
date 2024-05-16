using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Controls.Converters;

namespace Mids_Reborn.UIv2.Controls
{
    public partial class CollapsiblePanel : Panel
    {
        #region Events

        private event EventHandler<StateText?>? DisplayTextChanged;
        public event EventHandler<States>? StateChanged;

        #endregion

        #region HiddenProp - Designer

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new FormBorderStyle BorderStyle { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image? BackgroundImage { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout { get; set; } = ImageLayout.None;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string? Text { get; set; }

        #endregion

        #region VisibleProp - Designer

        private Color _foreColor = Color.WhiteSmoke;
        private ControlBorder? _border = new();
        private StateText? _stateText = new();

        [Description("Sets the border for the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ControlBorder? Border
        {
            get => _border;
            set
            {
                // Check if the incoming value is different from the current value
                // This requires an appropriate comparison implementation in ControlBorder
                if (_border != null && _border.Equals(value)) return;

                // Detach event handler from the current (old) border
                if (_border != null)
                {
                    _border.PropertyChanged -= Property_Changed;
                }

                _border = value ?? throw new ArgumentNullException(nameof(value), @"Border cannot be null.");

                // Attach event handler to the new border
                _border.PropertyChanged += Property_Changed;
                Invalidate();
            }
        }

        [Description("The color used for the display text of the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Color ForeColor
        {
            get => _foreColor;
            set
            {
                _foreColor = base.ForeColor = value;
                Invalidate();
            }
        }

        [Description("The text to be displayed on the button based on state.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [SettingsBindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        public StateText? DisplayText
        {
            get => _stateText;
            set
            {
                _stateText = value;
                DisplayTextChanged?.Invoke(this, value);
            }
        }

        [Category("Behavior"), Description("Determines the direction in which the panel collapses.")]
        public Direction CollapseDirection
        {
            get => _collapseDirection;
            set
            {
                _collapseDirection = value;
                UpdateLayout(_collapseDirection);
            }
        }

        [Category("Behavior"), Description("Determines the speed of collapse/expand animation.")]
        public int CollapseSpeed { get; set; } = 10;

        #endregion

        #region PublicProps

        private States _state = States.Expanded;

        [Browsable(false)]
        public States State
        {
            get => _state;
            set
            {
                _state = value;
                StateChanged?.Invoke(this, value);
            }
        }

        #endregion

        #region Controls

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Timer AnimationTimer { get; }

        #endregion

        #region Enums

        [Browsable(false)]
        public enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }

        public enum States
        {
            Collapsed,
            Expanded
        }

        #endregion

        #region PrivateProps

        private bool _isCollapsed;
        private Size _originalSize = new(0, 0);
        private Size _minSize = new(0, 0);
        private Point _relativeToParent;
        private Direction _collapseDirection = Direction.Left;

        #endregion

        #region Constructor

        public CollapsiblePanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            AnimationTimer = new Timer { Interval = 10 };
            AnimationTimer.Tick += AnimationTimer_Tick;
            DisplayTextChanged += DisplayText_Changed;
            StateChanged += State_Changed;
            UpdateLayout(CollapseDirection);
        }

        private void DisplayText_Changed(object? sender, StateText? e)
        {
            var currentStateDisplayText = State switch
            {
                States.Expanded => DisplayText?.Expanded,
                States.Collapsed => DisplayText?.Collapsed,
                _ => ToggleButton.Text // Assuming a default case for unexpected states
            };

            if (ToggleButton.Text == currentStateDisplayText) return;
            ToggleButton.Text = currentStateDisplayText;
            ToggleButton.Invalidate();
        }

        #endregion

        #region Methods

        private void AdjustLayout()
        {
            // Adjust the layout based on the current direction and collapse state
            // This might involve setting the sizes and positions of internal components like the ToggleButton and ContentPanel
            UpdateLayout(CollapseDirection);
            // Any additional adjustments needed to maintain the visual integrity of the panel after resizing
        }

        private void State_Changed(object? sender, States states)
        {
            ToggleButton.Text = states switch
            {
                States.Expanded => DisplayText?.Expanded,
                States.Collapsed => DisplayText?.Collapsed,
                _ => ToggleButton.Text
            };
            ToggleButton.Invalidate();
        }

        private void Property_Changed(object? sender, PropertyChangedEventArgs e)
        {
            Invalidate();
        }

        private void ToggleButton_Click(object? sender, EventArgs e)
        {
            // Start the animation or directly toggle the state
            AnimationTimer.Start();
        }

        private void UpdateLayout(Direction direction)
        {
            // Adjust toggle button docking and content panel layout.
            switch (direction)
            {
                case Direction.Right or Direction.Left:
                    ToggleButton.Dock = (direction == Direction.Left) ? DockStyle.Left : DockStyle.Right;
                    // Adjusting _minSize for vertical layout (minimum width includes toggle button and minimum content area).
                    _minSize = new Size(ToggleButton.Width, Height);
                    break;
                case Direction.Down or Direction.Up:
                    ToggleButton.Dock = (direction == Direction.Up) ? DockStyle.Top : DockStyle.Bottom;
                    // Adjusting _minSize for horizontal layout (minimum height includes toggle button and minimum content area).
                    _minSize = new Size(ToggleButton.Width, ToggleButton.Height);
                    break;
            }
            Refresh();
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            if (_originalSize == Size.Empty)
            {
                _originalSize = new Size(Width, Height);
            }

            switch (CollapseDirection)
            {
                case Direction.Left:
                    AnimateHorizontal(!_isCollapsed, _minSize.Width, _originalSize.Width);
                    break;
                case Direction.Right:
                    AnimateHorizontal(_isCollapsed, _minSize.Width, _originalSize.Width);
                    break;
                case Direction.Up:
                    AnimateVertical(!_isCollapsed, _minSize.Height, _originalSize.Height);
                    break;
                case Direction.Down:
                    AnimateVertical(_isCollapsed, _minSize.Height, _originalSize.Height);
                    break;
            }
        }

        private void AnimateHorizontal(bool collapsing, int minDimension, int maxDimension)
        {
            var targetWidth = collapsing ? minDimension : maxDimension;
            var deltaWidth = Math.Abs(Width - targetWidth);
            var speed = Math.Max(1, deltaWidth / 10); // Dynamic speed for smooth animation

            if (collapsing)
            {
                if (Width > minDimension)
                {
                    Width -= speed;
                    if (CollapseDirection == Direction.Right) Left += speed; // Adjust position for right collapse
                }
                else
                {
                    FinishCollapse();
                }
            }
            else
            {
                if (Width < maxDimension)
                {
                    Width += speed;
                    if (CollapseDirection == Direction.Right) Left -= speed; // Adjust position for right expand
                }
                else
                {
                    FinishExpand();
                }
            }
        }

        private void AnimateVertical(bool collapsing, int minDimension, int maxDimension)
        {
            var targetHeight = collapsing ? minDimension : maxDimension;
            var deltaHeight = Math.Abs(Height - targetHeight);
            var speed = Math.Max(1, deltaHeight / 10); // Dynamic speed for smooth animation

            if (collapsing)
            {
                if (Height > minDimension)
                {
                    Height -= speed;
                    if (CollapseDirection == Direction.Down) Top += speed; // Adjust position for down collapse
                }
                else
                {
                    FinishCollapse();
                }
            }
            else
            {
                if (Height < maxDimension)
                {
                    Height += speed;
                    if (CollapseDirection == Direction.Down) Top -= speed; // Adjust position for down expand
                }
                else
                {
                    FinishExpand();
                }
            }
        }

        private void FinishCollapse()
        {
            Width = _minSize.Width;
            AnimationTimer.Stop();
            State = States.Collapsed;
            AdjustLayout();
        }

        private void FinishExpand()
        {
            Width = _originalSize.Width;
            AnimationTimer.Stop();
            State = States.Expanded;
            AdjustLayout();
        }

        private void AdjustAfterAnimation()
        {
            // Additional adjustments after animation completes
            // For example, adjust control positions, finalize sizes, etc.
        }

        #endregion

        #region DataClasses

        [TypeConverter(typeof(GenericTypeConverter<StateText>))]
        [Serializable]
        public class StateText
        {
            [Description("Text to set when in an exanded state.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public string Expanded { get; set; } = "Collapse";

            [Description("Text to set when in a collapsed state.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public string Collapsed { get; set; } = "Expand";

            public override string ToString()
            {
                return $"{Expanded}, {Collapsed}";
            }
        }

        #endregion

        #region Overrides

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustLayout();
            Invalidate(); // Force a redraw to apply the new sizes
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            if (Border == null) return;

            // Extract properties for each side
            var leftColor = Border.Left.Style != ButtonBorderStyle.None ? Border.Left.Color : Color.Transparent;
            var leftThickness = Border.Left.Style != ButtonBorderStyle.None ? Border.Left.Thickness : 0;
            var rightColor = Border.Right.Style != ButtonBorderStyle.None ? Border.Right.Color : Color.Transparent;
            var rightThickness = Border.Right.Style != ButtonBorderStyle.None ? Border.Right.Thickness : 0;
            var topColor = Border.Top.Style != ButtonBorderStyle.None ? Border.Top.Color : Color.Transparent;
            var topThickness = Border.Top.Style != ButtonBorderStyle.None ? Border.Top.Thickness : 0;
            var bottomColor = Border.Bottom.Style != ButtonBorderStyle.None ? Border.Bottom.Color : Color.Transparent;
            var bottomThickness = Border.Bottom.Style != ButtonBorderStyle.None ? Border.Bottom.Thickness : 0;

            // Draw borders based on the extracted properties
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                leftColor, leftThickness, Border.Left.Style,
                topColor, topThickness, Border.Top.Style,
                rightColor, rightThickness, Border.Right.Style,
                bottomColor, bottomThickness, Border.Bottom.Style);
        }

        #endregion
    }

}
