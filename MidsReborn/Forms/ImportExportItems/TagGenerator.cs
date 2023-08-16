using System;
using System.Drawing;
using Mids_Reborn.Core.Base.Extensions;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public class TagGenerator
    {
        private readonly ExportFormatType _formatType;
        private readonly ColorTheme _activeTheme;

        public TagGenerator(ExportFormatType formatType, ColorTheme activeTheme)
        {
            _formatType = formatType;
            _activeTheme = activeTheme;
        }

        public string Header(string title, bool fullPage = true)
        {
            return _formatType switch
            {
                ExportFormatType.Html when fullPage => @$"<!DOCTYPE html>
<html>
<head>
<meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
<title>{title}</title>
<style type=""text/css"">
html, body {{
  background-color: {(_activeTheme.DarkTheme ? "#000" : "#FFF")};
  color: {_activeTheme.Title.ToHex()};
  font-family: 'Segoe UI', 'Microsoft Sans Serif', sans-serif;
  font-size: 10pt;
}}

hr {{
  border-top: 1px solid {_activeTheme.Title.ToHex()};
}}

ul {{
  list-style: none;
}}

ul li::before {{
  content: ""\2022"";
  color: {_activeTheme.Title.ToHex()};
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
            return _formatType switch
            {
                ExportFormatType.Html when fullPage => @"
</body>
</html>",
                _ => ""
            };
        }

        public string Size(int size, bool close = false)
        {
            switch (_formatType)
            {
                case ExportFormatType.Markdown:
                    return close ? "\r\n" : new string('#', Math.Max(1, Math.Min(6, 7 - size))) + " ";

                case ExportFormatType.BbCode:
                    return close ? "[/size]" : $"[size=\"{size}\"]";

                case ExportFormatType.Html:
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
            return _formatType switch
            {
                ExportFormatType.Markdown => "**",
                ExportFormatType.BbCode => close ? "[/b]" : "[b]",
                ExportFormatType.Html => close ? "</strong>" : "<strong>",
                _ => ""
            };
        }

        public string Bold(string text)
        {
            return $"{Bold()}{text}{Bold(true)}";
        }

        public string Italic(bool close = false)
        {
            return _formatType switch
            {
                ExportFormatType.Markdown => "*",
                ExportFormatType.BbCode => close ? "[/i]" : "[i]",
                ExportFormatType.Html => close ? "</em>" : "<em>",
                _ => ""
            };
        }

        public string Italic(string text)
        {
            return $"{Italic()}{text}{Italic(true)}";
        }

        public string Underline(bool close = false)
        {
            return _formatType switch
            {
                ExportFormatType.BbCode => close ? "[/u]" : "[u]",
                ExportFormatType.Html => close ? "</span>" : "<span style=\"text-decoration: underline;\">",
                ExportFormatType.Markdown => "__",
                _ => ""
            };
        }

        public string Underline(string text)
        {
            return $"{Underline()}{text}{Underline(true)}";
        }

        // Markdown supports inline Html color tags.
        // See https://stackoverflow.com/a/35485694
        public string Color(Color color)
        {
            return _formatType switch
            {
                ExportFormatType.BbCode => $"[color=\"{color.ToHex()}\"]",
                ExportFormatType.Html or ExportFormatType.MarkdownHtml => $"<span style=\"color: {color.ToHex()};\">",
                _ => ""
            };
        }

        public string Color()
        {
            return _formatType switch
            {
                ExportFormatType.BbCode => "[/color]",
                ExportFormatType.Html or ExportFormatType.MarkdownHtml => "</span>",
                _ => ""
            };
        }

        public string Color(Color color, string text)
        {
            return $"{Color(color)}{text}{Color()}";
        }

        public string List(bool close = false)
        {
            return _formatType switch
            {
                ExportFormatType.BbCode => close ? "[/list]" : "[list]",
                ExportFormatType.Html => close ? "</ul>\r\n" : "<ul>",
                _ => ""
            };
        }

        public string List(string text)
        {
            return $"{List()}{text}{List(true)}";
        }

        public string ListItem(bool close = false)
        {
            return _formatType switch
            {
                ExportFormatType.BbCode => close ? "\r\n" : "[*] ",
                ExportFormatType.Html => close ? "</li>\r\n" : "<li>",
                ExportFormatType.Markdown => close ? "\r\n" : "- ",
                _ => ""
            };
        }

        public string ListItem(string text)
        {
            return $"{ListItem()}{text}{ListItem(true)}";
        }

        public string BlankLine()
        {
            return _formatType switch
            {
                ExportFormatType.Html => "<br />\r\n",
                _ => "\r\n"
            };
        }

        public string SeparatorLine()
        {
            return _formatType switch
            {
                ExportFormatType.Html or ExportFormatType.MarkdownHtml => "<br /><hr /><br />\r\n",
                ExportFormatType.BbCode => "\r\n──────────────────────────────\r\n", // U+2500 Box Drawings Light Horizontal
                ExportFormatType.Markdown => "\r\n\r\n----\r\n\r\n",
                ExportFormatType.None => "\r\n──────────────────────────────\r\n",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
