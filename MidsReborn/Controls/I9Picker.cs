using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using FontStyle = System.Drawing.FontStyle;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace Mids_Reborn.Controls
{
    public class I9Picker : UserControl
    {
        public delegate void EnhancementPickedEventHandler(I9Slot e);

        public delegate void EnhancementSelectionCancelledEventHandler();

        public delegate void HoverEnhancementEventHandler(int e, EnhUniqueStatus? enhUniqueStatus);

        public delegate void HoverSetEventHandler(int e);

        public delegate void MovedEventHandler(Rectangle oldBounds, Rectangle newBounds);

        public struct EnhUniqueStatus
        {
            public bool InMain;
            public bool InAlternate;
        }

        private const bool AllowPlusThreeSpecialO = true;

        private const int IoMax = 50;
        private Color _cHighlight;
        private Color _cSelected;
        private clsDrawX? _hDraw;
        private int _headerHeight;
        private Point _hoverCell;
        private string _hoverText;
        private string _hoverTitle;
        private Enums.eEnhGrade _lastGrade;
        private Enums.eEnhRelative _lastRelativeLevel;
        private int _lastSet;
        private int _lastSpecial;
        private Enums.eType _lastTab;
        private bool _levelCapped;

        private Point _mouseOffset;
        private ExtendedBitmap _myBx;
        private I9Slot _mySlot;
        private int[] _mySlotted;
        private int _nPad;
        private int _nPowerIdx;
        private int _nSize;
        private int _rows;
        private double _timerLastKeypress;
        private int _userLevel;

        private IContainer components;
        public int LastLevel;

        private IEnhancement _selectedEnhancement;
        public CTracking Ui;

        private Rectangle _buttonRectangle;

        private List<EnhUniqueStatus?> _enhUniqueStatus = null;

        private enum ActiveZone
        {
            None,
            EnhType,
            Enhancement,
            EnhancementSet,
            CloseButton
        }

        public I9Picker()
        {
            Load += I9PickerLoad;
            Paint += I9PickerPaint;
            KeyDown += I9PickerKeyDown;
            MouseDown += I9PickerMouseDown;
            MouseMove += I9PickerMouseMove;
            _rows = 5;
            _hoverCell = new Point(-1, -1);
            _hoverTitle = "";
            _hoverText = "";
            _mySlotted = Array.Empty<int>();
            _userLevel = -1;
            _lastTab = Enums.eType.Normal;
            LastLevel = -1;
            _lastGrade = Enums.eEnhGrade.SingleO;
            _lastRelativeLevel = Enums.eEnhRelative.Even;
            _lastSpecial = 1;
            Ui = new CTracking();
            _nPad = 8;
            _nSize = 30;
            _cHighlight = Color.SlateBlue;
            _cSelected = Color.BlueViolet;
            _nPowerIdx = -1;
            _myBx = new ExtendedBitmap(Width, Height);
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.ContainerControl |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
        }

        [field: AccessedThroughProperty("tTip")]
        private ToolTip TTip { get; set; }

        public Color Highlight
        {
            get => _cHighlight;
            set
            {
                _cHighlight = value;
                FullDraw();
            }
        }

        public Color Selected
        {
            get => _cSelected;
            set
            {
                _cSelected = value;
                FullDraw();
            }
        }

        public new int Padding
        {
            get => _nPad;
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                if (value > 100)
                {
                    value = 100;
                }

                _nPad = value;
                FullDraw();
            }
        }

        public int ImageSize
        {
            get => _nSize;
            set
            {
                if (value < 1)
                {
                    value = 1;
                }

                if (value > 256)
                {
                    value = 256;
                }

                _nSize = value;
                FullDraw();
            }
        }

        public event EnhancementPickedEventHandler EnhancementPicked;
        public event EnhancementSelectionCancelledEventHandler EnhancementSelectionCancelled;
        public event HoverEnhancementEventHandler HoverEnhancement;
        public event HoverSetEventHandler HoverSet;
        public event MovedEventHandler Moved;

        public new void BringToFront()
        {
            _lastRelativeLevel = Enums.eEnhRelative.Even;
            Ui.View.RelLevel = Enums.eEnhRelative.Even;

            base.BringToFront();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            components = new Container();
            TTip = new ToolTip(components);
            BackColor = Color.Silver;
            Name = "I9Picker";
            var size = new Size(200, 300);
            Size = size;
        }

        private void I9PickerLoad(object sender, EventArgs e)
        {
            FullDraw();
        }

        private void SetBxSize()
        {
            if (_myBx.Size.Width != Width | _myBx.Size.Height != Height)
            {
                _myBx = new ExtendedBitmap(Width, Height);
            }

            _myBx.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            _myBx.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            _myBx.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            _myBx.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            _myBx.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        public override Font Font =>
            new(Fonts.Family("Noto Sans"), base.Font.Size, base.Font.Style, GraphicsUnit.Pixel);

        private void FullDraw(Point? mouseLocation = null)
        {
            const int levelBoxHeight = 46;

            // Ensure constant height of the level box line
            var rectBounds = GetRectBounds(4, _rows + 1);
            var h = Height - (rectBounds.Y + 2 + _nPad);
            if (h < levelBoxHeight)
            {
                Height += levelBoxHeight - h;
            }

            SetBxSize();
            DrawLayerLowest(mouseLocation ?? new Point(-1, -1));
            if (_hDraw == null)
            {
                return;
            }

            DrawLayerImages();

            /*var zoneNames = Enum.GetValues(typeof(ActiveZone)).Cast<ActiveZone>().ToList();
            var zoneColors = new[] {Color.LawnGreen, Color.Aqua, Color.OrangeRed, Color.Yellow};
            var k = 0;
            foreach (var z in zoneNames)
            {
                if (z == ActiveZone.None)
                {
                    continue;
                }

                _myBx.Graphics.DrawPath(new Pen(zoneColors[k++]), GetZonePath(z));
            }*/

            var graphics = CreateGraphics();
            var clipRect = new Rectangle(0, 0, Width, Height);
            I9PickerPaint(this, new PaintEventArgs(graphics, clipRect));
        }

        private void DrawLayerLowest(Point mouseLocation)
        {
            _myBx.Graphics.Clear(BackColor);

            DrawBorder();
            DrawHeaderBox(mouseLocation);
            DrawLevelBox();
            DrawHighlights();
            DrawTypeLine();
            DrawEnhBox();
            DrawTextBox();
        }

        private void DrawLayerImages()
        {
            DrawTypeImages();
            DrawEnhImages();
        }

        private void DrawHeaderBox(Point mouseLocation)
        {
            using var font = new Font(Font, FontStyle.Bold);
            using var brush = new SolidBrush(Color.White);
            var buttonTextBrush = new SolidBrush(Color.Gainsboro);
            using var highlightBrush = new SolidBrush(_cHighlight);
            using var pen = new Pen(ForeColor);
            using var buttonFont = new Font(Fonts.Family("Noto Sans"), 8f, FontStyle.Bold, GraphicsUnit.Pixel);
            Rectangle iRect = default;
            iRect.X = _nPad;
            iRect.Y = _nPad;
            checked
            {
                iRect.Width = _nSize * 5 + _nPad * 4;
                iRect.Height = (int) Math.Round(font.GetHeight(_myBx.Graphics));
                _headerHeight = iRect.Height + _nPad;
                var layoutRectangle = new RectangleF(iRect.X, iRect.Y, iRect.Width, iRect.Height);
                _myBx.Graphics.DrawRectangle(pen, Dilate(iRect));
                var buttonSize = layoutRectangle.Height - 4;
                var buttonLoc = new PointF(layoutRectangle.X + layoutRectangle.Width - 2 - buttonSize,
                    layoutRectangle.Y + 2);
                _buttonRectangle = new Rectangle(new Point((int) Math.Round(buttonLoc.X), (int) Math.Round(buttonLoc.Y)),
                    new Size((int) Math.Round(buttonSize), (int) Math.Round(buttonSize)));
                if (!MidsContext.Config.CloseEnhSelectPopupByMove)
                {
                    var hoveredButton = PointInRectangle(mouseLocation, _buttonRectangle);
                    if (hoveredButton)
                    {
                        _myBx.Graphics.FillRectangle(highlightBrush, buttonLoc.X, buttonLoc.Y, buttonSize, buttonSize);
                    }

                    _myBx.Graphics.DrawRectangle(pen, buttonLoc.X, buttonLoc.Y, buttonSize, buttonSize);
                    _myBx.Graphics.DrawString("X", buttonFont, hoveredButton ? brush : buttonTextBrush, buttonLoc.X + 2,
                        buttonLoc.Y);
                }

                if (_nPowerIdx <= -1)
                {
                    return;
                }

                _myBx.Graphics.DrawString($"Enhancing: {DatabaseAPI.Database.Power[_nPowerIdx].DisplayName}", font,
                    brush, layoutRectangle);
            }
        }

        private void DrawLevelBox()
        {
            var pen = new Pen(ForeColor);
            var font = new Font(Font, FontStyle.Bold);
            var brush = new SolidBrush(Color.White);
            Rectangle rectBounds;
            StringFormat stringFormat;
            RectangleF layoutRectangle;
            checked
            {
                rectBounds = GetRectBounds(4, _rows + 1);
                rectBounds.Y += 2;
                rectBounds.Height = Height - (rectBounds.Y + _nPad);
                _myBx.Graphics.DrawRectangle(pen, Dilate(rectBounds));
                stringFormat = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip)
                {
                    Alignment = StringAlignment.Center, Trimming = StringTrimming.None
                };
                layoutRectangle = new RectangleF(rectBounds.X, rectBounds.Y, rectBounds.Width, rectBounds.Height);
                _myBx.Graphics.DrawString("LVL", font, brush, layoutRectangle, stringFormat);
            }

            layoutRectangle.Y += font.GetHeight(_myBx.Graphics) + 3f;
            rectBounds.Height = checked(Height - (rectBounds.Y + _nPad));
            brush = new SolidBrush(ForeColor);
            font = new Font(Fonts.Family("Noto Sans"), 19f, FontStyle.Bold, GraphicsUnit.Pixel);
            var s = Ui.View.TabId == Enums.eType.InventO | Ui.View.TabId == Enums.eType.SetO
                ? $"{Ui.View.IoLevel}"
                : GetRelativeString(Ui.View.RelLevel);

            _myBx.Graphics.DrawString(s, font, brush, layoutRectangle, stringFormat);
        }

        private static string GetRelativeString(Enums.eEnhRelative iRel)
        {
            var result = iRel switch
            {
                0 => "X",
                Enums.eEnhRelative.MinusThree => "-3",
                Enums.eEnhRelative.MinusTwo => "-2",
                Enums.eEnhRelative.MinusOne => "-1",
                Enums.eEnhRelative.Even => "+/-",
                Enums.eEnhRelative.PlusOne => "+1",
                Enums.eEnhRelative.PlusTwo => "+2",
                Enums.eEnhRelative.PlusThree => "+3",
                Enums.eEnhRelative.PlusFour => "+4",
                Enums.eEnhRelative.PlusFive => "+5",
                _ => "!"
            };

            return result;
        }

        private void DrawTextBox()
        {
            using var brush = new SolidBrush(Color.White);
            using var pen = new Pen(ForeColor);
            Rectangle iRect = default;
            iRect.X = _nPad;
            RectangleF layoutRectangle = default;
            RectangleF layoutRectangle2 = default;
            checked
            {
                iRect.Y = _headerHeight + _nPad * 2 + _nSize + _nSize * _rows + _nPad * (_rows - 1) + 2 + _nPad;
                iRect.Height = Height - (iRect.Y + _nPad);
                iRect.Width = _nSize * 4 + _nPad * 3;
                layoutRectangle.X = iRect.X;
                layoutRectangle.Y = iRect.Y;
                layoutRectangle.Width = iRect.Width;
                layoutRectangle.Height = Font.GetHeight(_myBx.Graphics);
                layoutRectangle2.X = iRect.X;
            }

            layoutRectangle2.Y = iRect.Y + layoutRectangle.Height;
            layoutRectangle2.Width = iRect.Width;
            layoutRectangle2.Height = iRect.Height - layoutRectangle.Height;
            _myBx.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            var fontScale = 1f;
            var num2 = checked((int) Math.Round(_myBx.Graphics
                .MeasureString(_hoverTitle, Font, (int) Math.Round(layoutRectangle.Width * 10f)).Width));
            if (num2 > layoutRectangle.Width - 10f)
            {
                fontScale = Math.Max(0.5f, (layoutRectangle.Width - 10f) / num2);
            }

            using var font = new Font(Name, Font.Size * fontScale, FontStyle.Bold, Font.Unit, 0);
            _myBx.Graphics.DrawRectangle(pen, Dilate(iRect));
            if (!string.IsNullOrWhiteSpace(_hoverTitle))
            {
                _myBx.Graphics.DrawString(_hoverTitle, font, brush, layoutRectangle);
            }

            if (!string.IsNullOrWhiteSpace(_hoverText))
            {
                _myBx.Graphics.DrawString(_hoverText, Font, brush, layoutRectangle2);
            }
        }

        private void SetInfoStrings(string title, string message)
        {
            _hoverTitle = title;
            _hoverText = message;
        }

        private void DrawTypeImages()
        {
            var srcRect = new Rectangle(0, 0, _nSize, _nSize);
            Enums.eType eType = 0;
            var rectBounds = GetRectBounds(0, 0);
            checked
            {
                srcRect.X = (int) eType * _nSize;
                _myBx.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, rectBounds, srcRect.X, srcRect.Y, 30, 30,
                    GraphicsUnit.Pixel, _hDraw.pImageAttributes);

                eType = Enums.eType.Normal;
                rectBounds = GetRectBounds(1, 0);
                srcRect.X = (int) eType * _nSize;
                _myBx.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, rectBounds, srcRect, GraphicsUnit.Pixel);

                eType = Enums.eType.InventO;
                rectBounds = GetRectBounds(2, 0);
                srcRect.X = (int) eType * _nSize;
                _myBx.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, rectBounds, srcRect, GraphicsUnit.Pixel);

                eType = Enums.eType.SpecialO;
                rectBounds = GetRectBounds(3, 0);
                srcRect.X = (int) eType * _nSize;
                _myBx.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, rectBounds, srcRect, GraphicsUnit.Pixel);

                eType = Enums.eType.SetO;
                rectBounds = GetRectBounds(4, 0);
                srcRect.X = (int) eType * _nSize;
                _myBx.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, rectBounds, srcRect, GraphicsUnit.Pixel);
            }
        }

        private void DrawEnhImages()
        {
            checked
            {
                _enhUniqueStatus = new List<EnhUniqueStatus?>();
                switch (Ui.View.TabId)
                {
                    case Enums.eType.Normal:
                    {
                        var num = 1;
                        for (var i = Ui.NoGrades.Length - 1; i >= 1; i -= 1)
                        {
                            var srcRect = new Rectangle(Ui.NoGrades[i] * _nSize, 0, _nSize, _nSize);
                            _myBx.Graphics.DrawImage(I9Gfx.Borders.Bitmap, GetRectBounds(4, num),
                                I9Gfx.GetOverlayRect(I9Gfx.ToGfxGrade((Enums.eType) 1, (Enums.eEnhGrade) i)),
                                GraphicsUnit.Pixel);
                            _myBx.Graphics.DrawImage(I9Gfx.EnhGrades.Bitmap, GetRectBounds(4, num), srcRect,
                                GraphicsUnit.Pixel);
                            num++;
                        }

                        for (var i = 0; i < Ui.No.Length; i++)
                        {
                            var grade = Ui.View.GradeId switch
                            {
                                Enums.eEnhGrade.TrainingO => (Origin.Grade) 0,
                                Enums.eEnhGrade.DualO => Origin.Grade.DualO,
                                Enums.eEnhGrade.SingleO => Origin.Grade.SingleO,
                                _ => Origin.Grade.SingleO
                            };

                            var graphics = _myBx.Graphics;
                            I9Gfx.DrawEnhancementAt(ref graphics, GetRectBounds(IndexToXy(i)), Ui.No[i], grade);
                        }

                        break;
                    }
                    case Enums.eType.InventO:
                    {
                        for (var i = 0; i < Ui.Io.Length; i++)
                        {
                            var graphics = _myBx.Graphics;
                            I9Gfx.DrawEnhancementAt(ref graphics, GetRectBounds(IndexToXy(i)), Ui.Io[i],
                                (Origin.Grade) 4);
                        }

                        break;
                    }
                    case Enums.eType.SpecialO:
                    {
                        for (var i = 1; i < Ui.SpecialTypes.Length; i++)
                        {
                            var srcRect2 = new Rectangle(Ui.SpecialTypes[i] * _nSize, 0, _nSize, _nSize);
                            _myBx.Graphics.DrawImage(I9Gfx.EnhSpecials.Bitmap, GetRectBounds(4, i), srcRect2,
                                GraphicsUnit.Pixel);
                        }

                        for (var i = 0; i < Ui.SpecialO.Length; i++)
                        {
                            var graphics = _myBx.Graphics;
                            I9Gfx.DrawEnhancementAt(ref graphics, GetRectBounds(IndexToXy(i)), Ui.SpecialO[i],
                                (Origin.Grade) 3);
                        }

                        break;
                    }
                    case Enums.eType.SetO:
                    {
                        for (var i = 0; i < Ui.SetTypes.Length; i++)
                        {
                            var srcRect3 = new Rectangle(Ui.SetTypes[i] * _nSize, 0, _nSize, _nSize);
                            _myBx.Graphics.DrawImage(I9Gfx.SetTypes.Bitmap, GetRectBounds(4, i + 1), srcRect3,
                                GraphicsUnit.Pixel);
                        }

                        if (Ui.View.SetId > -1)
                        {
                            DisplaySetEnhancements();
                        }
                        else
                        {
                            DisplaySetImages();
                        }

                        break;
                    }
                }
            }
        }

        private static ImageAttributes GreyItem(bool grey)
        {
            checked
            {
                if (!grey)
                {
                    return new ImageAttributes();
                }

                var colorMatrix = new ColorMatrix(clsDrawX.heroMatrix);
                var r = 0;
                do
                {
                    var c = 0;
                    do
                    {
                        if (r != 4)
                        {
                            ColorMatrix colorMatrix2;
                            int row;
                            int column;
                            (colorMatrix2 = colorMatrix)[row = r, column = c] = colorMatrix2[row, column] / 2f;
                        }

                        c++;
                    } while (c <= 2);

                    r++;
                } while (r <= 2);

                var imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix);

                return imageAttributes;
            }
        }

        private void DisplaySetEnhancements()
        {
            checked
            {
                for (var i = 0; i < DatabaseAPI.Database.EnhancementSets[Ui.Sets[Ui.View.SetTypeId][Ui.View.SetId]].Enhancements.Length; i++)
                {
                    var enh = DatabaseAPI.Database.EnhancementSets[Ui.Sets[Ui.View.SetTypeId][Ui.View.SetId]].Enhancements[i];
                    var enhData = DatabaseAPI.Database.Enhancements[enh];
                    _enhUniqueStatus.Add(!enhData.Unique
                        ? null
                        : new EnhUniqueStatus
                        {
                            InMain = _mySlotted.Any(slotted => enh == slotted) || MidsContext.Character.CurrentBuild
                                .Powers
                                .Where(e => e is {Power.Slottable: true})
                                .Any(f => f.Slots.Any(g => g.Enhancement.Enh == enh)),
                            InAlternate = _mySlotted.Any(slotted => enh == slotted) || MidsContext.Character.CurrentBuild
                                .Powers
                                .Where(e => e is { Power.Slottable: true })
                                .Any(f => f.Slots.Any(g => g.FlippedEnhancement.Enh == enh))
                        });
                    var grey = _mySlotted.All(slotted => enh != slotted) && enhData.Unique &&
                               (_enhUniqueStatus[i] == null
                                   ? false
                                   : _enhUniqueStatus[i].Value.InMain);
                    var graphics = _myBx.Graphics;
                    I9Gfx.DrawEnhancementAt(ref graphics, GetRectBounds(IndexToXy(i)),
                        DatabaseAPI.Database.EnhancementSets[Ui.Sets[Ui.View.SetTypeId][Ui.View.SetId]].Enhancements[i],
                        (Origin.Grade) 5, GreyItem(grey));
                }
            }
        }

        private bool IsPlaced(int index)
        {
            if (Ui.View.TabId != Enums.eType.SetO)
            {
                return false;
            }

            if (Ui.View.SetId < 0)
            {
                return false;
            }

            if (Ui.View.SetTypeId >= Ui.Sets.Length)
            {
                return false;
            }

            if (Ui.View.SetId >= Ui.Sets[Ui.View.SetTypeId].Length)
            {
                return false;
            }

            if (Ui.Sets[Ui.View.SetTypeId][Ui.View.SetId] >= DatabaseAPI.Database.EnhancementSets.Count)
            {
                return false;
            }

            if (index >= DatabaseAPI.Database.EnhancementSets[Ui.Sets[Ui.View.SetTypeId][Ui.View.SetId]]
                    .Enhancements.Length)
            {
                return false;
            }

            if (Ui.Initial.SetTypeId == Ui.View.SetTypeId & Ui.Initial.SetId == Ui.View.SetId &
                Ui.Initial.PickerId == index)
            {
                return false;
            }

            return _mySlotted.Any(t =>
                DatabaseAPI.Database.EnhancementSets[Ui.Sets[Ui.View.SetTypeId][Ui.View.SetId]]
                    .Enhancements[index] == t);
        }

        private void DisplaySetImages()
        {
            checked
            {
                if (Ui.View.TabId != Enums.eType.SetO)
                {
                    return;
                }

                if (Ui.View.SetTypeId <= -1)
                {
                    return;
                }

                for (var i = 0; i < Ui.Sets[Ui.View.SetTypeId].Length; i++)
                {
                    var srcRect = new Rectangle(I9Gfx.OriginIndex * _nSize, 4 * _nSize, _nSize, _nSize);
                    _myBx.Graphics.DrawImage(I9Gfx.Borders.Bitmap, GetRectBounds(IndexToXy(i)), srcRect,
                        GraphicsUnit.Pixel);

                    srcRect = new Rectangle(Ui.Sets[Ui.View.SetTypeId][i] * _nSize, 0, _nSize, _nSize);
                    _myBx.Graphics.DrawImage(I9Gfx.Sets.Bitmap, GetRectBounds(IndexToXy(i)), srcRect,
                        GraphicsUnit.Pixel);
                }
            }
        }

        private void DrawBorder()
        {
            using var pen = new Pen(ForeColor);
            checked
            {
                var rect = new Rectangle(new Point(1, 1), new Size(Width - 2, Height - 2));
                _myBx.Graphics.DrawRectangle(pen, rect);
            }
        }

        private void DrawHighlights()
        {
            DrawHighlight(_hoverCell.X, _hoverCell.Y);
            DrawSelected((int) Ui.View.TabId, 0);
            switch (Ui.View.TabId)
            {
                case Enums.eType.SetO:
                {
                    if (Ui.View.SetTypeId > -1)
                    {
                        DrawSelected(4, checked(Ui.View.SetTypeId + 1));
                    }

                    break;
                }
                case Enums.eType.Normal:
                {
                    if ((int) Ui.View.GradeId > -1)
                    {
                        DrawSelected(4, Reverse((int) Ui.View.GradeId));
                    }

                    break;
                }
                case Enums.eType.SpecialO when (int) Ui.View.SpecialId > -1:
                    DrawSelected(4, (int) Ui.View.SpecialId);
                    break;
            }

            if (Ui.Initial.PickerId <= -1)
            {
                return;
            }

            if ((Ui.Initial.TabId == Ui.View.TabId) & (Ui.Initial.SetTypeId == Ui.View.SetTypeId) &
                (Ui.Initial.SetId == Ui.View.SetId) & (Ui.Initial.GradeId == Ui.View.GradeId) &
                (Ui.Initial.SpecialId == Ui.View.SpecialId))
            {
                DrawSelected(IndexToXy(Ui.Initial.PickerId).X, IndexToXy(Ui.Initial.PickerId).Y);
                DrawBox(IndexToXy(Ui.Initial.PickerId).X, IndexToXy(Ui.Initial.PickerId).Y);
            }
            else if (Ui.Initial.TabId == Ui.View.TabId && Ui.Initial.SetTypeId == Ui.View.SetTypeId &&
                     Ui.View.SetId < 0)
            {
                DrawSelected(IndexToXy(Ui.Initial.SetId).X, IndexToXy(Ui.Initial.SetId).Y);
                DrawBox(IndexToXy(Ui.Initial.SetId).X, IndexToXy(Ui.Initial.SetId).Y);
            }
        }

        private GraphicsPath RectangleToPath(Rectangle rect)
        {
            var gp = new GraphicsPath();
            gp.AddRectangle(rect);

            return gp;
        }

        private GraphicsPath? GetZonePath(ActiveZone zone)
        {
            // Enh types
            var rectBounds = GetRectBounds(0, 0);
            var rectBounds2 = GetRectBounds(4, 0);
            var enhTypesPath = new GraphicsPath();
            enhTypesPath.AddRectangle(new Rectangle(rectBounds.Left, rectBounds.Top,
                rectBounds2.Right - rectBounds.Left, rectBounds.Bottom - rectBounds.Top));

            // Enhancement set
            rectBounds = GetRectBounds(4, 1);
            rectBounds.Height = _nSize * _rows + _nPad * (_rows - 1);
            var enhSetsPath = new GraphicsPath();
            enhSetsPath.AddRectangle(rectBounds);

            // Enhancements
            const int enhIconSize = 30;
            const int enhPerRow = 4;
            var enhPath = new GraphicsPath();
            var nbEnh = Ui.View.TabId switch
            {
                Enums.eType.Normal => Ui.No.Length,
                Enums.eType.InventO => Ui.Io.Length,
                Enums.eType.SpecialO => Ui.SpecialO.Length,
                Enums.eType.SetO => Ui.SetO.Length,
                Enums.eType.None => 0,
                _ => throw new ArgumentOutOfRangeException()
            };

            var enhFullRows = (int)Math.Floor(nbEnh / (decimal)enhPerRow);
            var enhBoxRect = new Rectangle(_nPad, _headerHeight + _nPad * 2 + _nSize, _nSize * 4 + _nPad * 3, _nSize * _rows + _nPad * (_rows - 1));
            if (enhFullRows > 0)
            {
                var rectRows = enhBoxRect with
                {
                    Height = enhIconSize * enhFullRows + _nPad * (enhFullRows - 1)
                };
                enhPath.AddRectangle(rectRows);

                if (nbEnh % enhPerRow > 0)
                {
                    enhPath.AddRectangle(new Rectangle(enhBoxRect.X, rectRows.Bottom, enhIconSize * (nbEnh % enhPerRow) + _nPad * (nbEnh % enhPerRow - 1), enhIconSize + _nPad));
                }
            }
            else
            {
                enhPath.AddRectangle(enhBoxRect with
                {
                    Width = enhIconSize * (nbEnh % enhPerRow) + _nPad * (nbEnh % enhPerRow - 1),
                    Height = enhIconSize + _nPad
                });
            }

            return zone switch
            {
                ActiveZone.CloseButton => RectangleToPath(_buttonRectangle),
                ActiveZone.EnhType => enhTypesPath,
                ActiveZone.EnhancementSet => enhSetsPath,
                ActiveZone.Enhancement => enhPath,
                _ => null
            };
        }

        private void DrawTypeLine()
        {
            var rectBounds = GetRectBounds((int)Ui.View.TabId, 0);
            checked
            {
                var ly1 = _headerHeight + _nPad + _nSize;
                var y = ly1 + _nPad - 2;
                var ly2 = (int)Math.Round(rectBounds.X + rectBounds.Width / 2.0);
                using var pen = new Pen(ForeColor);
                _myBx.Graphics.DrawLine(pen, ly2, ly1, ly2, y);
            }
        }

        private void DrawSetLine()
        {
            checked
            {
                switch (Ui.View.TabId)
                {
                    case Enums.eType.SetO when Ui.View.SetTypeId < 0:
                    case Enums.eType.Normal when Ui.View.GradeId < 0:
                    case Enums.eType.SpecialO when Ui.View.SpecialId < 0:
                        return;
                }

                var rectangle = Ui.View.TabId switch
                {
                    Enums.eType.Normal => GetRectBounds(4, Reverse((int)Ui.View.GradeId)),
                    Enums.eType.SpecialO => GetRectBounds(4, Ui.View.SpecialId),
                    Enums.eType.SetO => GetRectBounds(4, Ui.View.SetTypeId + 1),
                    _ => default
                };

                var ly = (int)Math.Round(rectangle.Y + rectangle.Height / 2.0);
                var x = rectangle.X - 2;
                var x2 = rectangle.X - _nPad + 2;
                using var pen = new Pen(ForeColor);
                _myBx.Graphics.DrawLine(pen, x, ly, x2, ly);
            }
        }

        private void DrawEnhBox()
        {
            DrawSetBox();
            DrawSetLine();
            DrawEnhBoxSet();
            DrawBox((int)Ui.View.TabId, 0);
        }

        private void DrawEnhBoxSet()
        {
            using var pen = new Pen(ForeColor);
            Rectangle iRect = default;
            iRect.X = _nPad;
            checked
            {
                iRect.Y = _headerHeight + _nPad * 2 + _nSize;
                iRect.Width = _nSize * 4 + _nPad * 3;
                iRect.Height = _nSize * _rows + _nPad * (_rows -1);
                _myBx.Graphics.DrawRectangle(pen, Dilate(iRect));
            }
        }

        private void DrawSetBox()
        {
            var rectBounds = GetRectBounds(4, 1);
            using var pen = new Pen(ForeColor);
            rectBounds.Height = checked(_nSize * _rows + _nPad * (_rows - 1));
            _myBx.Graphics.DrawRectangle(pen, Dilate(rectBounds));
        }

        private void DrawHighlight(int x, int y)
        {
            if (x > -1 && y > -1)
            {
                _myBx.Graphics.FillRectangle(new SolidBrush(_cHighlight), GetRectBounds(x, y));
            }
        }

        private void DrawSelected(int x, int y)
        {
            if (x > -1 && y > -1)
            {
                _myBx.Graphics.FillRectangle(new SolidBrush(_cSelected), GetRectBounds(x, y));
            }
        }

        private void DrawBox(int x, int y)
        {
            checked
            {
                if (!(x > -1 & y > -1))
                {
                    return;
                }

                using var pen = new Pen(ForeColor);
                var rectBounds = GetRectBounds(x, y);
                rectBounds.X--;
                rectBounds.Y--;
                rectBounds.Width++;
                rectBounds.Height++;
                _myBx.Graphics.DrawRectangle(pen, rectBounds);
            }
        }

        private static Rectangle Dilate(Rectangle iRect, int extra = 2)
        {
            return checked(new Rectangle(iRect.X - extra, iRect.Y - extra, iRect.Width + extra + 1,
                iRect.Height + extra + 1));
        }

        private Point IndexToXy(int index)
        {
            var tCols = 4;
            var tRow = 0;
            checked
            {
                while (index >= tCols)
                {
                    index -= tCols;
                    tRow++;
                }

                var result = new Point(index, tRow + 1);
                return result;
            }
        }

        private Rectangle GetRectBounds(int x, int y)
        {
            return checked(new Rectangle(_nPad + x * (_nSize + _nPad), _headerHeight + _nPad + y * (_nSize + _nPad), _nSize, _nSize));
        }

        private Rectangle GetRectBounds(Point iPoint)
        {
            return GetRectBounds(iPoint.X, iPoint.Y);
        }

        private void I9PickerPaint(object sender, PaintEventArgs e)
        {
            if (_myBx.Bitmap != null)
            {
                e.Graphics.DrawImage(_myBx.Bitmap, e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle, GraphicsUnit.Pixel);
            }
        }

        private Point GetCellXy(Point iPt)
        {
            checked
            {
                for (var x = 0; x <= 4; x++)
                {
                    var rows = _rows;
                    for (var y = 0; y <= rows; y++)
                    {
                        var rectBounds = GetRectBounds(x, y);
                        if ((iPt.X >= rectBounds.X) & (iPt.X <= rectBounds.X + rectBounds.Width) &&
                            (iPt.Y >= rectBounds.Y) & (iPt.Y <= rectBounds.Y + rectBounds.Height))
                        {
                            return new Point(x, y);
                        }
                    }
                }

                return new Point(-1, -1);
            }
        }

        private int GetCellIndex(Point cell)
        {
            if ((cell.X < 0) | (cell.Y <= 0))
            {
                return -1;
            }

            return checked((cell.Y - 1) * 4 + cell.X);
        }

        private void I9PickerMouseDown(object sender, MouseEventArgs e)
        {
            var iPt = new Point(e.X, e.Y);
            if (!MidsContext.Config.CloseEnhSelectPopupByMove && PointInRectangle(iPt, _buttonRectangle))
            {
                EnhancementSelectionCancelled?.Invoke();

                return;
            }

            var cellXy = GetCellXy(iPt);
            var cellIndex = GetCellIndex(cellXy);
            checked
            {
                _mouseOffset = new Point(0 - e.X, 0 - e.Y);
                if (e.Button == MouseButtons.Right)
                {
                    return;
                }

                if (cellXy.Y == 0)
                {
                    Ui.View.TabId = (Enums.eType)cellXy.X;
                    switch (Ui.View.TabId)
                    {
                        case Enums.eType.SetO:
                            if (Ui.SetTypes.Length > 3)
                            {
                                _rows = Ui.SetTypes.Length;
                                Height = 100 + (_nSize + 2 + _nPad) * Ui.SetTypes.Length;
                            }
                            break;
                        case Enums.eType.SpecialO:
                            if (Ui.SpecialO.Length > 9)
                            {
                                _rows = 5;
                                Height = (_nSize + 2 + _nPad) * (Ui.SpecialO.Length / 4);
                            }
                            break;
                        default:
                            _rows = 5;
                            Height = 315;
                            break;
                    }

                    if (cellXy.X == 4 & Ui.SetTypes.Length > 0)
                    {
                        if (Ui.View.SetTypeId != 4)
                        {
                            Ui.View.SetTypeId = 0;

                            if (Ui.View.RelLevel < Enums.eEnhRelative.Even)
                            {
                                Ui.View.RelLevel = Enums.eEnhRelative.Even;
                            }
                        }
                    }
                    else
                    {
                        Ui.View.RelLevel = Enums.eEnhRelative.Even;
                        switch (cellXy.X)
                        {
                            case 2 when Ui.Initial.TabId == Enums.eType.Normal:
                                Ui.Initial.TabId = Enums.eType.InventO;

                                break;
                            case 1 when Ui.Initial.TabId == Enums.eType.InventO:
                                Ui.Initial.TabId = Enums.eType.Normal;

                                break;
                        }
                    }

                    if (cellXy.X == 0)
                    {
                        DoEnhancementPicked(-1);
                    }
                }
                else if (cellXy.Y > 0)
                {
                    if (CellSetTypeSelect(cellXy))
                    {
                        switch (Ui.View.TabId)
                        {
                            case Enums.eType.Normal:
                                if (cellXy.Y < Ui.NoGrades.Length)
                                {
                                    cellXy.Y = Reverse(cellXy.Y);
                                    Ui.View.GradeId = (Enums.eEnhGrade)cellXy.Y;
                                    if (Ui.Initial.TabId == Enums.eType.Normal)
                                    {
                                        Ui.Initial.GradeId = Ui.View.GradeId;
                                    }

                                    DoHover(_hoverCell, true);
                                }

                                break;
                            case Enums.eType.SpecialO:
                                if (cellXy.Y < Ui.SpecialTypes.Length)
                                {
                                    SetSpecialEnhSet(cellXy.Y);
                                    DoHover(_hoverCell, true);
                                }

                                break;
                            case Enums.eType.SetO:
                                if (cellXy.Y - 1 < Ui.SetTypes.Length)
                                {
                                    Ui.View.SetTypeId = cellXy.Y - 1;
                                    Ui.View.SetId = -1;
                                    DoHover(_hoverCell, true);
                                }

                                break;
                        }
                    }
                    else if (CellSetSelect(cellIndex))
                    {
                        Ui.View.RelLevel = Enums.eEnhRelative.Even;
                        Ui.View.SetId = cellIndex;
                        CheckAndFixIoLevel();
                        DoHover(_hoverCell, true);
                    }
                    else if (CellEnhSelect(cellIndex))
                    {
                        var enhValid = DoEnhancementPicked(cellIndex);
                        if (!enhValid)
                        {
                            return;
                        }
                    }
                }

                FullDraw();
            }
        }

        private static int Reverse(int iValue)
        {
            return iValue switch
            {
                3 => 1,
                1 => 3,
                _ => iValue
            };
        }

        private bool CellSetTypeSelect(Point cell)
        {
            return cell.X == 4;
        }

        private bool CellSetSelect(int cellIdx)
        {
            return Ui.View.SetTypeId >= 0 && (Ui.View.TabId == Enums.eType.SetO) & (Ui.View.SetId == -1) & (cellIdx > -1) & (cellIdx < Ui.Sets[Ui.View.SetTypeId].Length);
        }

        private bool CellEnhSelect(int cellIdx)
        {
            if (cellIdx <= -1 || Ui.View.TabId == Enums.eType.SetO && Ui.View.SetId <= -1)
            {
                return false;
            }

            var array = Array.Empty<int>();
            switch (Ui.View.TabId)
            {
                case Enums.eType.Normal:
                    array = Ui.No;
                    break;
                case Enums.eType.InventO:
                    array = Ui.Io;
                    break;
                case Enums.eType.SpecialO:
                    array = Ui.SpecialO;
                    break;
                case Enums.eType.SetO:
                    if (Ui.View.SetTypeId < 0)
                    {
                        return false;
                    }

                    array = DatabaseAPI.Database.EnhancementSets[Ui.Sets[Ui.View.SetTypeId][Ui.View.SetId]].Enhancements;
                    break;
            }

            return (cellIdx > -1) & (cellIdx < array.Length);
        }

        private bool DoEnhancementPicked(int index)
        {
            var i9Slot = (I9Slot)_mySlot.Clone();
            CheckAndFixIoLevel();
            checked
            {
                if (Ui.View.IoLevel != Ui.Initial.IoLevel & Ui.View.IoLevel != _userLevel | _userLevel == -1 && !(Ui.View.TabId == Enums.eType.InventO & Enhancement.GranularLevelZb(_userLevel - 1, 9, 49) == Ui.View.IoLevel))
                {
                    _levelCapped = true;
                }

                switch (Ui.View.TabId)
                {
                    case 0:
                        i9Slot = new I9Slot();
                        break;
                    case Enums.eType.Normal:
                        i9Slot.Enh = Ui.No[index];
                        i9Slot.Grade = Ui.View.GradeId;
                        i9Slot.RelativeLevel = Ui.View.RelLevel;
                        break;
                    case Enums.eType.InventO:
                        i9Slot.Enh = Ui.Io[index];
                        i9Slot.IOLevel = Ui.View.IoLevel - 1;
                        i9Slot.RelativeLevel = Ui.View.RelLevel;
                        break;
                    case Enums.eType.SpecialO:
                        i9Slot.Enh = Ui.SpecialO[index];
                        i9Slot.RelativeLevel = Ui.View.RelLevel;
                        _lastSpecial = Ui.View.SpecialId;
                        break;
                    case Enums.eType.SetO:
                        if (IsPlaced(index))
                        {
                            return true;
                        }

                        i9Slot.Enh = DatabaseAPI.Database.EnhancementSets[Ui.Sets[Ui.View.SetTypeId][Ui.View.SetId]].Enhancements[index];
                        if (DatabaseAPI.Database.Enhancements[i9Slot.Enh].Unique)
                        {
                            var uniqueSlotted = MidsContext.Character.CurrentBuild.Powers
                                .Where(e => e is {Power.Slottable: true})
                                .Any(f => f.Slots.Any(g => g.Enhancement.Enh == i9Slot.Enh));

                            if (uniqueSlotted)
                            {
                                return false;
                            }
                        }

                        i9Slot.IOLevel = Ui.View.IoLevel - 1;
                        if (DatabaseAPI.Database.Enhancements[i9Slot.Enh].GetPower() is { } power)
                        {
                            if (power.BoostBoostable)
                            {
                                i9Slot.RelativeLevel = Ui.View.RelLevel;
                            }
                        }
                        else
                        {
                            i9Slot.RelativeLevel = Ui.View.RelLevel;
                        }

                        break;
                }

                if (Ui.View.TabId != 0)
                {
                    _lastTab = Ui.View.TabId;
                }

                switch (Ui.View.TabId)
                {
                    case Enums.eType.SetO:
                        _lastSet = Ui.View.SetTypeId;
                        break;
                    case Enums.eType.Normal:
                        _lastGrade = Ui.View.GradeId;
                        _lastRelativeLevel = Ui.View.RelLevel;
                        break;
                    case Enums.eType.SpecialO:
                        _lastSpecial = Ui.View.SpecialId;
                        _lastRelativeLevel = Ui.View.RelLevel;
                        break;
                }

                if ((Ui.View.TabId == Enums.eType.SetO | Ui.View.TabId == Enums.eType.InventO) & !_levelCapped)
                {
                    if (Ui.View.TabId == Enums.eType.InventO & Enhancement.GranularLevelZb(_userLevel - 1, 9, 49) == Ui.View.IoLevel)
                    {
                        LastLevel = _userLevel;
                    }
                    else
                    {
                        LastLevel = Ui.View.IoLevel;
                    }
                }
                else if (_userLevel > -1)
                {
                    LastLevel = _userLevel;
                }

                EnhancementPicked?.Invoke(i9Slot);

                return true;
            }
        }

        private void RaiseHoverEnhancement(int e, EnhUniqueStatus? enhUniqueStatus)
        {
            HoverEnhancement?.Invoke(e, enhUniqueStatus);
        }

        private void RaiseHoverSet(int e)
        {
            HoverSet?.Invoke(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            FullDraw();
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            FullDraw();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            FullDraw();
        }

        private void I9PickerMouseMove(object sender, MouseEventArgs e)
        {
            var location = new Point(e.X, e.Y);
            var cellXy = GetCellXy(location);
            var zoneNames = Enum.GetValues(typeof(ActiveZone)).Cast<ActiveZone>().ToList();
            var zones = zoneNames.Select(GetZonePath).ToList();
            var k = 0;
            var inZones = zones.ToDictionary(z => zoneNames[k++], z => z?.IsVisible(location) ?? false);
            var inActiveZone = inZones.DefaultIfEmpty(new KeyValuePair<ActiveZone, bool>(ActiveZone.None, false)).FirstOrDefault(z => z.Value).Key;
            if (!MidsContext.Config.CloseEnhSelectPopupByMove && PointInRectangle(location, _buttonRectangle))
            {
                SetInfoStrings("Cancel", "Close Picker");
                FullDraw(location);

                return;
            }
            
            if (cellXy is {X: < 0, Y: < 0} && !inZones.Any(z => z.Value))
            {
                SetInfoStrings("", "");
                FullDraw();
            }
            else
            {
                DoHover(cellXy);
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            var point = new Point(e.X, e.Y);
            point.Offset(_mouseOffset.X, _mouseOffset.Y);
            location = Location;
            var location2 = checked(new Point(location.X + point.X, Location.Y + point.Y));
            var bounds = Bounds;
            Location = location2;
            Moved?.Invoke(Bounds, bounds);
        }

        private static bool PointInRectangle(Point point, Rectangle rect)
        {
            return point.X >= rect.X && point.X <= rect.X + rect.Width &&
                   point.Y >= rect.Y && point.Y <= rect.Y + rect.Height;
        }

        private void DoHover(Point cell, bool alwaysUpdate = false)
        {
            var cellIndex = GetCellIndex(cell);
            var hlOk = false;
            checked
            {
                if (cell.Y == 0)
                {
                    switch (cell.X)
                    {
                        case 0:
                            SetInfoStrings("No Enhancement", "");
                            break;
                        case 1:
                            SetInfoStrings("Regular Enhancement", "Training, Dual and Single Origin Types");
                            break;
                        case 2:
                            SetInfoStrings("Invention", "Crafted from salvage");
                            break;
                        case 3:
                            SetInfoStrings("Special", "These can have multiple effects");
                            break;
                        case 4:
                            SetInfoStrings("Invention Set", "Collecting a set will grant additional effects");
                            break;
                    }

                    hlOk = true;
                }
                else if (cell.Y > 0)
                {
                    if (CellSetTypeSelect(cell))
                    {
                        switch (Ui.View.TabId)
                        {
                            case Enums.eType.Normal:
                                if (cell.Y < Ui.NoGrades.Length)
                                {
                                    var message = "";
                                    var num = cell.Y;
                                    num = num switch
                                    {
                                        1 => 3,
                                        3 => 1,
                                        _ => num
                                    };

                                    message = num switch
                                    {
                                        0 => "This is not an enhancement grade!",
                                        1 => "This is the weakest type of enhancement.",
                                        2 => "Half as effective as Single Origin.",
                                        3 => "The most effective regular enhancement.",
                                        _ => message
                                    };

                                    SetInfoStrings(DatabaseAPI.Database.EnhGradeStringLong[Ui.NoGrades[num]], message);
                                    hlOk = true;
                                }

                                break;
                            case Enums.eType.SpecialO:
                                if (cell.Y < Ui.SpecialTypes.Length)
                                {
                                    SetInfoStrings(DatabaseAPI.GetSpecialEnhByIndex(Ui.SpecialTypes[cell.Y]).Name, DatabaseAPI.GetSpecialEnhByIndex(Ui.SpecialTypes[cell.Y]).Description);
                                    hlOk = true;
                                }

                                break;
                            case Enums.eType.SetO:
                                if (cell.Y - 1 < Ui.SetTypes.Length)
                                {
                                    SetInfoStrings(DatabaseAPI.GetSetTypeByIndex(Ui.SetTypes[cell.Y - 1]).Name, "Click here to view the set listing.");
                                    hlOk = true;
                                }

                                break;
                        }
                    }
                    else if (CellSetSelect(cellIndex))
                    {
                        var tId = Ui.Sets[Ui.View.SetTypeId][cellIndex];
                        var str = DatabaseAPI.Database.EnhancementSets[tId].LevelMin == DatabaseAPI.Database.EnhancementSets[tId].LevelMax
                            ? $" ({DatabaseAPI.Database.EnhancementSets[tId].LevelMin + 1})"
                            : $" ({DatabaseAPI.Database.EnhancementSets[tId].LevelMin + 1}-{DatabaseAPI.Database.EnhancementSets[tId].LevelMax + 1})";

                        SetInfoStrings(DatabaseAPI.Database.EnhancementSets[tId].DisplayName + str, $"Type: {DatabaseAPI.GetSetTypeByIndex(DatabaseAPI.Database.EnhancementSets[tId].SetType).ShortName}");
                        //SetInfoStrings(DatabaseAPI.Database.EnhancementSets[tId].DisplayName + str, "Type: " + setTypeStringLong[(int)DatabaseAPI.Database.EnhancementSets[tId].SetType]);
                        if ((cell.X != _hoverCell.X) | (cell.Y != _hoverCell.Y) || alwaysUpdate)
                        {
                            RaiseHoverSet(tId);
                        }

                        hlOk = true;
                    }
                    else if (CellEnhSelect(cellIndex))
                    {
                        var tId = 0;
                        switch (Ui.View.TabId)
                        {
                            case 0:
                                tId = -1;
                                _selectedEnhancement = null;
                                SetInfoStrings("", "");
                                break;
                            case Enums.eType.Normal:
                                tId = Ui.No[cellIndex];
                                _selectedEnhancement = DatabaseAPI.Database.Enhancements[tId];
                                SetInfoStrings(DatabaseAPI.Database.Enhancements[tId].Name, DatabaseAPI.Database.Enhancements[tId].Desc);
                                break;
                            case Enums.eType.InventO:
                                tId = Ui.Io[cellIndex];
                                _selectedEnhancement = DatabaseAPI.Database.Enhancements[tId];
                                SetInfoStrings(DatabaseAPI.Database.Enhancements[tId].Name, DatabaseAPI.Database.Enhancements[tId].Desc);
                                break;
                            case Enums.eType.SpecialO:
                                tId = Ui.SpecialO[cellIndex];
                                _selectedEnhancement = DatabaseAPI.Database.Enhancements[tId];
                                SetInfoStrings(DatabaseAPI.Database.Enhancements[tId].Name, DatabaseAPI.Database.Enhancements[tId].Desc);
                                break;
                            case Enums.eType.SetO:
                            {
                                tId = DatabaseAPI.Database.EnhancementSets[Ui.Sets[Ui.View.SetTypeId][Ui.View.SetId]].Enhancements[cellIndex];
                                _selectedEnhancement = DatabaseAPI.Database.Enhancements[tId];
                                var text = DatabaseAPI.Database.Enhancements[tId].Name;
                                string str2;
                                if (DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[tId].nIDSet].LevelMin == DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[tId].nIDSet].LevelMax)
                                {
                                    str2 = $" ({DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[tId].nIDSet].LevelMin + 1})";
                                }
                                else
                                {
                                    str2 = $" ({DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[tId].nIDSet].LevelMin + 1}-{DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[tId].nIDSet].LevelMax + 1})";
                                    if (DatabaseAPI.Database.Enhancements[tId].Unique)
                                    {
                                        text += " (Unique)";
                                    }
                                }

                                SetInfoStrings(DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[tId].nIDSet].DisplayName + str2, text);
                                break;
                            }
                        }

                        if (cell.X != _hoverCell.X || cell.Y != _hoverCell.Y || alwaysUpdate)
                        {
                            RaiseHoverEnhancement(tId, _enhUniqueStatus != null && _enhUniqueStatus.Count > cellIndex ? _enhUniqueStatus[cellIndex] : null);
                        }

                        hlOk = true;
                    }
                }

                if (!hlOk)
                {
                    cell = new Point(-1, -1);
                }

                if (cell.X == _hoverCell.X && cell.Y == _hoverCell.Y)
                {
                    return;
                }

                _hoverCell = cell;
                FullDraw();
            }
        }

        private int SetIdGlobalToLocal(int iId)
        {
            checked
            {
                for (var i = 0; i < Ui.SetTypes.Length; i++)
                for (var j = 0; j < Ui.Sets[i].Length; j++)
                {
                    if (Ui.Sets[i][j] == iId)
                    {
                        return j;
                    }
                }

                return 0;
            }
        }

        private static int[] GetSets(int iSetType)
        {
            var list = new List<int>();
            var num = 0;
            checked
            {
                for (var i = num; i < DatabaseAPI.Database.EnhancementSets.Count; i++)
                {
                    if (DatabaseAPI.Database.EnhancementSets[i].SetType == iSetType)
                    {
                        list.Add(i);
                    }
                }

                return list.ToArray();
            }
        }

        private static int[] GetValidSetTypes(int iPowerIdx)
        {
            if (iPowerIdx < 0)
            {
                return Array.Empty<int>();
            }

            return DatabaseAPI.Database.Power[iPowerIdx].SetTypes.ToArray();
        }

        private static List<int> GetValidEnhancements(int iPowerIdx, Enums.eType iType, int iSubType = 0)
        {
            return iPowerIdx < 0 ? new List<int>() : DatabaseAPI.Database.Power[iPowerIdx].GetValidEnhancements(iType, iSubType);
        }

        public void SetData(int iPower, ref I9Slot iSlot, ref clsDrawX iDraw, int[] slotted)
        {
            TimerReset();
            _levelCapped = false;
            _userLevel = -1;
            _hDraw = iDraw;
            _mySlot = (I9Slot)iSlot.Clone();
            _hoverCell = new Point(-1, -1);
            _hoverText = "";
            _hoverTitle = "";
            _nPowerIdx = iPower;
            Ui = new CTracking
            {
                SpecialTypes = DatabaseAPI.Database.SpecialEnhancements.Select(x => x.Index).ToArray()
            };
            Enums.eEnhGrade eEnhGrade = 0;
            Ui.NoGrades = (int[])Enum.GetValues(eEnhGrade.GetType());

            Ui.No = GetValidEnhancements(_nPowerIdx, Enums.eType.Normal).ToArray();
            Ui.Io = GetValidEnhancements(_nPowerIdx, Enums.eType.InventO).ToArray();
            Ui.Initial.GradeId = _lastGrade;
            Ui.Initial.RelLevel = _lastRelativeLevel;
            Ui.Initial.SpecialId = _lastSpecial;
            if (Ui.Initial.SpecialId == 0)
            {
                Ui.Initial.SpecialId = 1;
            }

            if (Ui.Initial.GradeId == 0)
            {
                Ui.Initial.GradeId = MidsContext.Config.CalcEnhOrigin;
            }

            if (Ui.Initial.RelLevel == 0)
            {
                Ui.Initial.RelLevel = MidsContext.Config.CalcEnhLevel;
            }

            if (_mySlot.Enh > -1)
            {
                if (DatabaseAPI.Database.Enhancements[_mySlot.Enh].SubTypeID != 0)
                {
                    Ui.SpecialO = GetValidEnhancements(_nPowerIdx, Enums.eType.SpecialO, DatabaseAPI.Database.Enhancements[_mySlot.Enh].SubTypeID).ToArray();
                    Ui.Initial.SpecialId = DatabaseAPI.Database.Enhancements[_mySlot.Enh].SubTypeID;
                }
                else
                {
                    Ui.SpecialO = GetValidEnhancements(_nPowerIdx, Enums.eType.SpecialO, Ui.Initial.SpecialId).ToArray();
                }
            }
            else
            {
                Ui.SpecialO = GetValidEnhancements(_nPowerIdx, Enums.eType.SpecialO, Ui.Initial.SpecialId).ToArray();
            }

            Ui.SetTypes = GetValidSetTypes(_nPowerIdx);
            checked
            {
                Ui.Sets = new int[Ui.SetTypes.Length][];
                for (var i = 0; i <= Ui.SetTypes.Length - 1; i++)
                {
                    Ui.Sets[i] = GetSets(Ui.SetTypes[i]);
                }

                if (_mySlot.Grade != 0)
                {
                    Ui.Initial.GradeId = _mySlot.Grade;
                }

                if (_mySlot.Enh > -1)
                {
                    Ui.Initial.RelLevel = _mySlot.RelativeLevel;
                }
                else
                {
                    _mySlot.RelativeLevel = _lastRelativeLevel;
                    _mySlot.Grade = _lastGrade;
                }

                if (_mySlot.Enh > -1)
                {
                    var power = DatabaseAPI.Database.Enhancements[_mySlot.Enh].GetPower();
                    if (power is { BoostBoostable: false })
                    {
                        _mySlot.RelativeLevel = Enums.eEnhRelative.Even;
                    }

                    if (_nPowerIdx < 0)
                    {
                        switch (DatabaseAPI.Database.Enhancements[_mySlot.Enh].TypeID)
                        {
                            case Enums.eType.Normal:
                                Ui.No = new int[1];
                                Ui.No[0] = _mySlot.Enh;
                                break;
                            case Enums.eType.InventO:
                                Ui.Io = new int[1];
                                Ui.Io[0] = _mySlot.Enh;
                                break;
                            case Enums.eType.SpecialO:
                                Ui.SpecialO = new int[1];
                                Ui.SpecialO[0] = _mySlot.Enh;
                                break;
                            case Enums.eType.SetO:
                            {
                                Ui.SetTypes = new int[1];
                                Ui.SetTypes[0] = DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[_mySlot.Enh].nIDSet].SetType;
                                Ui.Sets = new int[1][];
                                var array = new int[1];
                                Ui.Sets[0] = array;
                                Ui.Sets[0][0] = DatabaseAPI.Database.Enhancements[_mySlot.Enh].nIDSet;
                                Ui.SetO = new int[1];
                                Ui.SetO[0] = _mySlot.Enh;
                                break;
                            }
                        }
                    }

                    Ui.Initial.TabId = DatabaseAPI.Database.Enhancements[_mySlot.Enh].TypeID;
                    if (Ui.Initial.TabId == Enums.eType.SetO)
                    {
                        Ui.Initial.SetId = SetIdGlobalToLocal(DatabaseAPI.Database.Enhancements[_mySlot.Enh].nIDSet);
                        Ui.Initial.SetTypeId = SetTypeToId(DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[_mySlot.Enh].nIDSet].SetType);
                        Ui.SetO = new int[DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[_mySlot.Enh].nIDSet].Enhancements.Length - 1 + 1];
                        Array.Copy(DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[_mySlot.Enh].nIDSet].Enhancements, Ui.SetO, Ui.SetO.Length);
                    }
                    else if (Ui.SetTypes.Length > 0)
                    {
                        Ui.Initial.SetTypeId = 0;
                    }

                    Ui.Initial.PickerId = GetPickerIndex(_mySlot.Enh, Ui.Initial.TabId);
                }
                else
                {
                    Ui.Initial.TabId = 0;
                }

                if (Ui.Initial.TabId is Enums.eType.InventO or Enums.eType.SetO)
                {
                    Ui.Initial.IoLevel = _mySlot.IOLevel + 1;
                }
                else if (LastLevel > 0)
                {
                    Ui.Initial.IoLevel = LastLevel;
                }
                else
                {
                    Ui.Initial.IoLevel = MidsContext.Config.I9.DefaultIOLevel + 1;
                }

                Ui.View = new CTracking.CLocation(Ui.Initial);
                if (Ui.View.TabId == 0)
                {
                    Ui.View.TabId = _lastTab != 0 ? _lastTab : Enums.eType.Normal;
                    if ((Ui.View.TabId == Enums.eType.SetO) & (Ui.SetTypes.Length > _lastSet))
                    {
                        Ui.View.SetTypeId = _lastSet;
                    }
                }

                _mySlotted = new int[slotted.Length - 1 + 1];
                Array.Copy(slotted, _mySlotted, _mySlotted.Length);
                if (Ui.SetTypes.Length > 3)
                {
                    _rows = Ui.SetTypes.Length;
                    Height = 100 + (_nSize + 2 + _nPad) * (Ui.SetTypes.Length);
                }
                else if (Ui.SpecialO.Length > 9)
                {
                    _rows = 5;
                    Height = (_nSize + 2 + _nPad) * (Ui.SpecialO.Length / 4);
                }   
                else
                {
                    _rows = 5;
                    Height = 315;
                }
                FullDraw();
            }
        }

        private void SetSpecialEnhSet(int iSubType)
        {
            Ui.View.SpecialId = iSubType;
            if (_nPowerIdx > 0)
            {
                Ui.SpecialO = GetValidEnhancements(_nPowerIdx, Enums.eType.SpecialO, Ui.View.SpecialId).ToArray();
            }
            else if ((Ui.Initial.SpecialId == Ui.View.SpecialId) & ((int)Ui.Initial.TabId == 3))
            {
                Ui.SpecialO = new int[1];
                Ui.SpecialO[0] = _mySlot.Enh;
            }
            else
            {
                Ui.SpecialO = Array.Empty<int>();
            }
        }

        private int SetTypeToId(int iSetType)
        {
            checked
            {
                for (var i = 0; i <= Ui.SetTypes.Length - 1; i++)
                {
                    if (iSetType == Ui.SetTypes[i])
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        private int GetPickerIndex(int index, Enums.eType iType)
        {
            var array = iType switch
            {
                Enums.eType.Normal => Ui.No,
                Enums.eType.InventO => Ui.Io,
                Enums.eType.SpecialO => Ui.SpecialO,
                Enums.eType.SetO => Ui.SetO,
                _ => Array.Empty<int>()
            };

            checked
            {
                for (var i = 0; i <= array.Length - 1; i++)
                {
                    if (array[i] == index)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        private void CheckAndFixIoLevel()
        {
            var ioMax = IoMax;
            var ioMin = 10;
            checked
            {
                switch (Ui.View.TabId)
                {
                    case Enums.eType.InventO:
                    {
                        if (Ui.Initial.TabId == Ui.View.TabId && Ui.Initial.PickerId == Ui.View.PickerId &&
                            Ui.View.PickerId > -1)
                        {
                            ioMax = DatabaseAPI.Database.Enhancements[Ui.Io[Ui.View.PickerId]].LevelMax + 1;
                            ioMin = DatabaseAPI.Database.Enhancements[Ui.Io[Ui.View.PickerId]].LevelMin + 1;
                        }

                        break;
                    }
                    case Enums.eType.SetO when Ui.View.SetId > -1 && Ui.View.SetTypeId > -1:
                    {
                        var enhSetMax = DatabaseAPI.Database.EnhancementSets[Ui.Sets[Ui.View.SetTypeId][Ui.View.SetId]].LevelMax + 1;
                        ioMax = enhSetMax;
                        var enhSetMin = DatabaseAPI.Database.EnhancementSets[Ui.Sets[Ui.View.SetTypeId][Ui.View.SetId]].LevelMin + 1;
                        ioMin = enhSetMin;
                        break;
                    }
                }

                if (Ui.View.IoLevel > ioMax)
                {
                    Ui.View.IoLevel = ioMax;
                }

                if (Ui.View.IoLevel < ioMin)
                {
                    Ui.View.IoLevel = ioMin;
                }

                if (Ui.View.TabId != Enums.eType.InventO)
                {
                    return;
                }

                if (ioMax > IoMax)
                {
                    ioMax = IoMax;
                }

                var nextLevel = Enhancement.GranularLevelZb(Ui.View.IoLevel - 1, ioMin - 1, ioMax - 1) + 1;
                Ui.View.IoLevel = nextLevel;
            }
        }

        public int CheckAndReturnIoLevel()
        {
            var ioMax = IoMax;
            var ioMin = 10;
            var fixedLevel = Ui.View.IoLevel;
            checked
            {
                switch (Ui.View.TabId)
                {
                    case Enums.eType.InventO:
                    {
                        if ((Ui.Initial.TabId == Ui.View.TabId) & (Ui.Initial.PickerId == Ui.View.PickerId) & (Ui.View.PickerId > -1))
                        {
                            ioMax = DatabaseAPI.Database.Enhancements[Ui.Io[Ui.View.PickerId]].LevelMax + 1;
                            ioMin = DatabaseAPI.Database.Enhancements[Ui.Io[Ui.View.PickerId]].LevelMin + 1;
                        }

                        break;
                    }
                    case Enums.eType.SetO when (Ui.View.SetId > -1) & (Ui.View.SetTypeId > -1):
                        ioMax = DatabaseAPI.Database.EnhancementSets[Ui.Sets[Ui.View.SetTypeId][Ui.View.SetId]].LevelMax + 1;
                        ioMin = DatabaseAPI.Database.EnhancementSets[Ui.Sets[Ui.View.SetTypeId][Ui.View.SetId]].LevelMin + 1;
                        break;
                }

                if (fixedLevel > ioMax)
                {
                    fixedLevel = ioMax;
                }

                if (fixedLevel < ioMin)
                {
                    fixedLevel = ioMin;
                }

                if (Ui.View.TabId != Enums.eType.InventO)
                {
                    return fixedLevel;
                }

                if (ioMax > 50)
                {
                    ioMax = 50;
                }

                fixedLevel = Enhancement.GranularLevelZb(fixedLevel - 1, ioMin - 1, ioMax - 1) + 1;
                return fixedLevel;
            }
        }

        private void TimerReset()
        {
            _timerLastKeypress = -1;
        }

        private void NumberPressed(int iNumber)
        {
            var timer = DateAndTime.Timer;
            if (timer - _timerLastKeypress > 5)
            {
                Ui.View.IoLevel = iNumber;
                _userLevel = Ui.View.IoLevel;
                _timerLastKeypress = timer;
            }
            else
            {
                var text = $"{Ui.View.IoLevel}{iNumber}";
                checked
                {
                    if (text.Length > 2)
                    {
                        text = text.Substring(text.Length - 2);
                    }

                    var ret = float.TryParse(text, out var txtNum);
                    var number = Math.Max(1, Math.Min(50, (int)Math.Round(!ret ? 0 : txtNum)));

                    Ui.View.IoLevel = number;
                    _userLevel = Ui.View.IoLevel;
                    if (number >= 10)
                    {
                        TimerReset();
                    }
                }
            }
        }

        private void I9PickerKeyDown(object sender, KeyEventArgs e)
        {
            var num = -1;
            var keyCode = e.KeyCode;
            checked
            {
                switch (keyCode)
                {
                    case Keys.Return:
                        Ui.Initial.IoLevel = Ui.View.IoLevel;
                        Ui.Initial.RelLevel = Ui.View.RelLevel;
                        Ui.View = new CTracking.CLocation(Ui.Initial);
                        DoEnhancementPicked(Ui.View.PickerId);
                        break;
                    case Keys.Back:
                        Ui.View.IoLevel = Ui.Initial.IoLevel;
                        Ui.View.RelLevel = Ui.Initial.RelLevel;
                        TimerReset();
                        break;
                    case Keys.Delete:
                        Ui.View.IoLevel = Ui.Initial.IoLevel;
                        Ui.View.RelLevel = Ui.Initial.RelLevel;
                        TimerReset();
                        break;
                    case Keys.Decimal:
                        Ui.View.IoLevel = Ui.Initial.IoLevel;
                        Ui.View.RelLevel = Ui.Initial.RelLevel;
                        TimerReset();
                        break;
                    case Keys.Home:
                        Ui.View.IoLevel = 0;
                        Ui.View.RelLevel = Enums.eEnhRelative.MinusThree;
                        TimerReset();
                        break;
                    case Keys.End:
                        Ui.View.IoLevel = 100;
                        Ui.View.RelLevel = Enums.eEnhRelative.PlusFive;
                        TimerReset();
                        break;
                    case Keys.Space:
                        Ui.View.IoLevel = MidsContext.Config.I9.DefaultIOLevel + 1;
                        Ui.View.RelLevel = Enums.eEnhRelative.Even;
                        TimerReset();
                        break;
                    case Keys.Add:
                        RelAdjust(1);
                        break;
                    case Keys.Subtract:
                        RelAdjust(-1);
                        break;
                    case Keys.Oemplus:
                        RelAdjust(1);
                        break;
                    case Keys.OemMinus:
                        RelAdjust(-1);
                        break;
                    default:
                    {
                        if ((e.KeyValue >= 48) & (e.KeyValue <= 57))
                        {
                            num = e.KeyValue - 48;
                        }
                        else if ((e.KeyValue >= 96) & (e.KeyValue <= 105))
                        {
                            num = e.KeyValue - 96;
                        }

                        break;
                    }
                }

                if (Ui.View.TabId is Enums.eType.SetO or Enums.eType.InventO)
                {
                    if (num > -1)
                    {
                        NumberPressed(num);
                    }
                    else
                    {
                        CheckAndFixIoLevel();
                    }
                }

                DoHover(_hoverCell, true);
                FullDraw();
            }
        }

        private void RelAdjust(int direction)
        {
            var eEnhRelative = Ui.View.RelLevel;
            checked
            {
                if (direction < 0)
                {
                    if (Ui.View.RelLevel > 0)
                    {
                        eEnhRelative--;
                    }
                }
                else if (direction > 0 && (int)Ui.View.RelLevel < 9)
                {
                    eEnhRelative++;
                }

                var specialOLimit = AllowPlusThreeSpecialO ? Enums.eEnhRelative.PlusThree : Enums.eEnhRelative.PlusTwo;

                if ((DatabaseAPI.EnhHasCatalyst(_selectedEnhancement.UID) || DatabaseAPI.EnhIsNaturallyAttuned(Array.IndexOf(DatabaseAPI.Database.Enhancements, _selectedEnhancement))))
                {
                    eEnhRelative = Enums.eEnhRelative.Even;
                }
                else if ((eEnhRelative > Enums.eEnhRelative.PlusThree) & (Ui.View.TabId == Enums.eType.Normal))
                {
                    eEnhRelative = Enums.eEnhRelative.PlusThree;
                }
                else if ((eEnhRelative > specialOLimit) & (Ui.View.TabId == Enums.eType.SpecialO))
                {
                    eEnhRelative = specialOLimit;
                }
                else if ((eEnhRelative < Enums.eEnhRelative.Even) & ((Ui.View.TabId == Enums.eType.InventO) | (Ui.View.TabId == Enums.eType.SetO)))
                {
                    eEnhRelative = Enums.eEnhRelative.Even;
                }

                Ui.View.RelLevel = eEnhRelative;
            }
        }

        public class CTracking
        {
            public readonly CLocation Initial;
            public int[] Io;

            public int[] No;
            public int[] NoGrades;
            public int[] SetO;
            public int[][] Sets;
            public int[] SetTypes;
            public int[] SpecialO;
            public int[] SpecialTypes;
            public CLocation View;

            public CTracking()
            {
                No = Array.Empty<int>();
                Io = Array.Empty<int>();
                SpecialO = Array.Empty<int>();
                NoGrades = Array.Empty<int>();
                SpecialTypes = Array.Empty<int>();
                SetTypes = Array.Empty<int>();
                Sets = Array.Empty<int[]>();
                SetO = Array.Empty<int>();
                Initial = new CLocation();
                View = new CLocation();
            }

            public class CLocation
            {
                public Enums.eEnhGrade GradeId;
                public int IoLevel;
                public int PickerId;
                public Enums.eEnhRelative RelLevel;
                public int SetId;
                public int SetTypeId;
                public int SpecialId;

                public Enums.eType TabId;

                public CLocation()
                {
                    TabId = 0;
                    PickerId = -1;
                    SetTypeId = -1;
                    SetId = -1;
                    GradeId = 0;
                    SpecialId = 0;
                    IoLevel = 0;
                    RelLevel = Enums.eEnhRelative.Even;
                }

                public CLocation(CLocation iCl)
                {
                    TabId = 0;
                    PickerId = -1;
                    SetTypeId = -1;
                    SetId = -1;
                    GradeId = 0;
                    SpecialId = 0;
                    IoLevel = 0;
                    RelLevel = Enums.eEnhRelative.Even;
                    TabId = iCl.TabId;
                    PickerId = iCl.PickerId;
                    SetTypeId = iCl.SetTypeId;
                    GradeId = iCl.GradeId;
                    SpecialId = iCl.SpecialId;
                    SetId = iCl.SetId;
                    IoLevel = iCl.IoLevel;
                    RelLevel = iCl.RelLevel;
                }
            }
        }
    }
}