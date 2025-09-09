namespace Mids_Reborn.UI.Forms.Controls
{
    public sealed partial class CheckedListExt : CheckedListBox
    {
        public CheckedListExt()
        {
            ItemHeight = Font.Height + 10;
        }

        public override int ItemHeight { get; set; }
    }
}
