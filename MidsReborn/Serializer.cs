using System;
using mrbBase;
using mrbBase.Base.Data_Classes;
using Newtonsoft.Json;

namespace Mids_Reborn
{

    internal class Serializer : ISerialize
    {
        private readonly Func<object, string> _serializeFunc;

        public Serializer(Func<object, string> serializeFunc, string extension)
        {
            Extension = extension;
            _serializeFunc = serializeFunc;
        }

        public string Extension { get; }

        public string Serialize(object o)
        {
            return _serializeFunc(o);
        }

        public T Deserialize<T>(string x)
        {
            return JsonConvert.DeserializeObject<T>(x);
        }

        public static ISerialize GetSerializer()
        {
            return new Serializer(x =>
                JsonConvert.SerializeObject(x, Formatting.Indented, new JsonSerializerSettings {
                        NullValueHandling = NullValueHandling.Ignore,
                        PreserveReferencesHandling = PreserveReferencesHandling.None,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    }
                ), "json");
        }

        public static readonly JsonSerializerSettings SerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            Converters =
            {
                new AbstractConverter<Database, IDatabase>(),
                new AbstractConverter<Enhancement, IEnhancement>(),
                new AbstractConverter<Powerset, IPowerset>(),
                new AbstractConverter<Power, IPower>(),
                new AbstractConverter<Effect, IEffect>(),
            }
        };

        private class AbstractConverter<TReal, TAbstract> : JsonConverter where TReal : TAbstract
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(TAbstract);

            public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer jser) => jser.Deserialize<TReal>(reader);

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer jser) => jser.Serialize(writer, value);
        }
    }
}
