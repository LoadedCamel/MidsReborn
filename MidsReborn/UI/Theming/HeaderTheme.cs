using System.Text.Json.Serialization;

namespace Mids_Reborn.UI.Theming;

public class HeaderTheme
{
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color HeaderLight { get; set; }
    
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color HeaderMid { get; set; }
    
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color HeaderDark { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color WindowIcon { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color WindowIconHover { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color WindowIconPressed { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color WindowIconCloseHover { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color WindowIconClosePressed { get; set; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color LogoTargetColor { get; set; }
}