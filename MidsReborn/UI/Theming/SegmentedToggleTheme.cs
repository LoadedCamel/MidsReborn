using System.Text.Json.Serialization;

namespace Mids_Reborn.UI.Theming;

public class SegmentedToggleTheme
{
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color WellTop { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color WellBottom { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Divider { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color SelectedTop { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color SelectedBottom { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color SelectedBorder { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color SelectedText { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color SelectedTextOutline { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color UnselectedText { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color UnselectedTextOutline { get; set; }
}
