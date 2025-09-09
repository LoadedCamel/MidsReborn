using System.Text.Json.Serialization;

namespace Mids_Reborn.UI.Theming;

public sealed class DataViewTheme
{
    // Surface & accents
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Background { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Card { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Border { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Accent { get; init; }

    // Header tabs
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color HeaderTop { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color HeaderBottom { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color TabActiveTop { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color TabActiveBottom { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color TabInactiveTop { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color TabInactiveBottom { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color TabBorder { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Text { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color ValueText { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Muted { get; init; }

    // Controls
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Chip { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color ChipActive { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Rail { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color RailFill { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color Thumb { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color ThumbBorder { get; init; }

    // Grid
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color GridHeaderTop { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color GridHeaderBottom { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color GridHeaderBorder { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color GridRowEven { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color GridRowOdd { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color GridRowLine { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color GridBandLow { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color GridBandMid { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color GridBandHigh { get; init; }

    [JsonConverter(typeof(ColorJsonConverter))]
    public Color GridNeutral { get; init; }
}