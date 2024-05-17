using System;
using Mids_Reborn.Core.BuildFile.DataModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mids_Reborn.Core.BuildFile
{
    public class EnhancementDataConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is not EnhancementData eData) return;

            var jsonObject = new JObject
            {
                { "Uid", eData.Uid },
                { "Grade", eData.Grade },
                { "IoLevel", eData.IoLevel },
                { "RelativeLevel", eData.RelativeLevel },
                { "Obtained", eData.Obtained }
            };

            jsonObject.WriteTo(writer);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            // Handle null JSON token, which signifies no data to load for this slot
            if (reader.TokenType == JsonToken.Null)
            {
                return null; // Return null to indicate no enhancement data
            }

            var jsonObject = JObject.Load(reader);
            var enhancementData = new EnhancementData();

            if (jsonObject.TryGetValue("Uid", out var uidValue))
            {
                enhancementData.Uid = uidValue.ToObject<string>() ?? throw new InvalidOperationException();
            }
            else if (jsonObject.TryGetValue("Enhancement", out var enhancementValue))
            {
                enhancementData.Uid = DatabaseAPI.GetEnhancementUid(enhancementValue.ToObject<string>());
            }
            if (jsonObject.TryGetValue("Grade", out var gradeValue))
            {
                enhancementData.Grade = gradeValue.ToObject<string>() ?? throw new InvalidOperationException();
            }

            if (jsonObject.TryGetValue("IoLevel", out var ioLevelValue))
            {
                enhancementData.IoLevel = ioLevelValue.ToObject<int>();
            }

            if (jsonObject.TryGetValue("RelativeLevel", out var relativeLevelValue))
            {
                enhancementData.RelativeLevel = relativeLevelValue.ToObject<string>() ?? throw new InvalidOperationException();
            }

            if (jsonObject.TryGetValue("Obtained", out var obtainedValue))
            {
                enhancementData.Obtained = obtainedValue.ToObject<bool>();
            }

            return enhancementData;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EnhancementData);
        }
    }
}
