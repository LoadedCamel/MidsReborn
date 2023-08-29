using Mids_Reborn.Core.Utils;
using System.ComponentModel;
using System.Drawing;

namespace Mids_Reborn.Core
{
    public class ShareConfig
    {
        public class ForumFormatConfig
        {
            public BindingList<ColorTheme?> ColorThemes { get; private set; } = new();
            public ThemeFilter Filter { get; set; } = ThemeFilter.Any;
            public BindingList<FormatCode> FormatCodes { get; private set; } = new();

            public ColorTheme? SelectedTheme { get; set; }
            public FormatCode? SelectedFormatCode { get; set; }

            public bool InclIncarnates { get; set; }
            public bool InclAccolades { get; set; }
            public bool InclBonusBreakdown { get; set; }

            internal void ResetThemes()
            {
                ColorThemes = new BindingList<ColorTheme?>
                {
                    new()
                    {
                        Name = "Navy",
                        Title = Color.FromArgb(0x00, 0x00, 0xcd),
                        Headings = Color.FromArgb(0x00, 0x00, 0x80),
                        Levels = Color.FromArgb(0x48, 0x3d, 0x8b),
                        Slots = Color.FromArgb(0x48, 0x3d, 0x8b),
                        DarkTheme = false
                    },

                    new()
                    {
                        Name = "Light Blue",
                        Title = Color.FromArgb(0xb1, 0xc9, 0xf5),
                        Headings = Color.FromArgb(0x48, 0x9a, 0xff),
                        Levels = Color.FromArgb(0x4f, 0xa7, 0xff),
                        Slots = Color.FromArgb(0x5e, 0xae, 0xff),
                        DarkTheme = true
                    },

                    new()
                    {
                        Name = "Purple",
                        Title = Color.FromArgb(0x80, 0x00, 0x80),
                        Headings = Color.FromArgb(0x94, 0x00, 0xd3),
                        Levels = Color.FromArgb(0xba, 0x55, 0xd3),
                        Slots = Color.FromArgb(0x94, 0x00, 0xd3),
                        DarkTheme = false
                    },

                    new()
                    {
                        Name = "Light Purple",
                        Title = Color.FromArgb(0xcf, 0xb3, 0xff),
                        Headings = Color.FromArgb(0xbc, 0x9b, 0xff),
                        Levels = Color.FromArgb(0xaf, 0x8a, 0xfd),
                        Slots = Color.FromArgb(0xc2, 0xb4, 0xfc),
                        DarkTheme = true
                    },

                    new()
                    {
                        Name = "Orange",
                        Title = Color.FromArgb(0xff, 0x8c, 0x00),
                        Headings = Color.FromArgb(0xff, 0xa5, 0x00),
                        Levels = Color.FromArgb(0xff, 0x45, 0x00),
                        Slots = Color.FromArgb(0xb8, 0x86, 0x0b),
                        DarkTheme = false
                    },

                    new()
                    {
                        Name = "Olive Drab",
                        Title = Color.FromArgb(0x55, 0x6b, 0x2f),
                        Headings = Color.FromArgb(0x00, 0x80, 0x00),
                        Levels = Color.FromArgb(0x6b, 0x8e, 0x23),
                        Slots = Color.FromArgb(0x6b, 0x8e, 0x23),
                        DarkTheme = false
                    },

                    new()
                    {
                        Name = "Reds",
                        Title = Color.FromArgb(0x80, 0x00, 0x00),
                        Headings = Color.FromArgb(0xa8, 0x00, 0x00),
                        Levels = Color.FromArgb(0x84, 0x3f, 0x3c),
                        Slots = Color.FromArgb(0x6f, 0x00, 0x00),
                        DarkTheme = false
                    },

                    new()
                    {
                        Name = "Light Reds",
                        Title = Color.FromArgb(0xff, 0x6a, 0x6a),
                        Headings = Color.FromArgb(0xff, 0x00, 0x00),
                        Levels = Color.FromArgb(0xff, 0x6c, 0x6c),
                        Slots = Color.FromArgb(0xff, 0x80, 0x80),
                        DarkTheme = true
                    },

                    new()
                    {
                        Name = "Fruit Salad",
                        Title = Color.FromArgb(0xff, 0xa5, 0x00),
                        Headings = Color.FromArgb(0x1e, 0x90, 0xff),
                        Levels = Color.FromArgb(0x32, 0xcd, 0x32),
                        Slots = Color.FromArgb(0xff, 0xd7, 0x00),
                        DarkTheme = true
                    },

                    new()
                    {
                        Name = "Pink",
                        Title = Color.FromArgb(0xff, 0x80, 0xc0),
                        Headings = Color.FromArgb(0xff, 0x80, 0xff),
                        Levels = Color.FromArgb(0xff, 0x80, 0xff),
                        Slots = Color.FromArgb(0xff, 0xae, 0xff),
                        DarkTheme = true
                    }
                };
            }
            internal void ResetCodes()
            {
                FormatCodes = new BindingList<FormatCode>
                {
                    new("BBCode", ExportFormatType.BbCode),
                    new("HTML", ExportFormatType.Html),
                    new("Markdown", ExportFormatType.Markdown),
                    new("Markdown+HTML", ExportFormatType.MarkdownHtml),
                    new("Plain Text", ExportFormatType.None)
                };
            }
            internal void AddTheme(string name, bool isDarkTheme = false)
            {
                ColorThemes.Add(new ColorTheme
                {
                    Name = name,
                    Title = Color.Empty,
                    Headings = Color.Empty,
                    Levels = Color.Empty,
                    Slots = Color.Empty,
                    DarkTheme = isDarkTheme
                });
            }
            internal void AddTheme(ColorTheme theme)
            {
                ColorThemes.Add(theme);
            }
            internal void RemoveTheme(int index)
            {
                ColorThemes.RemoveAt(index);
            }
        }
        public class InfoGraphicConfig
        {
            public bool UseAltImage { get; set; }
        }
        public class MobileFriendlyConfig
        {
            public bool InclAccolades { get; set; }
            public bool InclIncarnates { get; set; }
            public bool InclSetBonus { get; set; }
            public bool InclSetBreakdown { get; set; }
        }

        public ForumFormatConfig ForumFormat { get; set; } = new();
        public InfoGraphicConfig InfoGraphic { get; set; } = new();
        public MobileFriendlyConfig MobileFriendly { get; set; } = new();
        public int LastPageIndex { get; set; } = -1;
    }
}
