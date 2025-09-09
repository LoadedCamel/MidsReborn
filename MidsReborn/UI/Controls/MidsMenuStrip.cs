using Mids_Reborn.Core.Theming;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Controls
{
    public partial class MidsMenuStrip : MenuStrip
    {
        private MenuStripTheme CurrentTheme => DesignMode
            ? ThemeManager.DesignTime.MenuStrip
            : (ThemeManager.CurrentTheme?.MenuStrip ?? ThemeManager.DesignTime.MenuStrip);

        public MidsMenuStrip()
        {
            InitializeComponent();
            ApplyTheme();

            if (!DesignMode)
            {
                ThemeManager.ThemeChanged += OnThemeChanged;
            }
        }

        private void ApplyTheme()
        {
            // Create a new renderer with the current theme's colors
            Renderer = new MidsMenuStripRenderer(CurrentTheme);
        }

        private void OnThemeChanged()
        {
            ApplyTheme();
            Invalidate();
        }

        protected override void OnItemAdded(ToolStripItemEventArgs e)
        {
            base.OnItemAdded(e);

            e.Item.MouseEnter += OnMenuItemMouseEnter;

            // Check if the item is a dropdown item and attach handlers to its children
            if (e.Item is not ToolStripMenuItem { HasDropDownItems: true } menuItem) return;
            foreach (ToolStripItem subItem in menuItem.DropDownItems)
            {
                subItem.MouseEnter += OnMenuItemMouseEnter;
            }
        }

        private void OnMenuItemMouseEnter(object? sender, EventArgs e)
        {
            Invalidate();
        }
    }

    internal class MidsMenuColorTable : ProfessionalColorTable
    {
        private readonly MenuStripTheme _theme;

        public MidsMenuColorTable(MenuStripTheme theme)
        {
            _theme = theme;
        }

        public override Color MenuItemSelected => _theme.ItemSelectedColor;
        public override Color MenuItemSelectedGradientBegin => _theme.ItemSelectedColor;
        public override Color MenuItemSelectedGradientEnd => _theme.ItemSelectedColor;
        public override Color MenuItemBorder => _theme.ItemSelectedColor;
        public override Color MenuItemPressedGradientBegin => _theme.ItemSelectedColor;
        public override Color MenuItemPressedGradientEnd => _theme.ItemSelectedColor;
        public override Color ButtonSelectedGradientBegin => _theme.ItemSelectedColor;
        public override Color ButtonSelectedGradientEnd => _theme.ItemSelectedColor;
        public override Color ButtonSelectedBorder => _theme.ItemSelectedColor;
        public override Color ButtonPressedGradientBegin => _theme.ItemSelectedColor;
        public override Color ButtonPressedGradientEnd => _theme.ItemSelectedColor;
        public override Color CheckSelectedBackground => _theme.ItemSelectedColor;
        public override Color CheckPressedBackground => _theme.ItemSelectedColor;
        public override Color SeparatorDark => _theme.AccentColor;
        public override Color GripDark => _theme.AccentColor;
        public override Color SeparatorLight => _theme.AccentLightColor;
        public override Color GripLight => _theme.AccentLightColor;

        public override Color MenuStripGradientBegin => Color.FromArgb(44, 47, 51);
        public override Color MenuStripGradientEnd => Color.FromArgb(44, 47, 51);
        public override Color ToolStripDropDownBackground => Color.FromArgb(44, 47, 51);
        public override Color CheckBackground => Color.FromArgb(44, 47, 51);
        public override Color OverflowButtonGradientBegin => Color.FromArgb(44, 47, 51);
        public override Color OverflowButtonGradientEnd => Color.FromArgb(44, 47, 51);
        public override Color ImageMarginGradientBegin => Color.FromArgb(44, 47, 51);
        public override Color ImageMarginGradientMiddle => Color.FromArgb(44, 47, 51);
        public override Color ImageMarginGradientEnd => Color.FromArgb(44, 47, 51);
        public override Color StatusStripGradientBegin => Color.FromArgb(44, 47, 51);
        public override Color StatusStripGradientEnd => Color.FromArgb(44, 47, 51);
    }

    public class MidsMenuStripRenderer : ToolStripProfessionalRenderer
    {
        public MidsMenuStripRenderer(MenuStripTheme theme) : base(new MidsMenuColorTable(theme)) { }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using var brush = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
            e.Graphics.FillRectangle(brush, e.AffectedBounds);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            string text = e.Text;
            Font font = e.TextFont;
            Rectangle rect = e.TextRectangle;
            Graphics g = e.Graphics;

            // Determine text color
            Color textColor = Color.WhiteSmoke;
            if (e.Item is ToolStripMenuItem menuItem)
            {
                if (menuItem.DropDown.Visible || menuItem.Selected)
                    textColor = Color.FromArgb(239, 239, 239);
            }
            else if (e.Item.Selected)
            {
                textColor = Color.FromArgb(239, 239, 239);
            }

            // Shadow color and position
            Color shadowColor = Color.FromArgb(160, 0, 0, 0); // Semi-transparent black
            Point shadowOffset = new Point(rect.X + 1, rect.Y + 1);

            // Draw shadow
            TextRenderer.DrawText(g, text, font, shadowOffset, shadowColor, TextFormatFlags.Default);

            // Draw actual text
            TextRenderer.DrawText(g, text, font, rect.Location, textColor, TextFormatFlags.Default);
        }

        // Override sub-menu arrow color
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            if (e.Item is ToolStripMenuItem)
            {
                e.ArrowColor = Color.FromArgb(239, 239, 239);
            }

            base.OnRenderArrow(e);
        }
    }
}
