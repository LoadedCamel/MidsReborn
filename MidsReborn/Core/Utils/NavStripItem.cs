using System.Drawing;
using MetaControls;
using Mids_Reborn.Controls;

namespace Mids_Reborn.Core.Utils
{
    public class NavStripItem
    {
        public Page? Page { get; set; }
        public Rectangle Bounds { get; set; }
        public bool IsHighlighted { get; set; }
        public NavItemState State { get; set; }
        public string? Text => Page?.Title;

        public NavStripItem()
        {
            Page = null;
            State = NavItemState.Inactive;
        }

        public NavStripItem(Page page)
        {
            Page = page;
            State = NavItemState.Inactive;
        }

        public NavStripItem(Page page, NavItemState state)
        {
            Page = page;
            State = state;
        }
    }
}
