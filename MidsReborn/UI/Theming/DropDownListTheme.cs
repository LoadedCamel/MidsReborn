using System.Text.Json.Serialization;

namespace Mids_Reborn.UI.Theming;

public class DropDownListTheme : ControlThemeBase
{
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color HoverBorder { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color FocusBorder { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Arrow { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color HoverArrow { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color LockColor { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color DropDownBackColor { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color DropDownSelectionBackColor { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color DropDownSelectionForeColor { get; set; }
}