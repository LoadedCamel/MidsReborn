using Mids_Reborn.Core;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using FontStyle = System.Drawing.FontStyle;

namespace Mids_Reborn.Forms.Sharing
{
    public static class InfoGraphic
    {
        private static readonly Character.TotalStatistics Totals = MidsContext.Character.Totals;
        private static readonly Statistics Display = MidsContext.Character.DisplayStats;

        private static Graphics? _graphics;
        private static readonly Font FooterFont = new(Fonts.Family("Noto Sans Medium"), 8.25f, FontStyle.Bold);
        private static readonly Font TextFont = new(Fonts.Family("Noto Sans"), 10.75f, FontStyle.Bold);
        private static readonly Font HeaderFont = new(Fonts.Family("Noto Sans Black"), 12.75f, FontStyle.Bold);
        private static readonly Font HeaderFontUl = new(Fonts.Family("Noto Sans Black"), 12.75f, FontStyle.Bold | FontStyle.Underline);

        public static string Generate()
        {
            // Disposable Stream for output
            using var stream = new MemoryStream();

            // Disposable Brushes
            using var background = new SolidBrush(Color.FromArgb(44, 47, 51));
            using var foreground = new SolidBrush(Color.WhiteSmoke);

            // Non-Disposable Brushes
            var outline = new SolidBrush(Color.White);

            // Graphics Setup and Vars
            using var tmp = new Bitmap(450, 250, PixelFormat.Format32bppArgb);
            _graphics = Graphics.FromImage(tmp);
            _graphics.CompositingQuality = CompositingQuality.HighQuality;
            _graphics.CompositingMode = CompositingMode.SourceOver;
            _graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            _graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            _graphics.SmoothingMode = SmoothingMode.HighQuality;
            _graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            _graphics.Clear(Color.FromArgb(44, 47, 51));

            // Conditional Variables and Disposables
            outline = MidsContext.Character.IsHero() ? new SolidBrush(Color.DodgerBlue) : new SolidBrush(Color.DarkRed);
            using var pen = new Pen(outline, 4);

            // Measure random text w/font for line height.
            var textSize = Measured("Some Random Text", TextFont);

            // Drawing
            _graphics.DrawRectangle(pen, 0, 0, 450, 250);

            // Insert Backdrop
            var imgPath = Path.Combine(AppContext.BaseDirectory, "Images");
            var bgImageFile = MidsContext.Character.IsHero() ? "InfoBackDropH.png" : "InfoBackDropV.png";
            bgImageFile = Path.Combine(imgPath, bgImageFile);
            _graphics.DrawImage(Image.FromFile(bgImageFile), new RectangleF(0, 0, 450, 250));

            // Insert Character Name & Level
            _graphics.DrawString($"Name: {MidsContext.Character.Name}", TextFont, foreground, new PointF(7, 7));
            _graphics.DrawString($"Level: {MidsContext.Character.Level + 1}", TextFont, foreground, new PointF(7, textSize.Height + 10));

            // Insert AT Header
            var powerSets = $"{MidsContext.Character.Powersets[0].DisplayName} / {MidsContext.Character.Powersets[1].DisplayName}";
            var psSize = Measured(powerSets, HeaderFont);
            var archetype = MidsContext.Character.Archetype.DisplayName;
            var atSize = Measured(archetype, HeaderFont);
            _graphics.DrawString(archetype, HeaderFont, foreground, new PointF(tmp.Width - atSize.Width - 7, textSize.Height / 2 - 7));
            _graphics.DrawString(powerSets, HeaderFont, foreground, new PointF(tmp.Width - psSize.Width - 7, textSize.Height + 7));

            // Draw Separator
            _graphics.DrawLine(new Pen(outline, 2), 0, textSize.Height * 3 - 7, 450, textSize.Height * 3 - 7);

            // Insert Stats Header
            _graphics.DrawString("Stats Preview", HeaderFontUl, foreground, new PointF(tmp.Width / 2f - Measured("Stats Preview", HeaderFontUl).Width / 2, textSize.Height * 3 + 7 - 4));

            // Insert Stats
            var hpRegen = $"Regen: {Convert.ToDecimal(Display.HealthRegenPercent(false)):0.##}% ({Convert.ToDecimal(Display.HealthRegenHPPerSec):0.##}/s)";
            var maxHp = $"Max HP: {Convert.ToDecimal(Display.HealthHitpointsPercentage):0.##}% ({Convert.ToDecimal(Display.HealthHitpointsNumeric(false)):0.##})";
            var endUse = $"End Usage: {Convert.ToDecimal(Display.EnduranceUsage):0.##}/s";
            var endRec = $"Recovery: {Display.EnduranceRecoveryPercentage(false):###0}% ({Convert.ToDecimal(Display.EnduranceRecoveryNumeric):0.##}/s)";
            var maxEnd = $"Max End: {Convert.ToDecimal(Totals.EndMax + 100f):0.##}%";
            var haste = $"Haste: {Convert.ToDecimal(Totals.BuffHaste * 100):0.##}%";
            _graphics.DrawString(maxHp, TextFont, foreground, new PointF(7, (float)(textSize.Height * 4.5 + 7)));
            _graphics.DrawString(hpRegen, TextFont, foreground, new PointF(7, (float)(textSize.Height * 5.5 + 7)));
            if (Totals.BuffHaste > 0)
            {
                _graphics.DrawString(haste, TextFont, foreground, new PointF(7, (float)(textSize.Height * 6.5 + 7)));
            }
            _graphics.DrawString(maxEnd, TextFont, foreground, new PointF(tmp.Width - Measured(maxEnd, TextFont).Width - 7, (float)(textSize.Height * 4.5 + 7)));
            _graphics.DrawString(endRec, TextFont, foreground, new PointF(tmp.Width - Measured(endRec, TextFont).Width - 7, (float)(textSize.Height * 5.5 + 7)));
            _graphics.DrawString(endUse, TextFont, foreground, new PointF(tmp.Width - Measured(endUse, TextFont).Width - 7, (float)(textSize.Height * 6.5 + 7)));

            // Insert App Version
            var versionString = $"This build was created with {MidsContext.AppName} v{MidsContext.AppFileVersion}\r\nUsing the {DatabaseAPI.DatabaseName} database v{DatabaseAPI.Database.Version}";
            var verMeasure = Measured(versionString, FooterFont);
            _graphics.DrawString(versionString, FooterFont, foreground, new PointF(7f, tmp.Height - verMeasure.Height - 7));

            // Save to stream
            tmp.Save(stream, ImageFormat.Png);
            var imageBytes = stream.ToArray();
            return Convert.ToBase64String(imageBytes);
        }



        private static SizeF Measured(string text, Font font)
        {
            if (_graphics != null) return _graphics.MeasureString(text, font);
            throw new NotImplementedException("Graphics not initialized");
        }
    }
}
