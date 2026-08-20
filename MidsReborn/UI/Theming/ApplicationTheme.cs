namespace Mids_Reborn.UI.Theming;

/*
Rev 1.1 (08/19/26):
- Expanded ListViewTheme to apply on power lists (MidsListView, ctlPowerList, ListLabel)
- Added Footer theme
*/

public class ApplicationTheme
{
    public required string Name { get; set; }
    public string? Version { get; set; } = "1.1";
    public required ButtonTheme Button { get; set; }
    public required DropDownListTheme DropDownList { get; set; }
    public required HeaderTheme Header { get; set; }
    public required ListViewTheme ListView { get; set; }
    public required MenuStripTheme MenuStrip { get; set; }
    public SegmentedToggleTheme? SegmentedToggle { get; set; }
    public required PowerSlotTheme PowerSlot { get; set; }
    public required ScrollPanelTheme ScrollPanel { get; set; }
    public required DataViewTheme DataView { get; set; }
    public FooterTheme? Footer { get; set; }
}
