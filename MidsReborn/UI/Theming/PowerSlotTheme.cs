using System.Text.Json.Serialization;

namespace Mids_Reborn.UI.Theming;

public class PowerSlotTheme : ControlThemeBase
{
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color OpenBorder { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color EmptyFill { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color DisabledFill { get; set; }
}