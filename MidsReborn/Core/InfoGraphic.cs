using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using FontStyle = System.Drawing.FontStyle;
using Point = System.Drawing.Point;

namespace Mids_Reborn.Core
{
    public static class InfoGraphic
    {
        private static Graphics? _graphics;
        private static readonly Font FooterFont = new("Arial", 8f, FontStyle.Bold);
        private static readonly Font HeaderFont = new("Arial Black", 12f, FontStyle.Bold);
        private static readonly Font StatHeaderFont = new("Arial Black", 11.25f, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point);
        private static readonly Font StatFont = new("Arial", 9f, FontStyle.Bold, GraphicsUnit.Point);

        public static byte[] Generate(bool useAltBg = false)
        {
            // Disposable Stream for output
            using var stream = new MemoryStream();

            // Disposable Brushes
            //using var background = new SolidBrush(Color.FromArgb(44, 47, 51));
            using var foreground = new SolidBrush(Color.WhiteSmoke);

            // Graphics Setup and Vars
            using var tmp = new Bitmap(650, 400, PixelFormat.Format32bppArgb);
            _graphics = Graphics.FromImage(tmp);
            _graphics.CompositingQuality = CompositingQuality.HighQuality;
            _graphics.CompositingMode = CompositingMode.SourceOver;
            _graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            _graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            _graphics.SmoothingMode = SmoothingMode.HighQuality;
            _graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            _graphics.Clear(Color.FromArgb(44, 47, 51));

            // Insert Backdrop
            switch (useAltBg)
            {
                case false:
                    _graphics.DrawImage(MidsContext.Character!.IsHero() ? Image.FromFile(Path.Combine(AppContext.BaseDirectory, "Images", "InfoBackDropH.png")) : Image.FromFile(Path.Combine(AppContext.BaseDirectory, "Images", "InfoBackDropV.png")), new RectangleF(0, 0, tmp.Width, tmp.Height));
                    break;
                case true:
                    _graphics.DrawImage(MidsContext.Character!.IsHero() ? Image.FromFile(Path.Combine(AppContext.BaseDirectory, "Images", "InfoBackDropH2.png")) : Image.FromFile(Path.Combine(AppContext.BaseDirectory, "Images", "InfoBackDropV2.png")), new RectangleF(0, 0, tmp.Width, tmp.Height));
                    break;
            }
            

            // Outline Brush and Disposable Pen
            var outline = MidsContext.Character!.IsHero() ? new SolidBrush(Color.DodgerBlue) : new SolidBrush(Color.DarkRed);
            using var pen = new Pen(outline, 4);

            // Measure random text w/font for line height.
            var textSize = Measured("Some Random Text", HeaderFont);

            // Drawing
            var clientRectangle = new Rectangle(0, 0, tmp.Width, tmp.Height);
            _graphics.DrawRectangle(pen, clientRectangle);

            // Insert Character Name & Level
            var name = $"Name: {MidsContext.Character.Name}";
            var level = $"Level: {MidsContext.Character.Level + 1}";

            _graphics.DrawString(name, HeaderFont, foreground, new PointF(7, textSize.Height / 2 - 7));
            _graphics.DrawString(level, HeaderFont, foreground, new PointF(7, textSize.Height + 7));

            // Insert AT Header
            var powerSets = $"{MidsContext.Character.Powersets[0]!.DisplayName} / {MidsContext.Character.Powersets[1]!.DisplayName}";
            var psSize = Measured(powerSets, HeaderFont);
            var archetype = MidsContext.Character.Archetype!.DisplayName;
            var atSize = Measured(archetype, HeaderFont);
            _graphics.DrawString(archetype, HeaderFont, foreground, new PointF(clientRectangle.Right - atSize.Width - 7, textSize.Height / 2 - 7));
            _graphics.DrawString(powerSets, HeaderFont, foreground, new PointF(clientRectangle.Right - psSize.Width - 7, textSize.Height + 7));


            // Draw Separator
            var sepY = textSize.Height * 3 - 15;
            _graphics.DrawLine(new Pen(outline, 2), 0, sepY, tmp.Width, sepY);

            // Insert Stats
            var statData = Helpers.GeneratedStatData(true);
            var rect = new Rectangle(10, (int)sepY + 4, tmp.Width - 7, tmp.Height - ((int)sepY + 4));
            var headerBrush = new SolidBrush(Color.WhiteSmoke);
            const float columnWidth = 125f;
            const float columnHeight = 24f;
            var columnSpacing = (rect.Width - columnWidth * 5f) / 4f;
            const int rowSpacing = 1;

            for (var cIndex = 0; cIndex < statData.Count; cIndex++)
            {
                var statOutPen = new Pen(Color.Black, 3) { LineJoin = LineJoin.Round };
                var sFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Near,
                    Trimming = StringTrimming.None
                };
                var stat = statData.ElementAt(cIndex);
                var headerRect = new Rectangle((int)(rect.Left + columnSpacing * cIndex + columnWidth * cIndex) - 8, rect.Top + 7, (int)columnWidth, rect.Height);
                headerBrush = stat.Key switch
                {
                    "Defense" => BrushFromHex("#b877dd"),
                    "Resistance" => BrushFromHex("#e8a080"),
                    "Sustain" => BrushFromHex("#65d6bf"),
                    "Offense" => BrushFromHex("#d87474"),
                    "Debuff Resist" => BrushFromHex("#d7c574"),
                    _ => headerBrush
                };
                var headerPath = new GraphicsPath();
                headerPath.AddString(stat.Key, StatHeaderFont.FontFamily, (int)StatHeaderFont.Style, StatHeaderFont.Size + 3f, headerRect, sFormat);
                _graphics.DrawPath(statOutPen, headerPath);
                _graphics.FillPath(headerBrush, headerPath);

                for (var rIndex = 0; rIndex < stat.Value.Count; rIndex++)
                {
                    var point = new Point((int)(rect.Left + columnSpacing * cIndex + columnWidth * cIndex), (int)(headerRect.Top + rowSpacing * rIndex + columnHeight * rIndex));
                    var statBrush = BrushFromHex("#ffffff");
                    var statRect = new Rectangle(point.X -4, point.Y + 24, (int)columnWidth, (int)columnHeight);
                    var statPath = new GraphicsPath();
                    statPath.AddString($"{stat.Value[rIndex].Type}: ", StatFont.FontFamily, (int)StatFont.Style, StatFont.Size + 4f, statRect, null);
                    _graphics.DrawPath(statOutPen, statPath);
                    _graphics.FillPath(statBrush, statPath);
                    var typeSize = Measured(stat.Value[rIndex].Type, StatFont);
                    var valueBrush = BrushFromHex("#a1d96a");
                    statRect = new Rectangle((int)(statRect.Left + typeSize.Width), statRect.Top, (int)(statRect.Width - typeSize.Width - 2), statRect.Height);
                    var valPath = new GraphicsPath();
                    sFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Far,
                        LineAlignment = StringAlignment.Near,
                        Trimming = StringTrimming.None
                    };
                    valPath.AddString($"{stat.Value[rIndex].Percentage}", StatFont.FontFamily, (int)StatFont.Style, StatFont.Size + 4f, statRect, sFormat);
                    _graphics.DrawPath(statOutPen, valPath);
                    _graphics.FillPath(valueBrush, valPath);
                }
            }

            // Insert App Version
            var versionString = $"This build graphic was created with {MidsContext.AppName} v{MidsContext.AssemblyVersion} Rev. {MidsContext.AppFileVersion.Revision}";
            var dbVersionString = $"Using the {DatabaseAPI.DatabaseName} database v{DatabaseAPI.Database.Version}";
            var verMeasure = Measured(versionString, FooterFont);
            var dbVerMeasure = Measured(dbVersionString, FooterFont);
            _graphics.DrawString(versionString, FooterFont, foreground, new PointF(tmp.Width - verMeasure.Width - 7f, tmp.Height - verMeasure.Height - dbVerMeasure.Height - 7));
            _graphics.DrawString(dbVersionString, FooterFont, foreground, new PointF(tmp.Width - dbVerMeasure.Width - 7f, tmp.Height - dbVerMeasure.Height - 7));

            // Save to stream
            tmp.Save(stream, ImageFormat.Png);
            var imageBytes = stream.ToArray();

            // Dispose of graphics
            _graphics.Dispose();

            return imageBytes;
        }

        public static string GenerateImageData(bool useAltBg = false)
        {
            var imgBytes = Generate(useAltBg);
            var compressed = Compression.CompressToBase64(imgBytes);
            return compressed.OutString;
        }

        private static SolidBrush BrushFromHex(string hex)
        {
            var converter = new ColorConverter();
            return new SolidBrush((Color)(converter.ConvertFromString(hex) ?? Color.WhiteSmoke));
        }

        private static SizeF Measured(string text, Font font)
        {
            if (_graphics != null)
            {
                return _graphics.MeasureString(text, font);
            }
            throw new NotImplementedException("Graphics not initialized");
        }
    }
}
