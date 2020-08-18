using System;
using Newtonsoft.Json;

namespace Hero_Designer.My
{
    internal class MyApplication
    {
        public static ISerialize GetSerializer()
        {
            return new Serializer(x =>
                JsonConvert.SerializeObject(x,
                    Formatting.Indented
                    , new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        PreserveReferencesHandling = PreserveReferencesHandling.None,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    }
                ), "json");
        }

        private class Serializer : ISerialize
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
        }
    }
}