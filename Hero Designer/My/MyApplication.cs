using System;
using Newtonsoft.Json;

namespace Hero_Designer.My
{
    internal class MyApplication
    {
        private class Serializer : ISerialize
        {
            readonly Func<object, string> _serializeFunc;
            public string Extension { get; }
            public Serializer(Func<object, string> serializeFunc, string extension)
            {
                Extension = extension;
                _serializeFunc = serializeFunc;
            }
            public string Serialize(object o) => _serializeFunc(o);
            public T Deserialize<T>(string x) => JsonConvert.DeserializeObject<T>(x);
        }
        public static ISerialize GetSerializer()
        {
            return new Serializer(x =>
                JsonConvert.SerializeObject(x,
                    Formatting.Indented
                    , settings: new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        PreserveReferencesHandling = PreserveReferencesHandling.None,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore

                    }
                    ), "json");
        }

    }
}
