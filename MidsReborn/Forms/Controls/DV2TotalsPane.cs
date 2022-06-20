using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Mids_Reborn.Forms.Controls
{
    public partial class DV2TotalsPane : UserControl, IDrawLock
    {
        private List<Item> Items = new();
        private const int BarHeight = 10;
        private const float LabelsFontSize = 7;
        private int HoveredBar = -1;
        private bool _DrawLock;

        #region Custom events

        public event EventHandler<bool> PaneVisibilityChanged;


        public new event MouseEventHandler MouseClick
        {
            add
            {
                base.MouseClick += value;
                foreach (Control control in Controls)
                {
                    control.MouseClick += value;
                }
            }
            remove
            {
                base.MouseClick -= value;
                foreach (Control control in Controls)
                {
                    control.MouseClick -= value;
                }
            }
        }

        public delegate void BarHoverEventHandler(object sender, string containerControlName, Point mouseLoc, int barIndex, string label, float value, float uncappedValue);

        [Description("Occurs when the mouse pointer is over one of the bars")]
        public event BarHoverEventHandler BarHover;

        #endregion

        #region Public fields

        [Description("Maximum visible items")]
        [Category("Data")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        [DefaultValue(6)]
        public int MaxItems { get; set; }

        [Description("Maximum global bar value")]
        [Category("Data")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        [DefaultValue(100)]
        public float GlobalMaxValue { get; set; }

        [Description("Show values above bars")]
        [Category("Appearance")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        [DefaultValue(true)]
        public bool ShowNumbers { get; set; }

        [Description("Bars background gradient end color")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        public Color BackgroundColorEnd { get; set; }

        [Description("Bars color (main value)")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        public Color BarColorMain { get; set; }

        [Description("Bars color (uncapped value)")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        public Color BarColorUncapped { get; set; }

        [Description("Highlighted item background color")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        public Color HighlightBackgroundColor { get; set; }

        [Description("Enable uncapped values (dual bars)")]
        [Category("Data")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        public bool EnableUncappedValues { get; set; }

        #endregion

        public DV2TotalsPane()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            Load += DV2TotalsPane_Load;
            Resize += DV2TotalsPane_Resize;
            PaneVisibilityChanged += OnPaneVisibilityChanged;
            InitializeComponent();

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, panel1, new object[] { true });

            panel1.MouseLeave += panel1_MouseLeave;
            panel1.MouseMove += panel1_MouseMove;
        }

        #region IDrawLock implementation

        public void LockDraw()
        {
            _DrawLock = true;
        }

        public void UnlockDraw(bool redraw = true)
        {
            _DrawLock = false;
            if (!redraw)
            {
                return;
            }

            Draw();
        }

        #endregion

        #region Helper methods

        public void ClearItems(bool redraw = false)
        {
            Items.Clear();
            if (!redraw | _DrawLock)
            {
                return;
            }

            Draw();
        }

        public void AddItem(Item item, bool redraw = false)
        {
            Items.Add(item);
            if (!redraw | _DrawLock)
            {
                return;
            }

            Draw();
        }

        public void Draw()
        {
            if (_DrawLock)
            {
                return;
            }

            panel1.BackgroundImage = PaintSurface().ToBitmap();
        }

        #endregion

        #region Event callbacks

        private void DV2TotalsPane_Load(object sender, EventArgs e)
        {
            Draw();
        }

        private void DV2TotalsPane_Resize(object sender, EventArgs e)
        {
            Draw();
        }

        private SKImage PaintSurface()
        {
            const float barWidthFactor = 0.6f;
            const byte valueOpacity = 0xA9;

            using var s = SKSurface.Create(new SKImageInfo(panel1.Width, panel1.Height));

            s.Canvas.Clear(SKColors.Black);

            using var bgGradientPaint = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0), new SKPoint(Width, 0),
                    new[] { SKColors.Black, SKColors.Black, new SKColor().FromColor(BackgroundColorEnd) },
                    new[] { 0, 1 - barWidthFactor, 1 },
                    SKShaderTileMode.Clamp
                )
            };

            s.Canvas.DrawRect(new SKRect(0, 0, Width, Height), bgGradientPaint);

            using var barBg = new SKPaint { Color = SKColors.Black };
            using var linePaint = new SKPaint { Color = SKColors.Black };
            using var barUncapped = new SKPaint { Color = new SKColor().FromColor(BarColorUncapped) };
            using var textPaint = new SKPaint { Color = SKColors.WhiteSmoke };
            using var outlinePaint = new SKPaint { Color = SKColors.Black };
            using var barHighlightBg = new SKPaint { Color = new SKColor().AddAlpha(HighlightBackgroundColor, 171) }; // 66% opacity

            var xStart = (int) Math.Round(Width * (1 - barWidthFactor)) - 2;
            var globalMaxValue = GlobalMaxValue < float.Epsilon ? 100 : GlobalMaxValue;

            if (HoveredBar > -1 & HoveredBar < Math.Min(Items.Count, MaxItems))
            {
                s.Canvas.DrawRect(new SKRect(1, HoveredBar * 12 + 2, Width - 1, HoveredBar * 12 + 13), barHighlightBg);
            }

            for (var i = 0; i < Math.Min(Items.Count, MaxItems); i++)
            {
                var scale = (Width - 1) * barWidthFactor / globalMaxValue;
                var barGradientPaint = new SKPaint
                {
                    Shader = SKShader.CreateLinearGradient(
                        new SKPoint(xStart + 1, 0), new SKPoint(xStart + 1 + Items[i].Value * scale, 0),
                        new[] { new SKColor().FromColor(BarColorMain).Multiply(0.25f), new SKColor().FromColor(BarColorMain) },
                        new[] { 0, 1f },
                        SKShaderTileMode.Clamp
                    )
                };

                if (EnableUncappedValues)
                {
                    s.Canvas.DrawRect(new SKRect(xStart, i * 12 + 2, xStart + 2 + Items[i].UncappedValue * scale, i * 12 + 13), barBg);
                    if (Math.Abs(Items[i].UncappedValue - Items[i].Value) > float.Epsilon)
                    {
                        s.Canvas.DrawRect(new SKRect(xStart + 1, i * 12 + 3, xStart + 1 + Items[i].UncappedValue * scale, i * 12 + 12), barUncapped);
                    }

                    s.Canvas.DrawRect(new SKRect(xStart + 1, i * 12 + 3, xStart + 1 + Items[i].Value * scale, i * 12 + 12), barGradientPaint);

                    if (Math.Abs(Items[i].UncappedValue - Items[i].Value) > float.Epsilon)
                    {
                        s.Canvas.DrawLine(new SKPoint(xStart + 2 + Items[i].Value * scale, i * 12 + 2), new SKPoint(xStart + 2 + Items[i].Value * scale, i * 12 + 13), linePaint);
                    }
                }
                else
                {
                    s.Canvas.DrawRect(new SKRect(xStart, i * 12 + 2, xStart + 2 + Items[i].Value * scale, i * 12 + 13), barBg);
                    //s.Canvas.DrawRect(new SKRect(xStart + 1, i * 12 + 3, xStart + 1 + Items[i].UncappedValue * scale, i * 12 + 12), barUncapped);
                    s.Canvas.DrawRect(new SKRect(xStart + 1, i * 12 + 3, xStart + 1 + Items[i].Value * scale, i * 12 + 12), barGradientPaint);
                }

                // Bar label
                //s.Canvas.DrawText(Items[i].Name, new SKPoint(2, i * 12 + 4), textPaint);
                s.Canvas.DrawOutlineText(Items[i].Name, new SKPoint(2, i * 12 + 7 + LabelsFontSize / 2f), SKColors.WhiteSmoke);

                if (ShowNumbers)
                {
                    s.Canvas.DrawOutlineText($"{Items[i].Value:##0.##}%",
                        new SKPoint(Width - 2, i * 12 + 7 + LabelsFontSize / 2f),
                        Items[i].UncappedValue - Items[i].Value >= 0.01 ? SKColors.Cyan : SKColors.WhiteSmoke,
                        SKTextAlign.Right, valueOpacity);
                }
            }

            return s.Snapshot();
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            HoveredBar = -1;
            Draw();

            BarHover?.Invoke(panel1, Name, new Point(-1, -1), -1, "", 0, 0);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            const float outerGrow = 1f;

            // y in [barIndex * 12 + 2 - outerGrow, barIndex * 12 + 13 + outerGrow]
            // y >= barIndex * 12 + 2 - outerGrow & y <= barIndex * 12 + 13 + outerGrow]
            // y - 2 + outerGrow >= barIndex * 12 & y - 13 - outerGrow <= barIndex * 12  
            // (y - 2 + outerGrow) / 12 >= barIndex & (y - 13 - outerGrow) / 12 <= barIndex

            var indexLower = (int) Math.Floor((e.Y - 2 + outerGrow) / 12);
            var indexUpper = (int) Math.Ceiling((e.Y - 13 - outerGrow) / 12);
            var visibleItems = Math.Min(Items.Count, MaxItems);

            if (indexLower < 0 ||
                indexUpper < 0 ||
                indexLower >= visibleItems ||
                indexUpper >= visibleItems ||
                indexLower != indexUpper)
            {
                HoveredBar = -1;
                Draw();

                BarHover?.Invoke(panel1, Name, e.Location, -1, "", 0, 0);

                return;
            }

            HoveredBar = indexLower;
            Draw();

            BarHover?.Invoke(panel1, Name, e.Location, HoveredBar, Items[HoveredBar].Name, Items[HoveredBar].Value, Items[HoveredBar].UncappedValue);
        }

        private void OnPaneVisibilityChanged(object sender, bool e)
        {
            Visible = e;
        }

        #endregion

        #region Table label sub-class

        public class Item
        {
            private readonly float _Value;
            private readonly float _UncappedValue;

            public string Name { get; set; }
            public float Value => DisplayPercentage ? _Value * 100 : _Value;
            public float UncappedValue => DisplayPercentage ? _UncappedValue * 100 : _UncappedValue;
            public bool DisplayPercentage { get; set; }

            public Item(string name, float value, float uncappedValue, bool displayPercentage)
            {
                Name = name;
                _Value = value;
                _UncappedValue = uncappedValue;
                DisplayPercentage = displayPercentage;
            }
        }

        #endregion
    }
}