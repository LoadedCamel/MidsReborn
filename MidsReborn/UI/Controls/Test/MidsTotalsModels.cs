namespace Mids_Reborn.UI.Controls.Test;

internal readonly record struct TotalsQuickMetric(
    string Label,
    string Value,
    string? Detail,
    string Tooltip,
    MidsTotalsGlyph Icon,
    Color AccentColor);

internal readonly record struct TotalsBarMetric(
    string Label,
    string Value,
    float BarValue,
    float MarkerValue,
    string Tooltip,
    MidsTotalsGlyph Icon,
    Color AccentColor);

internal readonly record struct TotalsValueMetric(
    string Label,
    string Value,
    string Tooltip,
    MidsTotalsGlyph Icon,
    Color AccentColor);
