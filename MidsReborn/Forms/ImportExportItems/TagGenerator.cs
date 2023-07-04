using System;
using System.Drawing;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public class TagGenerator
    {
        private TagsFormatType FormatType;
        private ForumColorTheme ActiveTheme;

        public TagGenerator(TagsFormatType formatType, ForumColorTheme activeTheme)
        {
            FormatType = formatType;
            ActiveTheme = activeTheme;
        }

        public string Header(string title, bool fullPage = true)
        {
            return FormatType switch
            {
                TagsFormatType.HTML when fullPage => @$"<!DOCTYPE html>
<html>
<head>
<meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
<title>{title}</title>
<style type=""text/css"">
html, body {{
  background-color: {(ActiveTheme.DarkTheme ? "#000" : "#FFF")};
  color: {ForumColorThemes.ColorToHex(ActiveTheme.Text)};
  font-family: 'Segoe UI', 'Microsoft Sans Serif', sans-serif;
  font-size: 10pt;
}}

hr {{
  border-top: 1px solid {ForumColorThemes.ColorToHex(ActiveTheme.Text)};
}}

ul {{
  list-style: none;
}}

ul li::before {{
  content: ""\2022"";
  color: {ForumColorThemes.ColorToHex(ActiveTheme.Text)};
  font-weight: bold;
  font-size: 12pt;
  display: inline-block; 
  width: 1em;
  margin-left: -1em;
}}
</style>
</head>
<body>
",
                _ => ""
            };
        }

        public string Footer(bool fullPage = true)
        {
            return FormatType switch
            {
                TagsFormatType.HTML when fullPage => @"
</body>
</html>",
                _ => ""
            };
        }

        public string Size(int size, bool close = false)
        {
            switch (FormatType)
            {
                case TagsFormatType.Markdown:
                    return close ? "\r\n" : new string('#', Math.Max(1, Math.Min(6, 7 - size))) + " ";

                case TagsFormatType.BBCode:
                    return close ? "[/size]" : $"[size=\"{size}\"]";

                case TagsFormatType.HTML:
                    var htmlSize = size switch
                    {
                        1 => 5,
                        2 => 7,
                        3 => 9,
                        4 => 10,
                        5 => 12,
                        6 => 14,
                        _ => 10
                    };

                    return close ? "</span>" : $"<span style=\"font-size: {htmlSize}pt;\">";

                default:
                    return "";
            }
        }

        public string Size(int size, string text)
        {
            return $"{Size(size)}{text}{Size(size, true)}";
        }

        public string Bold(bool close = false)
        {
            return FormatType switch
            {
                TagsFormatType.Markdown => "**",
                TagsFormatType.BBCode => close ? "[/b]" : "[b]",
                TagsFormatType.HTML => close ? "</strong>" : "<strong>",
                _ => ""
            };
        }

        public string Bold(string text)
        {
            return $"{Bold()}{text}{Bold(true)}";
        }

        public string Italic(bool close = false)
        {
            return FormatType switch
            {
                TagsFormatType.Markdown => "*",
                TagsFormatType.BBCode => close ? "[/i]" : "[i]",
                TagsFormatType.HTML => close ? "</em>" : "<em>",
                _ => ""
            };
        }

        public string Italic(string text)
        {
            return $"{Italic()}{text}{Italic(true)}";
        }

        public string Underline(bool close = false)
        {
            return FormatType switch
            {
                TagsFormatType.BBCode => close ? "[/u]" : "[u]",
                TagsFormatType.HTML => close ? "</span>" : "<span style=\"text-decoration: underline;\">",
                TagsFormatType.Markdown => "__",
                _ => ""
            };
        }

        public string Underline(string text)
        {
            return $"{Underline()}{text}{Underline(true)}";
        }

        // Markdown supports inline HTML color tags.
        // See https://stackoverflow.com/a/35485694
        public string Color(Color color)
        {
            return FormatType switch
            {
                TagsFormatType.BBCode => $"[color=\"{ForumColorThemes.ColorToHex(color)}\"]",
                TagsFormatType.HTML or TagsFormatType.MarkdownHTML => $"<span style=\"color: {ForumColorThemes.ColorToHex(color)};\">",
                _ => ""
            };
        }

        public string Color()
        {
            return FormatType switch
            {
                TagsFormatType.BBCode => "[/color]",
                TagsFormatType.HTML or TagsFormatType.MarkdownHTML => "</span>",
                _ => ""
            };
        }

        public string Color(Color color, string text)
        {
            return $"{Color(color)}{text}{Color()}";
        }

        public string List(bool close = false)
        {
            return FormatType switch
            {
                TagsFormatType.BBCode => close ? "[/list]" : "[list]",
                TagsFormatType.HTML => close ? "</ul>\r\n" : "<ul>",
                _ => ""
            };
        }

        public string List(string text)
        {
            return $"{List()}{text}{List(true)}";
        }

        public string ListItem(bool close = false)
        {
            return FormatType switch
            {
                TagsFormatType.BBCode => close ? "\r\n" : "[*] ",
                TagsFormatType.HTML => close ? "</li>\r\n" : "<li>",
                TagsFormatType.Markdown => close ? "\r\n" : "- ",
                _ => ""
            };
        }

        public string ListItem(string text)
        {
            return $"{ListItem()}{text}{ListItem(true)}";
        }

        public string BlankLine()
        {
            return FormatType switch
            {
                TagsFormatType.HTML => "<br />\r\n",
                _ => "\r\n"
            };
        }

        public string SeparatorLine()
        {
            return FormatType switch
            {
                TagsFormatType.HTML => "<br /><hr /><br />\r\n",
                TagsFormatType.BBCode => "\r\n──────────────────────────────\r\n", // U+2500 Box Drawings Light Horizontal
                TagsFormatType.Markdown => "\r\n\r\n----\r\n\r\n"
            };
        }
    }
}
