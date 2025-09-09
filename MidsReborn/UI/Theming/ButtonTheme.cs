using System.Text.Json.Serialization;

namespace Mids_Reborn.UI.Theming;

public class ButtonTheme : ControlThemeBase
{
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color PressedGradientTop { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color PressedGradientBottom { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color TextOutlineColor { get; set; }
    public float TextOutlineWidth { get; set; } = 2f;
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color ToggledBorderColor { get; set; }
    public float ToggledBorderWidth { get; set; } = 2f;
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color ToggledGradientTop { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color ToggledGradientBottom { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color ToggledTextColor { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color ToggledTextOutlineColor { get; set; }
}