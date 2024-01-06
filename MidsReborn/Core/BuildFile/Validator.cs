using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace Mids_Reborn.Core.BuildFile
{
    internal class Validator
    {
        private readonly JSchema _schema;

        internal struct BuildValidationResult
        {
            public bool Valid;
            public string Data;
            public string ErrorMessage;
        }

        public Validator()
        {
            var generator = new JSchemaGenerator
            {
                DefaultRequired = Required.AllowNull,
                SchemaIdGenerationHandling = SchemaIdGenerationHandling.TypeName
            };
            _schema = generator.Generate(typeof(CharacterBuildFile));
        }

        public BuildValidationResult Validate(string buildFile)
        {
            var data = File.ReadAllText(buildFile);
            try
            {
                var details = "Invalid JSON Payload.\r\n";
                var buildData = JObject.Parse(data);
                var valid = buildData.IsValid(_schema, out IList<string> errorMessages);

                for (var index = 0; index < errorMessages.Count; index++)
                {
                    var message = errorMessages[index];
                    if (index != errorMessages.Count) details += $"{message}\r\n";
                    else details += $"{message}";
                }

                return new BuildValidationResult { Valid = valid, Data = data, ErrorMessage = details };
            }
            catch (JSchemaException)
            {
                return new BuildValidationResult { Valid = false, Data = data, ErrorMessage = "Invalid JSON Format.\r\nPlease ensure you are using a valid MBD file." };
            }
        }
    }
}
