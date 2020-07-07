using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Base.Data_Classes;
using Base.Display;
using Base.Master_Classes;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace midsControls
{
    public class I9Picker : UserControl
    {
        public int LastLevel;

        public event EnhancementPickedEventHandler EnhancementPicked;
        public event HoverEnhancementEventHandler HoverEnhancement;
        public event HoverSetEventHandler HoverSet;
        public event MovedEventHandler Moved;

        const int IOMax = 50;
        const int Cols = 5;
        const int DilateVal = 2;
        const double timerResetDelay = 5.0;

        Point _mouseOffset;
        int _rows;
        clsDrawX _hDraw;
        I9Slot _mySlot;
        ExtendedBitmap _myBx;
        Point _hoverCell;
        string _hoverTitle;
        string _hoverText;
        int _headerHeight;
        int[] _mySlotted;
        bool _levelCapped;
        int _userLevel;
        int _lastSet;
        int _nPad;
        int _nSize;
        int _nPowerIDX;
        Enums.eType _lastTab;
        Enums.eEnhGrade _lastGrade;
        Enums.eEnhRelative _lastRelativeLevel;
        Enums.eSubtype _lastSpecial;
        double _timerLastKeypress;
        public cTracking UI;
        Color _cHighlight;
        Color _cSelected;

        private IEnhancement SelectedEnhancement = null;

        IContainer components;


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
            _lastSpecial = Enums.eSubtype.Hamidon;
            UI = new cTracking();
            _nPad = 8;
            _nSize = 30;
            _cHighlight = Color.SlateBlue;
            _cSelected = Color.BlueViolet;
            _nPowerIDX = -1;
            InitializeComponent();
        }

        public new void BringToFront()
        {
            _lastRelativeLevel = Enums.eEnhRelative.Even;
            UI.View.RelLevel = Enums.eEnhRelative.Even;

            base.BringToFront();
        }

        [field: AccessedThroughProperty("tTip")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private ToolTip tTip { get; set; }

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
                    value = 1;
                if (value > 256)
                    value = 256;
                _nSize = value;
                FullDraw();
            }
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
        void InitializeComponent()
        {
            components = new Container();
            tTip = new ToolTip(components);
            BackColor = Color.Silver;
            Name = "I9Picker";
            Size size = new Size(184, 244);
            Size = size;
        }

        // Token: 0x06000157 RID: 343 RVA: 0x0000B99D File Offset: 0x00009B9D
        void I9PickerLoad(object sender, EventArgs e)
        {
            FullDraw();
        }

        void SetBxSize()
        {
            if (_myBx == null)
            {
                _myBx = new ExtendedBitmap(Width, Height);
            }
            else if (_myBx.Size.Width != Width | _myBx.Size.Height != Height)
            {
                _myBx = new ExtendedBitmap(Width, Height);
            }

            _myBx.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        }

        void FullDraw()
        {
            SetBxSize();
            DrawLayerLowest();
            if (_hDraw == null)
                return;
            DrawLayerImages();
            Graphics graphics = CreateGraphics();
            Rectangle clipRect = new Rectangle(0, 0, Width, Height);
            I9PickerPaint(this, new PaintEventArgs(graphics, clipRect));
        }

        void DrawLayerLowest()
        {
            _myBx.Graphics.Clear(BackColor);
            DrawBorder();
            DrawHeaderBox();
            DrawLevelBox();
            DrawHighlights();
            DrawTypeLine();
            DrawEnhBox();
            DrawTextBox();
        }

        // Token: 0x0600015B RID: 347 RVA: 0x0000BB0A File Offset: 0x00009D0A
        void DrawLayerImages()
        {
            DrawTypeImages();
            DrawEnhImages();
        }

        // Token: 0x0600015C RID: 348 RVA: 0x0000BB1C File Offset: 0x00009D1C
        void DrawHeaderBox()
        {
            Font font = new Font(Font, FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.White);
            string text = "";
            Pen pen = new Pen(ForeColor);
            Rectangle iRect = default;
            iRect.X = _nPad;
            iRect.Y = _nPad;
            checked
            {
                iRect.Width = _nSize * 5 + _nPad * 4;
                iRect.Height = (int) Math.Round(font.GetHeight(_myBx.Graphics));
                _headerHeight = iRect.Height + _nPad;
                RectangleF layoutRectangle = new RectangleF(iRect.X, iRect.Y, iRect.Width, iRect.Height);
                _myBx.Graphics.DrawRectangle(pen, Dilate(iRect));
                if (_nPowerIDX > -1)
                {
                    text = "Enhancing: " + DatabaseAPI.Database.Power[_nPowerIDX].DisplayName;
                }

                if (Operators.CompareString(text, "", false) != 0)
                {
                    _myBx.Graphics.DrawString(text, font, brush, layoutRectangle);
                }
            }
        }

        // Token: 0x0600015D RID: 349 RVA: 0x0000BC6C File Offset: 0x00009E6C
        void DrawLevelBox()
        {
            Pen pen = new Pen(ForeColor);
            Font font = new Font(Font, FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.White);
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
            font = new Font("Arial", 19f, FontStyle.Bold, GraphicsUnit.Pixel);
            string s;
            if (UI.View.TabID == Enums.eType.InventO | UI.View.TabID == Enums.eType.SetO)
            {
                s = Convert.ToString(UI.View.IOLevel);
            }
            else
            {
                s = GetRelativeString(UI.View.RelLevel);
            }

            _myBx.Graphics.DrawString(s, font, brush, layoutRectangle, stringFormat);
        }

        static string GetRelativeString(Enums.eEnhRelative iRel)
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

        void DrawTextBox()
        {
            SolidBrush brush = new SolidBrush(Color.White);
            Pen pen = new Pen(ForeColor);
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
            float num = 1f;
            int num2 = checked((int) Math.Round(_myBx.Graphics.MeasureString(_hoverTitle, Font, (int) Math.Round(layoutRectangle.Width * 10f))
                .Width));
            if (num2 > layoutRectangle.Width - 10f)
            {
                num = (layoutRectangle.Width - 10f) / num2;
            }

            if (num < 0.5)
            {
                num = 0.5f;
            }

            Font font = new Font(Name, Font.Size * num, FontStyle.Bold, Font.Unit, 0);
            _myBx.Graphics.DrawRectangle(pen, Dilate(iRect));
            if (Operators.CompareString(_hoverTitle, "", false) != 0)
            {
                _myBx.Graphics.DrawString(_hoverTitle, font, brush, layoutRectangle);
            }

            if (Operators.CompareString(_hoverText, "", false) != 0)
            {
                _myBx.Graphics.DrawString(_hoverText, Font, brush, layoutRectangle2);
            }
        }

        // Token: 0x06000160 RID: 352 RVA: 0x0000C199 File Offset: 0x0000A399
        void SetInfoStrings(string title, string message)
        {
            _hoverTitle = title;
            _hoverText = message;
        }

        // Token: 0x06000161 RID: 353 RVA: 0x0000C1AC File Offset: 0x0000A3AC
        void DrawTypeImages()
        {
            Rectangle srcRect = new Rectangle(0, 0, _nSize, _nSize);
            Enums.eType eType = 0;
            Rectangle rectBounds = GetRectBounds(0, 0);
            checked
            {
                srcRect.X = (int) eType * _nSize;
                _myBx.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, rectBounds, srcRect.X, srcRect.Y, 30, 30, GraphicsUnit.Pixel,
                    _hDraw.pImageAttributes);
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

        void DrawEnhImages()
        {
            checked
            {
                switch (UI.View.TabID)
                {
                    case Enums.eType.Normal:
                    {
                        int num = 1;
                        for (int i = UI.NOGrades.Length - 1; i >= 1; i += -1)
                        {
                            Rectangle srcRect = new Rectangle(UI.NOGrades[i] * _nSize, 0, _nSize, _nSize);
                            _myBx.Graphics.DrawImage(I9Gfx.Borders.Bitmap, GetRectBounds(4, num),
                                I9Gfx.GetOverlayRect(I9Gfx.ToGfxGrade((Enums.eType) 1, (Enums.eEnhGrade) i)), GraphicsUnit.Pixel);
                            _myBx.Graphics.DrawImage(I9Gfx.EnhGrades.Bitmap, GetRectBounds(4, num), srcRect, GraphicsUnit.Pixel);
                            num++;
                        }

                        int num2 = 0;
                        int num3 = UI.NO.Length - 1;
                        for (int i = num2; i <= num3; i++)
                        {
                            var grade = UI.View.GradeID switch
                            {
                                Enums.eEnhGrade.TrainingO => (Origin.Grade) 0,
                                Enums.eEnhGrade.DualO => Origin.Grade.DualO,
                                Enums.eEnhGrade.SingleO => Origin.Grade.SingleO,
                                _ => Origin.Grade.SingleO
                            };

                            Graphics graphics = _myBx.Graphics;
                            I9Gfx.DrawEnhancementAt(ref graphics, GetRectBounds(IndexToXY(i)), UI.NO[i], grade);
                        }

                        break;
                    }
                    case Enums.eType.InventO:
                    {
                        for (int i = 0; i <= UI.IO.Length - 1; i++)
                        {
                            Graphics graphics = _myBx.Graphics;
                            I9Gfx.DrawEnhancementAt(ref graphics, GetRectBounds(IndexToXY(i)), UI.IO[i], (Origin.Grade) 4);
                        }

                        break;
                    }
                    case Enums.eType.SpecialO:
                    {
                        for (int i = 1; i <= UI.SpecialTypes.Length - 1; i++)
                        {
                            Rectangle srcRect2 = new Rectangle(UI.SpecialTypes[i] * _nSize, 0, _nSize, _nSize);
                            _myBx.Graphics.DrawImage(I9Gfx.EnhSpecials.Bitmap, GetRectBounds(4, i), srcRect2, GraphicsUnit.Pixel);
                        }

                        for (int i = 0; i <= UI.SpecialO.Length - 1; i++)
                        {
                            Graphics graphics = _myBx.Graphics;
                            I9Gfx.DrawEnhancementAt(ref graphics, GetRectBounds(IndexToXY(i)), UI.SpecialO[i], (Origin.Grade) 3);
                        }

                        break;
                    }
                    case Enums.eType.SetO:
                    {
                        for (int i = 0; i <= UI.SetTypes.Length - 1; i++)
                        {
                            Rectangle srcRect3 = new Rectangle(UI.SetTypes[i] * _nSize, 0, _nSize, _nSize);
                            _myBx.Graphics.DrawImage(I9Gfx.SetTypes.Bitmap, GetRectBounds(4, i + 1), srcRect3, GraphicsUnit.Pixel);
                        }

                        if (UI.View.SetID > -1)
                            DisplaySetEnhancements();
                        else
                            DisplaySetImages();
                        break;
                    }
                }
            }
        }

        static ImageAttributes GreyItem(bool grey)
        {
            checked
            {
                if (!grey)
                {
                    return new ImageAttributes();
                }

                ColorMatrix colorMatrix = new ColorMatrix(clsDrawX.heroMatrix);
                int r = 0;
                do
                {
                    int c = 0;
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

                ImageAttributes imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix);
                return imageAttributes;
            }
        }

        void DisplaySetEnhancements()
        {
            checked
            {
                for (int i = 0;
                    i <= DatabaseAPI.Database.EnhancementSets[UI.Sets[UI.View.SetTypeID][UI.View.SetID]].Enhancements.Length - 1;
                    i++)
                {
                    var enh = DatabaseAPI.Database.EnhancementSets[UI.Sets[UI.View.SetTypeID][UI.View.SetID]].Enhancements[i];
                    bool grey = _mySlotted.Any(slotted => enh == slotted);
                    Graphics graphics = _myBx.Graphics;
                    I9Gfx.DrawEnhancementAt(ref graphics, GetRectBounds(IndexToXY(i)),
                        DatabaseAPI.Database.EnhancementSets[UI.Sets[UI.View.SetTypeID][UI.View.SetID]].Enhancements[i], (Origin.Grade) 5,
                        GreyItem(grey));
                }
            }
        }

        bool IsPlaced(int index)
        {
            checked
            {
                // only check sets
                if (UI.View.TabID != Enums.eType.SetO) return false;
                // id must be set
                if (UI.View.SetID < 0) return false;
                // the set type must be valid
                if (UI.View.SetTypeID >= UI.Sets.Length) return false;
                // the id index must be inside the range of the set
                if (UI.View.SetID >= UI.Sets[UI.View.SetTypeID].Length) return false;

                if (UI.Sets[UI.View.SetTypeID][UI.View.SetID] >= DatabaseAPI.Database.EnhancementSets.Count) return false;
                if (index >= DatabaseAPI.Database.EnhancementSets[UI.Sets[UI.View.SetTypeID][UI.View.SetID]].Enhancements.Length) return false;
                if (UI.Initial.SetTypeID == UI.View.SetTypeID & UI.Initial.SetID == UI.View.SetID & UI.Initial.PickerID == index) return false;

                int num = 0;
                int num2 = _mySlotted.Length - 1;
                for (int i = num; i <= num2; i++)
                {
                    if (DatabaseAPI.Database.EnhancementSets[UI.Sets[UI.View.SetTypeID][UI.View.SetID]].Enhancements[index] == _mySlotted[i])
                        return true;
                }

                return false;
            }
        }

        void DisplaySetImages()
        {
            checked
            {
                if (UI.View.TabID != Enums.eType.SetO)
                    return;
                if (UI.View.SetTypeID <= -1)
                    return;
                int num = 0;
                int num2 = UI.Sets[UI.View.SetTypeID].Length - 1;
                for (int i = num; i <= num2; i++)
                {
                    Rectangle srcRect = new Rectangle(I9Gfx.OriginIndex * _nSize, 4 * _nSize, _nSize, _nSize);
                    _myBx.Graphics.DrawImage(I9Gfx.Borders.Bitmap, GetRectBounds(IndexToXY(i)), srcRect, GraphicsUnit.Pixel);
                    srcRect = new Rectangle(UI.Sets[UI.View.SetTypeID][i] * _nSize, 0, _nSize, _nSize);
                    _myBx.Graphics.DrawImage(I9Gfx.Sets.Bitmap, GetRectBounds(IndexToXY(i)), srcRect, GraphicsUnit.Pixel);
                }
            }
        }

        void DrawBorder()
        {
            Pen pen = new Pen(ForeColor);
            Rectangle rect = default;
            rect.X = 0;
            rect.Y = 0;
            checked
            {
                rect.Height = Height - 1;
                rect.Width = Width - 1;
                _myBx.Graphics.DrawRectangle(pen, rect);
            }
        }

        void DrawHighlights()
        {
            DrawHighlight(_hoverCell.X, _hoverCell.Y);
            DrawSelected((int) UI.View.TabID, 0);
            switch (UI.View.TabID)
            {
                case Enums.eType.SetO:
                {
                    if (UI.View.SetTypeID > -1)
                    {
                        DrawSelected(4, checked(UI.View.SetTypeID + 1));
                    }

                    break;
                }
                case Enums.eType.Normal:
                {
                    if ((int) UI.View.GradeID > -1)
                    {
                        DrawSelected(4, Reverse((int) UI.View.GradeID));
                    }

                    break;
                }
                case Enums.eType.SpecialO when (int) UI.View.SpecialID > -1:
                    DrawSelected(4, (int) UI.View.SpecialID);
                    break;
            }

            if (UI.Initial.PickerID <= -1)
                return;
            if (UI.Initial.TabID == UI.View.TabID & UI.Initial.SetTypeID == UI.View.SetTypeID & UI.Initial.SetID == UI.View.SetID &
                UI.Initial.GradeID == UI.View.GradeID & UI.Initial.SpecialID == UI.View.SpecialID)
            {
                DrawSelected(IndexToXY(UI.Initial.PickerID).X, IndexToXY(UI.Initial.PickerID).Y);
                DrawBox(IndexToXY(UI.Initial.PickerID).X, IndexToXY(UI.Initial.PickerID).Y);
            }
            else if (UI.Initial.TabID == UI.View.TabID && UI.Initial.SetTypeID == UI.View.SetTypeID && UI.View.SetID < 0)
            {
                DrawSelected(IndexToXY(UI.Initial.SetID).X, IndexToXY(UI.Initial.SetID).Y);
                DrawBox(IndexToXY(UI.Initial.SetID).X, IndexToXY(UI.Initial.SetID).Y);
            }
        }

        void DrawTypeLine()
        {
            Rectangle rectBounds = GetRectBounds((int) UI.View.TabID, 0);
            checked
            {
                int ly1 = _headerHeight + _nPad + _nSize;
                int y = ly1 + _nPad - 2;
                int ly2 = (int) Math.Round(rectBounds.X + rectBounds.Width / 2.0);
                Pen pen = new Pen(ForeColor);
                _myBx.Graphics.DrawLine(pen, ly2, ly1, ly2, y);
            }
        }

        void DrawSetLine()
        {
            checked
            {
                switch (UI.View.TabID)
                {
                    case Enums.eType.SetO when UI.View.SetTypeID < 0:
                    case Enums.eType.Normal when UI.View.GradeID < 0:
                    case Enums.eType.SpecialO when UI.View.SpecialID < 0:
                        return;
                }

                var rectangle = UI.View.TabID switch
                {
                    Enums.eType.Normal => GetRectBounds(4, Reverse((int) UI.View.GradeID)),
                    Enums.eType.SpecialO => GetRectBounds(4, (int) UI.View.SpecialID),
                    Enums.eType.SetO => GetRectBounds(4, UI.View.SetTypeID + 1),
                    _ => default
                };

                int ly = (int) Math.Round(rectangle.Y + rectangle.Height / 2.0);
                int x = rectangle.X - 2;
                int x2 = rectangle.X - _nPad + 2;
                Pen pen = new Pen(ForeColor);
                _myBx.Graphics.DrawLine(pen, x, ly, x2, ly);
            }
        }

        void DrawEnhBox()
        {
            DrawSetBox();
            DrawSetLine();
            DrawEnhBoxSet();
            DrawBox((int) UI.View.TabID, 0);
        }

        void DrawEnhBoxSet()
        {
            Pen pen = new Pen(ForeColor);
            Rectangle iRect = default;
            iRect.X = _nPad;
            checked
            {
                iRect.Y = _headerHeight + _nPad * 2 + _nSize;
                iRect.Width = _nSize * 4 + _nPad * 3;
                iRect.Height = _nSize * _rows + _nPad * (_rows - 1);
                _myBx.Graphics.DrawRectangle(pen, Dilate(iRect));
            }
        }

        void DrawSetBox()
        {
            Rectangle rectBounds = GetRectBounds(4, 1);
            Pen pen = new Pen(ForeColor);
            rectBounds.Height = checked(_nSize * _rows + _nPad * (_rows - 1));
            _myBx.Graphics.DrawRectangle(pen, Dilate(rectBounds));
        }

        void DrawHighlight(int x, int y)
        {
            if (x > -1 && y > -1)
                _myBx.Graphics.FillRectangle(new SolidBrush(_cHighlight), GetRectBounds(x, y));
        }

        void DrawSelected(int x, int y)
        {
            if (x > -1 && y > -1)
                _myBx.Graphics.FillRectangle(new SolidBrush(_cSelected), GetRectBounds(x, y));
        }

        void DrawBox(int x, int y)
        {
            checked
            {
                if (!(x > -1 & y > -1))
                    return;
                Pen pen = new Pen(ForeColor);
                Rectangle rectBounds = GetRectBounds(x, y);
                rectBounds.X--;
                rectBounds.Y--;
                rectBounds.Width++;
                rectBounds.Height++;
                _myBx.Graphics.DrawRectangle(pen, rectBounds);
            }
        }

        static Rectangle Dilate(Rectangle iRect, int extra = 2)
            => checked(new Rectangle(iRect.X - extra, iRect.Y - extra, iRect.Width + extra + 1, iRect.Height + extra + 1));

        Point IndexToXY(int index)
        {
            int tCols = 4;
            int tRow = 0;
            checked
            {
                while (index >= tCols)
                {
                    index -= tCols;
                    tRow++;
                }

                Point result = new Point(index, tRow + 1);
                return result;
            }
        }

        Rectangle GetRectBounds(int x, int y)
            => checked(new Rectangle(_nPad + x * (_nSize + _nPad), _headerHeight + _nPad + y * (_nSize + _nPad), _nSize, _nSize));

        Rectangle GetRectBounds(Point iPoint)
            => GetRectBounds(iPoint.X, iPoint.Y);

        void I9PickerPaint(object sender, PaintEventArgs e)
        {
            if (_myBx.Bitmap != null)
            {
                e.Graphics.DrawImage(_myBx.Bitmap, e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle, GraphicsUnit.Pixel);
            }
        }

        Point GetCellXY(Point iPt)
        {
            checked
            {
                for (int x = 0; x <= 4; x++)
                {
                    int rows = _rows;
                    for (int y = 0; y <= rows; y++)
                    {
                        Rectangle rectBounds = GetRectBounds(x, y);
                        if ((iPt.X >= rectBounds.X & iPt.X <= rectBounds.X + rectBounds.Width) &&
                            (iPt.Y >= rectBounds.Y & iPt.Y <= rectBounds.Y + rectBounds.Height))
                            return new Point(x, y);
                    }
                }

                return new Point(-1, -1);
            }
        }

        int GetCellIndex(Point cell)
        {
            if (cell.X < 0 | cell.Y <= 0) return -1;
            return checked((cell.Y - 1) * 4 + cell.X);
        }

        void I9PickerMouseDown(object sender, MouseEventArgs e)
        {
            Point iPt = new Point(e.X, e.Y);
            Point cellXY = GetCellXY(iPt);
            int cellIndex = GetCellIndex(cellXY);
            checked
            {
                _mouseOffset = new Point(0 - e.X, 0 - e.Y);
                if (e.Button == MouseButtons.Right)
                    return;

                if (cellXY.Y == 0)
                {
                    UI.View.TabID = (Enums.eType)cellXY.X;
                    if (cellXY.X == 4 & UI.SetTypes.Length > 0)
                    {
                        UI.View.SetTypeID = 0;
                        if (UI.View.RelLevel < Enums.eEnhRelative.Even)
                        {
                            UI.View.RelLevel = Enums.eEnhRelative.Even;
                        }
                    }
                    else
                    {
                        /*switch (cellXY.X)
                        {
                            case 2:
                            {
                                if (UI.Initial.TabID == Enums.eType.Normal)
                                    UI.Initial.TabID = Enums.eType.InventO;
                                if (UI.View.RelLevel < Enums.eEnhRelative.Even)
                                    UI.View.RelLevel = Enums.eEnhRelative.Even;
                                break;
                            }
                            case 1:
                            {
                                if (UI.View.RelLevel > Enums.eEnhRelative.PlusThree)
                                    UI.View.RelLevel = Enums.eEnhRelative.PlusThree;
                                if (UI.Initial.TabID == Enums.eType.InventO)
                                    UI.Initial.TabID = Enums.eType.Normal;
                                break;
                            }
                            case 3 when UI.View.RelLevel > Enums.eEnhRelative.PlusTwo:
                                UI.View.RelLevel = Enums.eEnhRelative.PlusTwo;
                                break;
                        }*/
                        UI.View.RelLevel = Enums.eEnhRelative.Even;
                        if (cellXY.X == 2)
                        {
                            if (UI.Initial.TabID == Enums.eType.Normal) UI.Initial.TabID = Enums.eType.InventO;
                        }
                        else if (cellXY.X == 1)
                        {
                            if (UI.Initial.TabID == Enums.eType.InventO) UI.Initial.TabID = Enums.eType.Normal;
                        }
                    }

                    if (cellXY.X == 0)
                    {
                        DoEnhancementPicked(-1);
                    }
                }
                else if (cellXY.Y > 0)
                {
                    if (CellSetTypeSelect(cellXY))
                    {
                        switch (UI.View.TabID)
                        {
                            case Enums.eType.Normal:
                                if (cellXY.Y < UI.NOGrades.Length)
                                {
                                    cellXY.Y = Reverse(cellXY.Y);
                                    UI.View.GradeID = (Enums.eEnhGrade) cellXY.Y;
                                    if (UI.Initial.TabID == Enums.eType.Normal)
                                    {
                                        UI.Initial.GradeID = UI.View.GradeID;
                                    }

                                    DoHover(_hoverCell, true);
                                }

                                break;
                            case Enums.eType.SpecialO:
                                if (cellXY.Y < UI.SpecialTypes.Length)
                                {
                                    SetSpecialEnhSet((Enums.eSubtype) cellXY.Y);
                                    DoHover(_hoverCell, true);
                                }

                                break;
                            case Enums.eType.SetO:
                                if (cellXY.Y - 1 < UI.SetTypes.Length)
                                {
                                    UI.View.SetTypeID = cellXY.Y - 1;
                                    UI.View.SetID = -1;
                                    DoHover(_hoverCell, true);
                                }

                                break;
                        }
                    }
                    else if (CellSetSelect(cellIndex))
                    {
                        UI.View.RelLevel = Enums.eEnhRelative.Even;
                        UI.View.SetID = cellIndex;
                        CheckAndFixIOLevel();
                        DoHover(_hoverCell, true);
                    }
                    else if (CellEnhSelect(cellIndex))
                        DoEnhancementPicked(cellIndex);
                }

                FullDraw();
            }
        }

        static int Reverse(int iValue)
        {
            return iValue switch
            {
                3 => 1,
                1 => 3,
                _ => iValue
            };
        }

        bool CellSetTypeSelect(Point cell)
            => cell.X == 4;

        bool CellSetSelect(int cellIDX)
            => UI.View.SetTypeID >= 0 && (UI.View.TabID == Enums.eType.SetO & UI.View.SetID == -1 & cellIDX > -1 &
                                          cellIDX < UI.Sets[UI.View.SetTypeID].Length);

        bool CellEnhSelect(int cellIDX)
        {
            if (cellIDX <= -1 || UI.View.TabID == Enums.eType.SetO && UI.View.SetID <= -1)
                return false;

            int[] array = Array.Empty<int>();
            switch (UI.View.TabID)
            {
                case Enums.eType.Normal:
                    array = UI.NO;
                    break;
                case Enums.eType.InventO:
                    array = UI.IO;
                    break;
                case Enums.eType.SpecialO:
                    array = UI.SpecialO;
                    break;
                case Enums.eType.SetO:
                    if (UI.View.SetTypeID < 0)
                        return false;
                    array = DatabaseAPI.Database.EnhancementSets[UI.Sets[UI.View.SetTypeID][UI.View.SetID]].Enhancements;
                    break;
            }

            return cellIDX > -1 & cellIDX < array.Length;
        }

        void DoEnhancementPicked(int index)
        {
            I9Slot i9Slot = (I9Slot) _mySlot.Clone();
            CheckAndFixIOLevel();
            checked
            {
                if (((UI.View.IOLevel != UI.Initial.IOLevel & UI.View.IOLevel != _userLevel) | _userLevel == -1) &&
                    !(UI.View.TabID == Enums.eType.InventO & Enhancement.GranularLevelZb(_userLevel - 1, 9, 49) == UI.View.IOLevel))
                    _levelCapped = true;
                switch (UI.View.TabID)
                {
                    case 0:
                        i9Slot = new I9Slot();
                        break;
                    case Enums.eType.Normal:
                        i9Slot.Enh = UI.NO[index];
                        i9Slot.Grade = UI.View.GradeID;
                        i9Slot.RelativeLevel = UI.View.RelLevel;
                        break;
                    case Enums.eType.InventO:
                        i9Slot.Enh = UI.IO[index];
                        i9Slot.IOLevel = UI.View.IOLevel - 1;
                        i9Slot.RelativeLevel = UI.View.RelLevel;
                        break;
                    case Enums.eType.SpecialO:
                        i9Slot.Enh = UI.SpecialO[index];
                        i9Slot.RelativeLevel = UI.View.RelLevel;
                        _lastSpecial = UI.View.SpecialID;
                        break;
                    case Enums.eType.SetO:
                        if (IsPlaced(index))
                            return;
                        i9Slot.Enh = DatabaseAPI.Database.EnhancementSets[UI.Sets[UI.View.SetTypeID][UI.View.SetID]].Enhancements[index];
                        i9Slot.IOLevel = UI.View.IOLevel - 1;
                        if (DatabaseAPI.Database.Enhancements[i9Slot.Enh].GetPower() is IPower power)
                        {
                            if (power.BoostBoostable)
                                i9Slot.RelativeLevel = UI.View.RelLevel;
                        }
                        else
                            i9Slot.RelativeLevel = UI.View.RelLevel;

                        break;
                }

                if (UI.View.TabID != 0)
                    _lastTab = UI.View.TabID;
                switch (UI.View.TabID)
                {
                    case Enums.eType.SetO:
                        _lastSet = UI.View.SetTypeID;
                        break;
                    case Enums.eType.Normal:
                        _lastGrade = UI.View.GradeID;
                        _lastRelativeLevel = UI.View.RelLevel;
                        break;
                    case Enums.eType.SpecialO:
                        _lastSpecial = UI.View.SpecialID;
                        _lastRelativeLevel = UI.View.RelLevel;
                        break;
                }

                if ((UI.View.TabID == Enums.eType.SetO | UI.View.TabID == Enums.eType.InventO) & !_levelCapped)
                {
                    if (UI.View.TabID == Enums.eType.InventO & Enhancement.GranularLevelZb(_userLevel - 1, 9, 49) == UI.View.IOLevel)
                        LastLevel = _userLevel;
                    else
                        LastLevel = UI.View.IOLevel;
                }
                else if (_userLevel > -1)
                {
                    LastLevel = _userLevel;
                }

                EnhancementPicked?.Invoke(i9Slot);
            }
        }

        void RaiseHoverEnhancement(int e)
            => HoverEnhancement?.Invoke(e);

        void RaiseHoverSet(int e)
            => HoverSet?.Invoke(e);

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

        void I9PickerMouseMove(object sender, MouseEventArgs e)
        {
            Point location = new Point(e.X, e.Y);
            Point cellXY = GetCellXY(location);
            DoHover(cellXY);
            if (e.Button != MouseButtons.Left)
                return;
            Point point = new Point(e.X, e.Y);
            point.Offset(_mouseOffset.X, _mouseOffset.Y);
            location = Location;
            Point location2 = checked(new Point(location.X + point.X, Location.Y + point.Y));
            Rectangle bounds = Bounds;
            Location = location2;
            Moved?.Invoke(Bounds, bounds);
        }

        void DoHover(Point cell, bool alwaysUpdate = false)
        {
            int cellIndex = GetCellIndex(cell);
            bool hlOk = false;
            string[] setTypeStringLong = DatabaseAPI.Database.SetTypeStringLong;
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
                            SetInfoStrings("Special/Hami-O", "These can have multiple effects");
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
                        switch (UI.View.TabID)
                        {
                            case Enums.eType.Normal:
                                if (cell.Y < UI.NOGrades.Length)
                                {
                                    string message = "";
                                    int num = cell.Y;
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
                                        2 => "Half as efective as Single Origin.",
                                        3 => "The most effective regular enhancement.",
                                        _ => message
                                    };

                                    SetInfoStrings(DatabaseAPI.Database.EnhGradeStringLong[UI.NOGrades[num]], message);
                                    hlOk = true;
                                }

                                break;
                            case Enums.eType.SpecialO:
                                if (cell.Y < UI.SpecialTypes.Length)
                                {
                                    var message2 = cell.Y switch
                                    {
                                        0 => "This is not an enhancement subtype!",
                                        1 => "Hamidon/Synthetic Hamidon enhancements.",
                                        2 => "Rewards from the Sewer Trial.",
                                        3 => "Rewards from the Eden Trial.",
                                        _ => ""
                                    };

                                    SetInfoStrings(DatabaseAPI.Database.SpecialEnhStringLong[UI.SpecialTypes[cell.Y]], message2);
                                    hlOk = true;
                                }

                                break;
                            case Enums.eType.SetO:
                                if (cell.Y - 1 < UI.SetTypes.Length)
                                {
                                    SetInfoStrings(setTypeStringLong[UI.SetTypes[cell.Y - 1]], "Click here to view the set listing.");
                                    hlOk = true;
                                }

                                break;
                        }
                    }
                    else if (CellSetSelect(cellIndex))
                    {
                        int tId = UI.Sets[UI.View.SetTypeID][cellIndex];
                        string str;
                        if (DatabaseAPI.Database.EnhancementSets[tId].LevelMin == DatabaseAPI.Database.EnhancementSets[tId].LevelMax)
                        {
                            str = " (" + Convert.ToString(DatabaseAPI.Database.EnhancementSets[tId].LevelMin + 1) + ")";
                        }
                        else
                        {
                            str = string.Concat(" (", Convert.ToString(DatabaseAPI.Database.EnhancementSets[tId].LevelMin + 1), "-",
                                Convert.ToString(DatabaseAPI.Database.EnhancementSets[tId].LevelMax + 1), ")");
                        }

                        SetInfoStrings(DatabaseAPI.Database.EnhancementSets[tId].DisplayName + str,
                            "Type: " + setTypeStringLong[(int) DatabaseAPI.Database.EnhancementSets[tId].SetType]);
                        if ((cell.X != _hoverCell.X | cell.Y != _hoverCell.Y) || alwaysUpdate)
                        {
                            RaiseHoverSet(tId);
                        }

                        hlOk = true;
                    }
                    else if (CellEnhSelect(cellIndex))
                    {
                        int tId = 0;
                        switch (UI.View.TabID)
                        {
                            case 0:
                                tId = -1;
                                SelectedEnhancement = null;
                                SetInfoStrings("", "");
                                break;
                            case Enums.eType.Normal:
                                tId = UI.NO[cellIndex];
                                SelectedEnhancement = DatabaseAPI.Database.Enhancements[tId];
                                SetInfoStrings(DatabaseAPI.Database.Enhancements[tId].Name, DatabaseAPI.Database.Enhancements[tId].Desc);
                                break;
                            case Enums.eType.InventO:
                                tId = UI.IO[cellIndex];
                                SelectedEnhancement = DatabaseAPI.Database.Enhancements[tId];
                                SetInfoStrings(DatabaseAPI.Database.Enhancements[tId].Name, DatabaseAPI.Database.Enhancements[tId].Desc);
                                break;
                            case Enums.eType.SpecialO:
                                tId = UI.SpecialO[cellIndex];
                                SelectedEnhancement = DatabaseAPI.Database.Enhancements[tId];
                                SetInfoStrings(DatabaseAPI.Database.Enhancements[tId].Name, DatabaseAPI.Database.Enhancements[tId].Desc);
                                break;
                            case Enums.eType.SetO:
                            {
                                tId = DatabaseAPI.Database.EnhancementSets[UI.Sets[UI.View.SetTypeID][UI.View.SetID]].Enhancements[cellIndex];
                                SelectedEnhancement = DatabaseAPI.Database.Enhancements[tId];
                                string text = DatabaseAPI.Database.Enhancements[tId].Name;
                                string str2;
                                if (DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[tId].nIDSet].LevelMin ==
                                    DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[tId].nIDSet].LevelMax)
                                {
                                    str2 = " (" + Convert.ToString(
                                               DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[tId].nIDSet].LevelMin +
                                               1) + ")";
                                }
                                else
                                {
                                    str2 = string.Concat(" (",
                                        Convert.ToString(DatabaseAPI.Database
                                                                 .EnhancementSets[DatabaseAPI.Database.Enhancements[tId].nIDSet].LevelMin + 1),
                                        "-",
                                        Convert.ToString(DatabaseAPI.Database
                                                                 .EnhancementSets[DatabaseAPI.Database.Enhancements[tId].nIDSet].LevelMax + 1),
                                        ")");
                                    if (DatabaseAPI.Database.Enhancements[tId].Unique)
                                    {
                                        text += " (Unique)";
                                    }
                                }

                                SetInfoStrings(
                                    DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[tId].nIDSet].DisplayName + str2,
                                    text);
                                break;
                            }
                        }

                        if (cell.X != _hoverCell.X || cell.Y != _hoverCell.Y || alwaysUpdate)
                        {
                            RaiseHoverEnhancement(tId);
                        }

                        hlOk = true;
                    }
                }

                if (!hlOk)
                {
                    cell = new Point(-1, -1);
                }

                if (cell.X == _hoverCell.X && cell.Y == _hoverCell.Y)
                    return;
                _hoverCell = cell;
                FullDraw();
            }
        }

        int SetIDGlobalToLocal(int iId)
        {
            checked
            {
                for (int i = 0; i <= UI.SetTypes.Length - 1; i++)
                {
                    for (int j = 0; j <= UI.Sets[i].Length - 1; j++)
                    {
                        if (UI.Sets[i][j] == iId)
                        {
                            return j;
                        }
                    }
                }

                return 0;
            }
        }

        static int[] GetSets(Enums.eSetType iSetType)
        {
            List<int> list = new List<int>();
            int num = 0;
            checked
            {
                int num2 = DatabaseAPI.Database.EnhancementSets.Count - 1;
                for (int i = num; i <= num2; i++)
                {
                    if (DatabaseAPI.Database.EnhancementSets[i].SetType == iSetType)
                        list.Add(i);
                }

                return list.ToArray();
            }
        }

        static int[] GetValidSetTypes(int iPowerIDX)
        {
            if (iPowerIDX < 0)
                return Array.Empty<int>();
            var array = new int[checked(DatabaseAPI.Database.Power[iPowerIDX].SetTypes.Length - 1 + 1)];
            Array.Copy(DatabaseAPI.Database.Power[iPowerIDX].SetTypes, array, DatabaseAPI.Database.Power[iPowerIDX].SetTypes.Length);
            Array.Sort(array);
            return array;
        }

        static int[] GetValidEnhancements(int iPowerIDX, Enums.eType iType, Enums.eSubtype iSubType = 0)
        {
            return iPowerIDX < 0 ? Array.Empty<int>() : DatabaseAPI.Database.Power[iPowerIDX].GetValidEnhancements(iType, iSubType);
        }

        public void SetData(int iPower, ref I9Slot iSlot, ref clsDrawX iDraw, int[] slotted)
        {
            TimerReset();
            _levelCapped = false;
            _userLevel = -1;
            _hDraw = iDraw;
            _mySlot = (I9Slot) iSlot.Clone();
            _hoverCell = new Point(-1, -1);
            _hoverText = "";
            _hoverTitle = "";
            _nPowerIDX = iPower;
            UI = new cTracking();
            Enums.eSubtype eSubtype = 0;
            UI.SpecialTypes = (int[]) Enum.GetValues(eSubtype.GetType());
            Enums.eEnhGrade eEnhGrade = 0;
            UI.NOGrades = (int[]) Enum.GetValues(eEnhGrade.GetType());
            UI.NO = GetValidEnhancements(_nPowerIDX, Enums.eType.Normal);
            UI.IO = GetValidEnhancements(_nPowerIDX, Enums.eType.InventO);
            UI.Initial.GradeID = _lastGrade;
            UI.Initial.RelLevel = _lastRelativeLevel;
            UI.Initial.SpecialID = _lastSpecial;
            if (UI.Initial.SpecialID == 0)
            {
                UI.Initial.SpecialID = Enums.eSubtype.Hamidon;
            }

            if (UI.Initial.GradeID == 0)
            {
                UI.Initial.GradeID = MidsContext.Config.CalcEnhOrigin;
            }

            if (UI.Initial.RelLevel == 0)
            {
                UI.Initial.RelLevel = MidsContext.Config.CalcEnhLevel;
            }

            if (_mySlot.Enh > -1)
            {
                if (DatabaseAPI.Database.Enhancements[_mySlot.Enh].SubTypeID != 0)
                {
                    UI.SpecialO = GetValidEnhancements(_nPowerIDX, Enums.eType.SpecialO,
                        DatabaseAPI.Database.Enhancements[_mySlot.Enh].SubTypeID);
                    UI.Initial.SpecialID = DatabaseAPI.Database.Enhancements[_mySlot.Enh].SubTypeID;
                }
                else
                {
                    UI.SpecialO = GetValidEnhancements(_nPowerIDX, Enums.eType.SpecialO, UI.Initial.SpecialID);
                }
            }
            else
            {
                UI.SpecialO = GetValidEnhancements(_nPowerIDX, Enums.eType.SpecialO, UI.Initial.SpecialID);
            }

            UI.SetTypes = GetValidSetTypes(_nPowerIDX);
            checked
            {
                UI.Sets = new int[UI.SetTypes.Length][];
                for (int i = 0; i <= UI.SetTypes.Length - 1; i++)
                {
                    UI.Sets[i] = GetSets((Enums.eSetType) UI.SetTypes[i]);
                }

                if (_mySlot.Grade != 0)
                    UI.Initial.GradeID = _mySlot.Grade;
                if (_mySlot.Enh > -1)
                    UI.Initial.RelLevel = _mySlot.RelativeLevel;
                else
                {
                    _mySlot.RelativeLevel = _lastRelativeLevel;
                    _mySlot.Grade = _lastGrade;
                }

                if (_mySlot.Enh > -1)
                {
                    var power = DatabaseAPI.Database.Enhancements[_mySlot.Enh].GetPower();
                    if (power != null && !power.BoostBoostable)
                    {
                        _mySlot.RelativeLevel = Enums.eEnhRelative.Even;
                    }

                    if (_nPowerIDX < 0)
                    {
                        switch (DatabaseAPI.Database.Enhancements[_mySlot.Enh].TypeID)
                        {
                            case Enums.eType.Normal:
                                UI.NO = new int[1];
                                UI.NO[0] = _mySlot.Enh;
                                break;
                            case Enums.eType.InventO:
                                UI.IO = new int[1];
                                UI.IO[0] = _mySlot.Enh;
                                break;
                            case Enums.eType.SpecialO:
                                UI.SpecialO = new int[1];
                                UI.SpecialO[0] = _mySlot.Enh;
                                break;
                            case Enums.eType.SetO:
                            {
                                UI.SetTypes = new int[1];
                                UI.SetTypes[0] = (int) DatabaseAPI.Database
                                    .EnhancementSets[DatabaseAPI.Database.Enhancements[_mySlot.Enh].nIDSet].SetType;
                                UI.Sets = new int[1][];
                                int[] array = new int[1];
                                UI.Sets[0] = array;
                                UI.Sets[0][0] = DatabaseAPI.Database.Enhancements[_mySlot.Enh].nIDSet;
                                UI.SetO = new int[1];
                                UI.SetO[0] = _mySlot.Enh;
                                break;
                            }
                        }
                    }

                    UI.Initial.TabID = DatabaseAPI.Database.Enhancements[_mySlot.Enh].TypeID;
                    if (UI.Initial.TabID == Enums.eType.SetO)
                    {
                        UI.Initial.SetID = SetIDGlobalToLocal(DatabaseAPI.Database.Enhancements[_mySlot.Enh].nIDSet);
                        UI.Initial.SetTypeID = SetTypeToID(DatabaseAPI.Database
                            .EnhancementSets[DatabaseAPI.Database.Enhancements[_mySlot.Enh].nIDSet].SetType);
                        UI.SetO = new int[DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[_mySlot.Enh].nIDSet]
                                              .Enhancements.Length - 1 + 1];
                        Array.Copy(DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[_mySlot.Enh].nIDSet].Enhancements,
                            UI.SetO, UI.SetO.Length);
                    }
                    else if (UI.SetTypes.Length > 0)
                    {
                        UI.Initial.SetTypeID = 0;
                    }

                    UI.Initial.PickerID = GetPickerIndex(_mySlot.Enh, UI.Initial.TabID);
                }
                else
                    UI.Initial.TabID = 0;

                if (UI.Initial.TabID == Enums.eType.InventO || UI.Initial.TabID == Enums.eType.SetO)
                    UI.Initial.IOLevel = _mySlot.IOLevel + 1;
                else if (LastLevel > 0)
                    UI.Initial.IOLevel = LastLevel;
                else
                    UI.Initial.IOLevel = MidsContext.Config.I9.DefaultIOLevel + 1;
                UI.View = new cTracking.cLocation(UI.Initial);
                if (UI.View.TabID == 0)
                {
                    UI.View.TabID = _lastTab != 0 ? _lastTab : Enums.eType.Normal;
                    if (UI.View.TabID == Enums.eType.SetO & UI.SetTypes.Length > _lastSet)
                        UI.View.SetTypeID = _lastSet;
                }

                _mySlotted = new int[slotted.Length - 1 + 1];
                Array.Copy(slotted, _mySlotted, _mySlotted.Length);
                if (UI.SetTypes.Length > 3)
                {
                    _rows = UI.SetTypes.Length;
                    Height = 235 + (_nSize + 2 + _nPad) * (UI.SetTypes.Length - 3);
                }
                else
                {
                    _rows = 5;
                    Height = 235;
                }

                FullDraw();
            }
        }

        void SetSpecialEnhSet(Enums.eSubtype iSubType)
        {
            UI.View.SpecialID = iSubType;
            if (_nPowerIDX > 0)
                UI.SpecialO = GetValidEnhancements(_nPowerIDX, Enums.eType.SpecialO, UI.View.SpecialID);
            else if (UI.Initial.SpecialID == UI.View.SpecialID & (int) UI.Initial.TabID == 3)
            {
                UI.SpecialO = new int[1];
                UI.SpecialO[0] = _mySlot.Enh;
            }
            else
                UI.SpecialO = Array.Empty<int>();
        }

        int SetTypeToID(Enums.eSetType iSetType)
        {
            checked
            {
                for (int i = 0; i <= UI.SetTypes.Length - 1; i++)
                {
                    if (iSetType == (Enums.eSetType) UI.SetTypes[i])
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        int GetPickerIndex(int index, Enums.eType iType)
        {
            var array = iType switch
            {
                Enums.eType.Normal => UI.NO,
                Enums.eType.InventO => UI.IO,
                Enums.eType.SpecialO => UI.SpecialO,
                Enums.eType.SetO => UI.SetO,
                _ => Array.Empty<int>()
            };

            checked
            {
                for (int i = 0; i <= array.Length - 1; i++)
                {
                    if (array[i] == index)
                        return i;
                }

                return -1;
            }
        }

        void CheckAndFixIOLevel()
        {
            int ioMax = IOMax;
            int ioMin = 10;
            checked
            {
                switch (UI.View.TabID)
                {
                    case Enums.eType.InventO:
                    {
                        if (UI.Initial.TabID == UI.View.TabID && UI.Initial.PickerID == UI.View.PickerID && UI.View.PickerID > -1)
                        {
                            ioMax = DatabaseAPI.Database.Enhancements[UI.IO[UI.View.PickerID]].LevelMax + 1;
                            ioMin = DatabaseAPI.Database.Enhancements[UI.IO[UI.View.PickerID]].LevelMin + 1;
                        }

                        break;
                    }
                    case Enums.eType.SetO when UI.View.SetID > -1 && UI.View.SetTypeID > -1:
                    {
                        var enhSetMax = DatabaseAPI.Database.EnhancementSets[UI.Sets[UI.View.SetTypeID][UI.View.SetID]].LevelMax + 1;
                        ioMax = enhSetMax;
                        var enhSetMin = DatabaseAPI.Database.EnhancementSets[UI.Sets[UI.View.SetTypeID][UI.View.SetID]].LevelMin + 1;
                        ioMin = enhSetMin;
                        break;
                    }
                }

                if (UI.View.IOLevel > ioMax)
                    UI.View.IOLevel = ioMax;
                if (UI.View.IOLevel < ioMin)
                    UI.View.IOLevel = ioMin;

                if (UI.View.TabID != Enums.eType.InventO)
                    return;
                if (ioMax > IOMax)
                    ioMax = IOMax;
                var nextLevel = Enhancement.GranularLevelZb(UI.View.IOLevel - 1, ioMin - 1, ioMax - 1) + 1;
                UI.View.IOLevel = nextLevel;
            }
        }

        public int CheckAndReturnIOLevel()
        {
            int ioMax = IOMax;
            int ioMin = 10;
            int fixedLevel = UI.View.IOLevel;
            checked
            {
                switch (UI.View.TabID)
                {
                    case Enums.eType.InventO:
                    {
                        if (UI.Initial.TabID == UI.View.TabID & UI.Initial.PickerID == UI.View.PickerID & UI.View.PickerID > -1)
                        {
                            ioMax = DatabaseAPI.Database.Enhancements[UI.IO[UI.View.PickerID]].LevelMax + 1;
                            ioMin = DatabaseAPI.Database.Enhancements[UI.IO[UI.View.PickerID]].LevelMin + 1;
                        }

                        break;
                    }
                    case Enums.eType.SetO when UI.View.SetID > -1 & UI.View.SetTypeID > -1:
                        ioMax = DatabaseAPI.Database.EnhancementSets[UI.Sets[UI.View.SetTypeID][UI.View.SetID]].LevelMax + 1;
                        ioMin = DatabaseAPI.Database.EnhancementSets[UI.Sets[UI.View.SetTypeID][UI.View.SetID]].LevelMin + 1;
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

                if (UI.View.TabID != Enums.eType.InventO)
                    return fixedLevel;
                if (ioMax > 50)
                {
                    ioMax = 50;
                }

                fixedLevel = Enhancement.GranularLevelZb(fixedLevel - 1, ioMin - 1, ioMax - 1) + 1;
                return fixedLevel;
            }
        }

        void TimerReset()
            => _timerLastKeypress = -1.0;

        void NumberPressed(int iNumber)
        {
            double timer = DateAndTime.Timer;
            if (timer - _timerLastKeypress > 5.0)
            {
                UI.View.IOLevel = iNumber;
                _userLevel = UI.View.IOLevel;
                _timerLastKeypress = timer;
            }
            else
            {
                string text = Convert.ToString(UI.View.IOLevel);
                text += Convert.ToString(iNumber);
                checked
                {
                    if (text.Length > 2)
                        text = text.Substring(text.Length - 2);

                    int number = (int) Math.Round(Conversion.Val(text));
                    if (number > 50)
                        number = 50;
                    if (number < 1)
                        number = 1;
                    UI.View.IOLevel = number;
                    _userLevel = UI.View.IOLevel;
                    if (number >= 10)
                        TimerReset();
                }
            }
        }

        void I9PickerKeyDown(object sender, KeyEventArgs e)
        {
            int num = -1;
            Keys keyCode = e.KeyCode;
            checked
            {
                switch (keyCode)
                {
                    case Keys.Return:
                        UI.Initial.IOLevel = UI.View.IOLevel;
                        UI.Initial.RelLevel = UI.View.RelLevel;
                        UI.View = new cTracking.cLocation(UI.Initial);
                        DoEnhancementPicked(UI.View.PickerID);
                        break;
                    case Keys.Back:
                        UI.View.IOLevel = UI.Initial.IOLevel;
                        UI.View.RelLevel = UI.Initial.RelLevel;
                        TimerReset();
                        break;
                    case Keys.Delete:
                        UI.View.IOLevel = UI.Initial.IOLevel;
                        UI.View.RelLevel = UI.Initial.RelLevel;
                        TimerReset();
                        break;
                    case Keys.Decimal:
                        UI.View.IOLevel = UI.Initial.IOLevel;
                        UI.View.RelLevel = UI.Initial.RelLevel;
                        TimerReset();
                        break;
                    case Keys.Home:
                        UI.View.IOLevel = 0;
                        UI.View.RelLevel = Enums.eEnhRelative.MinusThree;
                        TimerReset();
                        break;
                    case Keys.End:
                        UI.View.IOLevel = 100;
                        UI.View.RelLevel = Enums.eEnhRelative.PlusFive;
                        TimerReset();
                        break;
                    case Keys.Space:
                        UI.View.IOLevel = MidsContext.Config.I9.DefaultIOLevel + 1;
                        UI.View.RelLevel = Enums.eEnhRelative.Even;
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
                        if (e.KeyValue >= 48 & e.KeyValue <= 57)
                        {
                            num = e.KeyValue - 48;
                        }
                        else if (e.KeyValue >= 96 & e.KeyValue <= 105)
                        {
                            num = e.KeyValue - 96;
                        }

                        break;
                    }
                }

                //if (this.UI.View.TabID == Enums.eType.SetO || this.UI.View.TabID ==Enums.eType.InventO)
                if (UI.View.TabID == Enums.eType.SetO || UI.View.TabID == Enums.eType.InventO)
                {
                    if (num > -1)
                        NumberPressed(num);
                    else
                        CheckAndFixIOLevel();
                }

                DoHover(_hoverCell, true);
                FullDraw();
            }
        }

        void RelAdjust(int direction)
        {
            Enums.eEnhRelative eEnhRelative = UI.View.RelLevel;
            checked
            {
                if (direction < 0)
                {
                    if (UI.View.RelLevel > 0)
                    {
                        eEnhRelative--;
                    }
                }
                else if (direction > 0 && (int) UI.View.RelLevel < 9)
                {
                    eEnhRelative++;
                }

                if (SelectedEnhancement != null && (DatabaseAPI.EnhHasCatalyst(SelectedEnhancement.UID) || DatabaseAPI.EnhIsNaturallyAttuned(SelectedEnhancement.StaticIndex)))
                {
                    eEnhRelative = Enums.eEnhRelative.Even;
                }
                //if (eEnhRelative > Enums.eEnhRelative.PlusThree & (UI.View.TabID == Enums.eType.Normal | (int) UI.View.TabID == 3))
                else if (eEnhRelative > Enums.eEnhRelative.PlusThree & (UI.View.TabID == Enums.eType.Normal))
                {
                    eEnhRelative = Enums.eEnhRelative.PlusThree;
                }
                else if (eEnhRelative > Enums.eEnhRelative.PlusTwo & (UI.View.TabID == Enums.eType.SpecialO))
                {
                    eEnhRelative = Enums.eEnhRelative.PlusTwo;
                }
                else if (eEnhRelative < Enums.eEnhRelative.Even & (UI.View.TabID == Enums.eType.InventO | UI.View.TabID == Enums.eType.SetO))
                {
                    eEnhRelative = Enums.eEnhRelative.Even;
                }

                UI.View.RelLevel = eEnhRelative;
            }
        }

        public class cTracking
        {
            public cTracking()
            {
                NO = Array.Empty<int>();
                IO = Array.Empty<int>();
                SpecialO = Array.Empty<int>();
                NOGrades = Array.Empty<int>();
                SpecialTypes = Array.Empty<int>();
                SetTypes = Array.Empty<int>();
                Sets = Array.Empty<int[]>();
                SetO = Array.Empty<int>();
                Initial = new cLocation();
                View = new cLocation();
            }

            public int[] NO;
            public int[] IO;
            public int[] SpecialO;
            public int[] NOGrades;
            public int[] SpecialTypes;
            public int[] SetTypes;
            public int[][] Sets;
            public int[] SetO;
            public readonly cLocation Initial;
            public cLocation View;

            public class cLocation
            {
                public cLocation()
                {
                    TabID = 0;
                    PickerID = -1;
                    SetTypeID = -1;
                    SetID = -1;
                    GradeID = 0;
                    SpecialID = 0;
                    IOLevel = 0;
                    RelLevel = Enums.eEnhRelative.Even;
                }

                public cLocation(cLocation iCL)
                {
                    TabID = 0;
                    PickerID = -1;
                    SetTypeID = -1;
                    SetID = -1;
                    GradeID = 0;
                    SpecialID = 0;
                    IOLevel = 0;
                    RelLevel = Enums.eEnhRelative.Even;
                    TabID = iCL.TabID;
                    PickerID = iCL.PickerID;
                    SetTypeID = iCL.SetTypeID;
                    GradeID = iCL.GradeID;
                    SpecialID = iCL.SpecialID;
                    SetID = iCL.SetID;
                    IOLevel = iCL.IOLevel;
                    RelLevel = iCL.RelLevel;
                }

                public Enums.eType TabID;
                public int PickerID;
                public int SetTypeID;
                public int SetID;
                public Enums.eEnhGrade GradeID;
                public Enums.eSubtype SpecialID;
                public int IOLevel;
                public Enums.eEnhRelative RelLevel;
            }
        }

        public delegate void EnhancementPickedEventHandler(I9Slot e);

        public delegate void HoverEnhancementEventHandler(int e);

        public delegate void HoverSetEventHandler(int e);

        public delegate void MovedEventHandler(Rectangle oldBounds, Rectangle newBounds);
    }
}
