using System.Collections.Generic;
using System.Drawing;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public enum TagsFormatType
    {
        None,
        BBCode,
        MarkdownHTML,
        Markdown,
        HTML
    }

    public class ForumColorTheme
    {
        public string Name;
        public Color Text;
        public Color Headings;
        public Color Levels;
        public Color Slots;
        public bool DarkTheme;
    }

    public struct ForumColorsHex
    {
        public string Text;
        public string Headings;
        public string Levels;
        public string Slots;
    }

    public static class ForumColorThemes
    {
        public static List<ForumColorTheme> GetThemes()
        {
            return new List<ForumColorTheme>
            {
                new()
                {
                    Name = "Navy",
                    Text = Color.FromArgb(0x00, 0x00, 0xcd),
                    Headings = Color.FromArgb(0x00, 0x00, 0x80),
                    Levels = Color.FromArgb(0x48, 0x3d, 0x8b),
                    Slots = Color.FromArgb(0x48, 0x3d, 0x8b),
                    DarkTheme = false
                },

                new()
                {
                    Name = "Light Blue",
                    Text = Color.FromArgb(0xb1, 0xc9, 0xf5),
                    Headings = Color.FromArgb(0x48, 0x9a, 0xff),
                    Levels = Color.FromArgb(0x4f, 0xa7, 0xff),
                    Slots = Color.FromArgb(0x5e, 0xae, 0xff),
                    DarkTheme = true
                },

                new()
                {
                    Name = "Purple",
                    Text = Color.FromArgb(0x80, 0x00, 0x80),
                    Headings = Color.FromArgb(0x94, 0x00, 0xd3),
                    Levels = Color.FromArgb(0xba, 0x55, 0xd3),
                    Slots = Color.FromArgb(0x94, 0x00, 0xd3),
                    DarkTheme = false
                },

                new()
                {
                    Name = "Light Purple",
                    Text = Color.FromArgb(0xcf, 0xb3, 0xff),
                    Headings = Color.FromArgb(0xbc, 0x9b, 0xff),
                    Levels = Color.FromArgb(0xaf, 0x8a, 0xfd),
                    Slots = Color.FromArgb(0xc2, 0xb4, 0xfc),
                    DarkTheme = true
                },

                new()
                {
                    Name = "Orange",
                    Text = Color.FromArgb(0xff, 0x8c, 0x00),
                    Headings = Color.FromArgb(0xff, 0xa5, 0x00),
                    Levels = Color.FromArgb(0xff, 0x45, 0x00),
                    Slots = Color.FromArgb(0xb8, 0x86, 0x0b),
                    DarkTheme = false
                },

                new()
                {
                    Name = "Olive Drab",
                    Text = Color.FromArgb(0x55, 0x6b, 0x2f),
                    Headings = Color.FromArgb(0x00, 0x80, 0x00),
                    Levels = Color.FromArgb(0x6b, 0x8e, 0x23),
                    Slots = Color.FromArgb(0x6b, 0x8e, 0x23),
                    DarkTheme = false
                },

                new()
                {
                    Name = "Reds",
                    Text = Color.FromArgb(0x80, 0x00, 0x00),
                    Headings = Color.FromArgb(0xa8, 0x00, 0x00),
                    Levels = Color.FromArgb(0x84, 0x3f, 0x3c),
                    Slots = Color.FromArgb(0x6f, 0x00, 0x00),
                    DarkTheme = false
                },

                new()
                {
                    Name = "Light Reds",
                    Text = Color.FromArgb(0xff, 0x6a, 0x6a),
                    Headings = Color.FromArgb(0xff, 0x00, 0x00),
                    Levels = Color.FromArgb(0xff, 0x6c, 0x6c),
                    Slots = Color.FromArgb(0xff, 0x80, 0x80),
                    DarkTheme = true
                },

                new()
                {
                    Name = "Fruit Salad",
                    Text = Color.FromArgb(0xff, 0xa5, 0x00),
                    Headings = Color.FromArgb(0x1e, 0x90, 0xff),
                    Levels = Color.FromArgb(0x32, 0xcd, 0x32),
                    Slots = Color.FromArgb(0xff, 0xd7, 0x00),
                    DarkTheme = true
                },

                new()
                {
                    Name = "Pink",
                    Text = Color.FromArgb(0xff, 0x80, 0xc0),
                    Headings = Color.FromArgb(0xff, 0x80, 0xff),
                    Levels = Color.FromArgb(0xff, 0x80, 0xff),
                    Slots = Color.FromArgb(0xff, 0xae, 0xff),
                    DarkTheme = true
                }
            };
        }

        public static string ColorToHex(Color color, bool sharpPrefix=true)
        {
            return $"{(sharpPrefix ? "#" : "")}{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}
