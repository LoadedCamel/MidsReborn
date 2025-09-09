using System.Text.Json.Serialization;

namespace Mids_Reborn.UI.Theming;

public abstract class ControlThemeBase
{
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color GradientTop { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color GradientBottom { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color HoverGradientTop { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color HoverGradientBottom { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Border { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color ForeColor { get; set; }
}