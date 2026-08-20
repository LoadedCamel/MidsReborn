using System.Text.Json.Serialization;

namespace Mids_Reborn.UI.Theming;

public class ListViewTheme
{
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color ScrollBar { get; set; } // Track color

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color ScrollButton { get; set; } // Thumb and arrow color

    // MidsListView and similar default colors/states
    // Color.LightBlue, Color.LightGreen, Color.LightGray, Color.DarkGreen, Color.Red, Color.Orange
    // Enabled, Selected, Disabled, SelectedDisabled, Invalid, Heading
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color? Enabled { get; set; } = Color.LightBlue;
    
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color? Selected { get; set; } = Color.LightGreen;
    
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color? Disabled { get; set; } = Color.LightGray;
    
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color? SelectedDisabled { get; set; } = Color.DarkGreen;
    
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color? Invalid { get; set; } = Color.Red;
    
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color? Heading { get; set; } = Color.Orange;
}