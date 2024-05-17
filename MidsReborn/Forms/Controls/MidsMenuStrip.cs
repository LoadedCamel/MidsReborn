using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public partial class MidsMenuStrip : MenuStrip
    {
        public MidsMenuStrip()
        {
            InitializeComponent();
            Renderer = new MidsMenuStripRenderer();
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
        // Background color of the menu bar and the dropdown menus
        public override Color MenuStripGradientBegin => Color.FromArgb(44, 47, 51);
        public override Color MenuStripGradientEnd => Color.FromArgb(44, 47, 51);

        // Background color of a menu item that is highlighted
        public override Color MenuItemSelected => Color.FromArgb(3, 111, 160);
        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(3, 111, 160);
        public override Color MenuItemSelectedGradientEnd => Color.FromArgb(3, 111, 160);

        // Border color of a menu item that is highlighted
        public override Color MenuItemBorder => Color.FromArgb(3, 111, 160);

        // Background color for menu items that are on a drop-down when they are highlighted
        public override Color MenuItemPressedGradientBegin => Color.FromArgb(3, 111, 160);
        public override Color MenuItemPressedGradientEnd => Color.FromArgb(3, 111, 160);

        // Background color of the drop-down menu
        public override Color ToolStripDropDownBackground => Color.FromArgb(44, 47, 51);

        // Separator color
        public override Color SeparatorDark => Color.Gold;
        public override Color SeparatorLight => Color.WhiteSmoke;

        // Button-selected gradient (for ToolStrip)
        public override Color ButtonSelectedGradientBegin => Color.FromArgb(3, 111, 160);
        public override Color ButtonSelectedGradientEnd => Color.FromArgb(3, 111, 160);
        public override Color ButtonSelectedBorder => Color.FromArgb(3, 111, 160);

        // Button-pressed gradient (for ToolStrip)
        public override Color ButtonPressedGradientBegin => Color.FromArgb(3, 111, 160);
        public override Color ButtonPressedGradientEnd => Color.FromArgb(3, 111, 160);

        // Button-checked gradient (for ToolStrip when an item is checked or toggled)
        public override Color CheckBackground => Color.FromArgb(44, 47, 51);
        public override Color CheckSelectedBackground => Color.FromArgb(3, 111, 160);
        public override Color CheckPressedBackground => Color.FromArgb(3, 111, 160);

        // Overflow button colors (if applicable)
        public override Color OverflowButtonGradientBegin => Color.FromArgb(44, 47, 51);
        public override Color OverflowButtonGradientEnd => Color.FromArgb(44, 47, 51);

        // Grip (handle) colors for movable toolstrips
        public override Color GripDark => Color.Gold; // Dark part of the grip
        public override Color GripLight => Color.WhiteSmoke; // Light part of the grip

        // Image margin (background area around images in a toolbar)
        public override Color ImageMarginGradientBegin => Color.FromArgb(44, 47, 51);
        public override Color ImageMarginGradientMiddle => Color.FromArgb(44, 47, 51);
        public override Color ImageMarginGradientEnd => Color.FromArgb(44, 47, 51);

        // Status strip background
        public override Color StatusStripGradientBegin => Color.FromArgb(44, 47, 51);
        public override Color StatusStripGradientEnd => Color.FromArgb(44, 47, 51);
    }

    public class MidsMenuStripRenderer : ToolStripProfessionalRenderer
    {
        public MidsMenuStripRenderer() : base(new MidsMenuColorTable())
        {
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            // Check if the item is a top-level menu item
            if (e.Item is ToolStripMenuItem menuItem)
            {
                if (menuItem.DropDown.Visible)  // Dropdown is open
                {
                    e.TextColor = Color.FromArgb(239, 239, 239); // Keep text black when dropdown is open
                }
                else if (menuItem.Selected)  // Hovered but dropdown not necessarily open
                {
                    e.TextColor = Color.FromArgb(239, 239, 239); // Text color black when item is selected/hovered
                }
                else
                {
                    e.TextColor = Color.WhiteSmoke; // Default color when not selected
                }
            }
            else
            {
                // Non-menu item texts, could be used for other types of items
                e.TextColor = e.Item.Selected ? Color.FromArgb(239, 239, 239) : Color.WhiteSmoke;
            }

            base.OnRenderItemText(e);
        }
    }
}
