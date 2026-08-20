using System.Text.Json.Serialization;

namespace Mids_Reborn.UI.Theming;

public class FooterTheme
{
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Background { get; set; } = Color.FromArgb(6, 17, 35);

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color SummaryText { get; set; } = Color.WhiteSmoke;

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color TotalSlotsText { get; set; } = Color.WhiteSmoke;

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color SlotsLeftText { get; set; } = Color.FromArgb(115, 255, 110);
}
