using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mids_Reborn.UI.Theming;

public class ColorJsonConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        if (string.IsNullOrWhiteSpace(s)) return Color.Empty;

        s = s.Trim();
        if (s[0] == '#') s = s[1..];

        if (s.Length == 8) // AARRGGBB
        {
            byte a = Convert.ToByte(s.Substring(0, 2), 16);
            byte r = Convert.ToByte(s.Substring(2, 2), 16);
            byte g = Convert.ToByte(s.Substring(4, 2), 16);
            byte b = Convert.ToByte(s.Substring(6, 2), 16);
            return Color.FromArgb(a, r, g, b);
        }
        if (s.Length == 6) // RRGGBB
        {
            byte r = Convert.ToByte(s.Substring(0, 2), 16);
            byte g = Convert.ToByte(s.Substring(2, 2), 16);
            byte b = Convert.ToByte(s.Substring(4, 2), 16);
            return Color.FromArgb(255, r, g, b);
        }
        // Fallback to named colors, etc.
        return ColorTranslator.FromHtml('#' + s);
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        string hexColor = $"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}";
        writer.WriteStringValue(hexColor);
    }
}