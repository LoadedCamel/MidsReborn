using System.Text.Json.Serialization;

namespace Mids_Reborn.UI.Theming;

public class ScrollPanelTheme
{
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Bar { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Hover { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Track { get; set; }
}