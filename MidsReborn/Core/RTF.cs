using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Globalization;
using System.Text;

namespace Mids_Reborn.Core
{
    public static class RTF
    {
        public enum ElementID
        {
            Black = 0,
            Enhancement = 1,
            Faded = 2,
            Invention = 3,
            InventionInvert = 4,
            Text = 5,
            Warning = 6,
            BackgroundHero = 7,
            BackgroundVillain = 8,
            Alert = 9
        }

        // Deltas are in HALF-POINTS (RTF \fs units). ±2 == ±1pt, ±4 == ±2pt, etc.
        public enum SizeID
        {
            VeryTiny = -8,
            Tiny = -4,
            Regular = 0,
            Larger = 2,
            Large = 4,
            Huge = 8
        }

        private const string SymbolFontDecl = "{\\f1\\fnil\\fcharset2 Symbol;}";
        private const string BoldOn = "\\b ";
        private const string BoldOff = "\\b0 ";
        private const string ItalicOn = "\\i ";
        private const string ItalicOff = "\\i0 ";
        private const string UnderlineOn = "\\ul ";
        private const string UnderlineOff = "\\ulnone ";
        private const string Par = "\\par ";
        private const string Tab = "\\tab ";

        // -----------------------
        // PUBLIC HELPERS
        // -----------------------

        // Escapes user text for inclusion in RTF and replaces newlines/tabs.
        public static string ToRTF(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            // Escape \ { }
            var esc = s
                .Replace("\\", "\\\\")
                .Replace("{", "\\{")
                .Replace("}", "\\}");
            // Normalize newlines/tabs
            esc = esc.Replace("\r\n", "\n").Replace("\r", "\n");
            esc = esc.Replace("\n", Par).Replace("\t", Tab);
            return esc;
        }

        public static string Crlf() => Par;

        public static string Bold(string content) => BoldOn + content + BoldOff;
        public static string Italic(string content) => ItalicOn + content + ItalicOff;
        public static string Underline(string content) => UnderlineOn + content + UnderlineOff;

        public static string Color(ElementID element) => "\\cf" + ((int)element).ToString(CultureInfo.InvariantCulture) + " ";

        // Size using global base (legacy behavior). Base must be HALF-POINTS already.
        public static string Size(SizeID delta)
        {
            int baseFs = MidsContext.Config.RtFont.RTFBase; // EXPECTED: half-points
            return "\\fs" + (baseFs + (int)delta).ToString(CultureInfo.InvariantCulture) + " ";
        }

        // Size relative to a Font (recommended).
        public static string Size(SizeID delta, Font font)
        {
            int baseFs = FontToFs(font); // half-points
            return "\\fs" + (baseFs + (int)delta).ToString(CultureInfo.InvariantCulture) + " ";
        }

        // -----------------------
        // DOCUMENT BUILD
        // -----------------------

        // Legacy start/end (kept for compatibility) – uses global RTFBase + hard-coded Arial.
        public static string StartRTF()
        {
            var sb = new StringBuilder();
            sb.Append("{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang2057{\\fonttbl{\\f0\\fswiss\\fcharset0 Arial;}")
              .Append(SymbolFontDecl).Append("}");
            sb.Append(Environment.NewLine);
            sb.Append(GetColorTable());
            sb.Append(Environment.NewLine);
            sb.Append(GetInitialLine(MidsContext.Config.RtFont.RTFBase, "Arial", MidsContext.Config.RtFont.RTFBold));
            return sb.ToString();
        }

        public static string EndRTF()
        {
            var sb = new StringBuilder();
            sb.Append(Size(SizeID.Regular));
            sb.Append(Color(ElementID.Text));
            sb.Append(GetFooter(MidsContext.Config.RtFont.RTFBold));
            return sb.ToString();
        }

        // Recommended start/end – drives font face/size from the control’s Font.
        public static string StartRTF(Font font, bool boldDefault = false)
        {
            var safeFace = SanitizeFontName(font?.Name ?? "Segoe UI");
            int fs = FontToFs(font);
            var sb = new StringBuilder();
            sb.Append("{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang2057{\\fonttbl{\\f0 ").Append(safeFace).Append(";}")
              .Append(SymbolFontDecl).Append("}");
            sb.Append(Environment.NewLine);
            sb.Append(GetColorTable());
            sb.Append(Environment.NewLine);
            sb.Append(GetInitialLine(fs, safeFace, boldDefault));
            return sb.ToString();
        }

        public static string EndRTF(Font font, bool boldDefault = false)
        {
            var sb = new StringBuilder();
            sb.Append(Size(SizeID.Regular, font));
            sb.Append(Color(ElementID.Text));
            sb.Append(GetFooter(boldDefault));
            return sb.ToString();
        }

        // -----------------------
        // INTERNALS
        // -----------------------

        private static string GetColorTable()
        {
            // Index 0 is "auto" (empty entry)
            var c = MidsContext.Config.RtFont;
            var sb = new StringBuilder("{\\colortbl ;");
            // 1 Enhancement
            sb.Append("\\red").Append(c.ColorEnhancement.R).Append("\\green").Append(c.ColorEnhancement.G).Append("\\blue").Append(c.ColorEnhancement.B).Append(";");
            // 2 Faded
            sb.Append("\\red").Append(c.ColorFaded.R).Append("\\green").Append(c.ColorFaded.G).Append("\\blue").Append(c.ColorFaded.B).Append(";");
            // 3 Invention
            sb.Append("\\red").Append(c.ColorInvention.R).Append("\\green").Append(c.ColorInvention.G).Append("\\blue").Append(c.ColorInvention.B).Append(";");
            // 4 InventionInv
            sb.Append("\\red").Append(c.ColorInventionInv.R).Append("\\green").Append(c.ColorInventionInv.G).Append("\\blue").Append(c.ColorInventionInv.B).Append(";");
            // 5 Text
            sb.Append("\\red").Append(c.ColorText.R).Append("\\green").Append(c.ColorText.G).Append("\\blue").Append(c.ColorText.B).Append(";");
            // 6 Warning
            sb.Append("\\red").Append(c.ColorWarning.R).Append("\\green").Append(c.ColorWarning.G).Append("\\blue").Append(c.ColorWarning.B).Append(";");
            // 7 BackgroundHero
            sb.Append("\\red").Append(c.ColorBackgroundHero.R).Append("\\green").Append(c.ColorBackgroundHero.G).Append("\\blue").Append(c.ColorBackgroundHero.B).Append(";");
            // 8 BackgroundVillain
            sb.Append("\\red").Append(c.ColorBackgroundVillain.R).Append("\\green").Append(c.ColorBackgroundVillain.G).Append("\\blue").Append(c.ColorBackgroundVillain.B).Append(";");
            // 9 Alert (hard-coded yellow in your original)
            sb.Append("\\red255\\green255\\blue0;");
            sb.Append("}");
            return sb.ToString();
        }

        private static string GetInitialLine(int baseFsHalfPoints, string face, bool boldDefault)
        {
            var sb = new StringBuilder();
            sb.Append("{\\*\\generator MHD_RTFClass;}\\viewkind4\\uc1\\pard\\f0\\fs")
              .Append(baseFsHalfPoints.ToString(CultureInfo.InvariantCulture)).Append(" ");
            sb.Append(Color(ElementID.Text));
            if (boldDefault) sb.Append(BoldOn);
            return sb.ToString();
        }

        private static string GetFooter(bool boldDefault)
        {
            var sb = new StringBuilder();
            if (boldDefault) sb.Append(BoldOff);
            sb.Append("\\par}");
            return sb.ToString();
        }

        private static int FontToFs(Font font)
        {
            // \fs is half-points: fs = round(points * 2)
            float points = (font?.SizeInPoints ?? 9.75f);
            return (int)Math.Round(points * 2f, MidpointRounding.AwayFromZero);
        }

        private static string SanitizeFontName(string name)
        {
            // Very light sanitization for fonttbl entry
            if (string.IsNullOrWhiteSpace(name)) return "Segoe UI";
            // RTF doesn't like unescaped braces/backslashes in font names (rare), strip them.
            return name.Replace("\\", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty);
        }
    }
}