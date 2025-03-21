using System.Drawing;

namespace Mids_Reborn.Core.Utils
{
    public class ColorTheme
    {
        public string? Name { get; set; } = "Light Blue";
        public Color Title { get; set; } = Color.FromArgb(0xb1, 0xc9, 0xf5);
        public Color Headings { get; set; } = Color.FromArgb(0x48, 0x9a, 0xff);
        public Color Levels { get; set; } = Color.FromArgb(0x4f, 0xa7, 0xff);
        public Color Slots { get; set; } = Color.FromArgb(0x5e, 0xae, 0xff);
        public bool DarkTheme { get; set; } = true;
    }
}
