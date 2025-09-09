using System.Text.Json.Serialization;

namespace Mids_Reborn.UI.Theming;

public class MenuStripTheme
{
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color ItemSelectedColor { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color AccentColor { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color AccentLightColor { get; set; }
}