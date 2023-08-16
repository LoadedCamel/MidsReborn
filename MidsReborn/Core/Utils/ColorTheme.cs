using System.Drawing;

namespace Mids_Reborn.Core.Utils
{
    public class ColorTheme
    {
        public string? Name { get; set; }
        public Color Title { get; set; } = Color.Empty;
        public Color Headings { get; set; } = Color.Empty;
        public Color Levels { get; set; } = Color.Empty;
        public Color Slots { get; set; } = Color.Empty;
        public bool DarkTheme { get; set; }
    }
}
