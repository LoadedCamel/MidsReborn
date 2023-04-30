using System;
using System.Drawing;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public static class TagGenerator
    {
        public static string Size(TagsFormatType formatType, int size, bool close = false)
        {
            switch (formatType)
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

        public static string Bold(TagsFormatType formatType, bool close = false)
        {
            return formatType switch
            {
                TagsFormatType.Markdown => "**",
                TagsFormatType.BBCode => close ? "[/b]" : "[b]",
                TagsFormatType.HTML => close ? "</strong>" : "<strong>",
                _ => ""
            };
        }

        public static string Italic(TagsFormatType formatType, bool close = false)
        {
            return formatType switch
            {
                TagsFormatType.Markdown => "*",
                TagsFormatType.BBCode => close ? "[/i]" : "[i]",
                TagsFormatType.HTML => close ? "</em>" : "<em>",
                _ => ""
            };
        }

        public static string Underline(TagsFormatType formatType, bool close = false)
        {
            return formatType switch
            {
                TagsFormatType.BBCode => close ? "[/u]" : "[u]",
                TagsFormatType.HTML => close ? "</span>" : "<span style=\"text-decoration: underline;\">",
                _ => "" // Doable in Markdown ?
            };
        }

        public static string Color(TagsFormatType formatType, Color color, bool close = false)
        {
            return formatType switch
            {
                TagsFormatType.BBCode => close ? "[/color]" : $"[color=\"{ForumColorThemes.ColorToHex(color)}\"]",
                TagsFormatType.HTML => close ? "</span>" : $"<span style=\"color: {ForumColorThemes.ColorToHex(color)};\">",
                _ => ""
            };
        }
    }
}
