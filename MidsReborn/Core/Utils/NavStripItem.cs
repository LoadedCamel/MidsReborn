using System.Drawing;
using Mids_Reborn.Controls;

namespace Mids_Reborn.Core.Utils
{
    public class NavStripItem
    {
        public Page? Page { get; set; }
        public Rectangle Bounds { get; set; }
        public bool IsHighlighted { get; set; }
        public string? Text => Page?.Title;

        private NavItemState _state;

        public NavItemState State
        {
            get => _state;
            set => _state = Page?.Enabled == false ? NavItemState.Disabled : value;
        }

        public NavStripItem()
        {
            Page = null;
            State = NavItemState.Inactive;
        }

        public NavStripItem(Page page)
        {
            Page = page;
            State = page.Enabled ? NavItemState.Inactive : NavItemState.Disabled;
        }

        public NavStripItem(Page page, NavItemState state)
        {
            Page = page;
            State = page.Enabled ? state : NavItemState.Disabled;
        }
    }
}
