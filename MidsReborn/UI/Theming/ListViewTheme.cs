using System.Text.Json.Serialization;

namespace Mids_Reborn.UI.Theming;

public class ListViewTheme
{
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color ScrollBar { get; set; } // Track color

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color ScrollButton { get; set; } // Thumb and arrow color
}