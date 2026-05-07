using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

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
            => StartRTF(font, boldDefault, null);

        public static string StartRTF(Font font, bool boldDefault, IReadOnlyList<Color>? extraColors)
        {
            var safeFace = SanitizeFontName(font?.Name ?? "Segoe UI");
            int fs = FontToFs(font);
            var sb = new StringBuilder();
            sb.Append("{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang2057{\\fonttbl{\\f0 ").Append(safeFace).Append(";}")
              .Append(SymbolFontDecl).Append("}");
            sb.Append(Environment.NewLine);
            sb.Append(GetColorTable(extraColors));
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

        public static string FormatMarkupDocument(string s, Font font, bool boldDefault = false)
        {
            var extraColors = new List<Color>();
            var content = ToRtfMarkup(s, extraColors);
            return StartRTF(font, boldDefault, extraColors) + content + EndRTF(font, boldDefault);
        }

        // -----------------------
        // INTERNALS
        // -----------------------

        private static string GetColorTable(IReadOnlyList<Color>? extraColors = null)
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
            if (extraColors is { Count: > 0 })
            {
                foreach (var color in extraColors)
                {
                    sb.Append("\\red").Append(color.R).Append("\\green").Append(color.G).Append("\\blue").Append(color.B).Append(";");
                }
            }
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

        private static string ToRtfMarkup(string s, List<Color> extraColors)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            var colorStack = new Stack<int>();
            colorStack.Push((int)ElementID.Text);

            int boldDepth = 0;
            int italicDepth = 0;
            int underlineDepth = 0;
            int index = 0;

            while (index < s.Length)
            {
                if (TryReadMarkupToken(s, index, out var token, out var tokenLength) &&
                    TryAppendMarkupToken(token, extraColors, colorStack, ref boldDepth, ref italicDepth, ref underlineDepth, sb))
                {
                    index += tokenLength;
                    continue;
                }

                int nextTokenIndex = FindNextTokenStart(s, index);
                if (nextTokenIndex < 0)
                {
                    nextTokenIndex = s.Length;
                }
                else if (nextTokenIndex == index)
                {
                    // Unknown markup-like sequences should render as literal text instead of stalling
                    // the parser on the same '<' or '[' forever.
                    nextTokenIndex = Math.Min(s.Length, index + 1);
                }

                AppendPlainText(sb, s[index..nextTokenIndex]);
                index = nextTokenIndex;
            }

            while (underlineDepth-- > 0) sb.Append(UnderlineOff);
            while (italicDepth-- > 0) sb.Append(ItalicOff);
            while (boldDepth-- > 0) sb.Append(BoldOff);

            if (colorStack.Peek() != (int)ElementID.Text)
            {
                sb.Append(Color(ElementID.Text));
            }

            return sb.ToString();
        }

        private static void AppendPlainText(StringBuilder sb, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            sb.Append(ToRTF(WebUtility.HtmlDecode(text)));
        }

        private static int FindNextTokenStart(string text, int startIndex)
        {
            int htmlIndex = text.IndexOf('<', startIndex);
            int bbCodeIndex = text.IndexOf('[', startIndex);

            if (htmlIndex < 0) return bbCodeIndex;
            if (bbCodeIndex < 0) return htmlIndex;
            return Math.Min(htmlIndex, bbCodeIndex);
        }

        private static bool TryReadMarkupToken(string text, int index, out string token, out int tokenLength)
        {
            token = string.Empty;
            tokenLength = 0;

            char start = text[index];
            char end = start switch
            {
                '<' => '>',
                '[' => ']',
                _ => '\0'
            };

            if (end == '\0')
            {
                return false;
            }

            int endIndex = text.IndexOf(end, index + 1);
            if (endIndex < 0)
            {
                return false;
            }

            token = text[index..(endIndex + 1)];
            tokenLength = token.Length;
            return true;
        }

        private static bool TryAppendMarkupToken(
            string token,
            List<Color> extraColors,
            Stack<int> colorStack,
            ref int boldDepth,
            ref int italicDepth,
            ref int underlineDepth,
            StringBuilder sb)
        {
            var normalized = token.Trim();
            if (string.IsNullOrEmpty(normalized))
            {
                return false;
            }

            if (Regex.IsMatch(normalized, @"^<br\s*/?>$", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(normalized, @"^</?p\s*/?>$", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(normalized, @"^</?div\s*/?>$", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(normalized, @"^<li\s*>$", RegexOptions.IgnoreCase))
            {
                sb.Append(Par);
                return true;
            }

            if (Regex.IsMatch(normalized, @"^</li\s*>$", RegexOptions.IgnoreCase))
            {
                return true;
            }

            if (IsOpenTag(normalized, "b", "strong") || IsOpenBbCode(normalized, "b"))
            {
                if (boldDepth++ == 0) sb.Append(BoldOn);
                return true;
            }

            if (IsCloseTag(normalized, "b", "strong") || IsCloseBbCode(normalized, "b"))
            {
                if (boldDepth > 0 && --boldDepth == 0) sb.Append(BoldOff);
                return true;
            }

            if (IsOpenTag(normalized, "i", "em") || IsOpenBbCode(normalized, "i"))
            {
                if (italicDepth++ == 0) sb.Append(ItalicOn);
                return true;
            }

            if (IsCloseTag(normalized, "i", "em") || IsCloseBbCode(normalized, "i"))
            {
                if (italicDepth > 0 && --italicDepth == 0) sb.Append(ItalicOff);
                return true;
            }

            if (IsOpenTag(normalized, "u") || IsOpenBbCode(normalized, "u"))
            {
                if (underlineDepth++ == 0) sb.Append(UnderlineOn);
                return true;
            }

            if (IsCloseTag(normalized, "u") || IsCloseBbCode(normalized, "u"))
            {
                if (underlineDepth > 0 && --underlineDepth == 0) sb.Append(UnderlineOff);
                return true;
            }

            if (TryParseColorTag(normalized, out var colorIndex, extraColors))
            {
                colorStack.Push(colorIndex);
                sb.Append("\\cf").Append(colorIndex.ToString(CultureInfo.InvariantCulture)).Append(' ');
                return true;
            }

            if (IsColorCloseTag(normalized) && colorStack.Count > 1)
            {
                colorStack.Pop();
                sb.Append("\\cf").Append(colorStack.Peek().ToString(CultureInfo.InvariantCulture)).Append(' ');
                return true;
            }

            return false;
        }

        private static bool IsOpenTag(string token, params string[] names)
            => names.Any(name => Regex.IsMatch(token, $@"^<{name}\s*>$", RegexOptions.IgnoreCase));

        private static bool IsCloseTag(string token, params string[] names)
            => names.Any(name => Regex.IsMatch(token, $@"^</{name}\s*>$", RegexOptions.IgnoreCase));

        private static bool IsOpenBbCode(string token, string name)
            => Regex.IsMatch(token, $@"^\[{name}\]$", RegexOptions.IgnoreCase);

        private static bool IsCloseBbCode(string token, string name)
            => Regex.IsMatch(token, $@"^\[/{name}\]$", RegexOptions.IgnoreCase);

        private static bool IsColorCloseTag(string token)
            => Regex.IsMatch(token, @"^</(?:color|font|span)\s*>$", RegexOptions.IgnoreCase) ||
               Regex.IsMatch(token, @"^\[/color\]$", RegexOptions.IgnoreCase);

        private static bool TryParseColorTag(string token, out int colorIndex, List<Color> extraColors)
        {
            colorIndex = 0;
            string? colorSpec = null;

            var htmlColorMatch = Regex.Match(token, @"^<color(?:\s+|\s*=\s*|:\s*)([^>]+)>$", RegexOptions.IgnoreCase);
            if (htmlColorMatch.Success)
            {
                colorSpec = htmlColorMatch.Groups[1].Value;
            }

            if (colorSpec is null)
            {
                var bbCodeMatch = Regex.Match(token, @"^\[color(?:\s*=\s*|:\s*)([^\]]+)\]$", RegexOptions.IgnoreCase);
                if (bbCodeMatch.Success)
                {
                    colorSpec = bbCodeMatch.Groups[1].Value;
                }
            }

            if (colorSpec is null)
            {
                var fontMatch = Regex.Match(token, @"^<font\b[^>]*\bcolor\s*=\s*(['""]?)([^'"">\s]+)\1[^>]*>$", RegexOptions.IgnoreCase);
                if (fontMatch.Success)
                {
                    colorSpec = fontMatch.Groups[2].Value;
                }
            }

            if (colorSpec is null)
            {
                var spanMatch = Regex.Match(token, @"^<span\b[^>]*style\s*=\s*(['""])[^'""]*color\s*:\s*([^;'""]+)[^'""]*\1[^>]*>$", RegexOptions.IgnoreCase);
                if (spanMatch.Success)
                {
                    colorSpec = spanMatch.Groups[2].Value;
                }
            }

            if (colorSpec is null || !TryParseHtmlColor(colorSpec, out var color))
            {
                return false;
            }

            colorIndex = GetOrAddExtraColorIndex(extraColors, color);
            return true;
        }

        private static bool TryParseHtmlColor(string rawValue, out System.Drawing.Color color)
        {
            color = System.Drawing.Color.Empty;

            var value = WebUtility.HtmlDecode(rawValue).Trim().Trim('"', '\'');
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (Regex.Match(value, @"^rgb\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*\)$", RegexOptions.IgnoreCase) is { Success: true } rgbMatch)
            {
                color = System.Drawing.Color.FromArgb(
                    ClampColorByte(rgbMatch.Groups[1].Value),
                    ClampColorByte(rgbMatch.Groups[2].Value),
                    ClampColorByte(rgbMatch.Groups[3].Value));
                return true;
            }

            try
            {
                color = System.Drawing.ColorTranslator.FromHtml(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static int ClampColorByte(string value)
            => Math.Clamp(int.Parse(value, CultureInfo.InvariantCulture), 0, 255);

        private static int GetOrAddExtraColorIndex(List<Color> extraColors, Color color)
        {
            for (int i = 0; i < extraColors.Count; i++)
            {
                if (extraColors[i].ToArgb() == color.ToArgb())
                {
                    return 10 + i;
                }
            }

            extraColors.Add(color);
            return 10 + extraColors.Count - 1;
        }
    }
}
