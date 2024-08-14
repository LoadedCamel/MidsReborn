using System;
using System.Text.RegularExpressions;

namespace Mids_Reborn.Core.Utils
{
    public static class DataClassifier
    {
        public enum DataType
        {
            Mbd,
            Mxd,
            UnkBase64,
            Unknown
        }

        public readonly struct ClassificationResult
        {
            public DataType Type { get; }
            private readonly string? _content;
            public string Content => _content ?? throw new InvalidOperationException("Content is not available for Unknown type.");
            public bool IsValid { get; }

            private ClassificationResult(DataType type, string? content, bool isValid)
            {
                Type = type;
                _content = content;
                IsValid = isValid;
            }

            public static ClassificationResult Create(DataType type, string? content)
            {
                return type == DataType.Unknown ? new ClassificationResult(type, null, false) : new ClassificationResult(type, content, true);
            }
        }

        private static readonly Regex MxdPattern = new(@"^\|MxDz;[0-9]+;[0-9]+;[0-9]+;HEX;\|", RegexOptions.Compiled);
        private static readonly Regex MbdPattern = new(@"^\|MBD;[0-9]+;[0-9]+;[0-9]+;BASE64;\|", RegexOptions.Compiled);
        private static readonly Regex UnkBase64Pattern = new(@"^[A-Za-z0-9+/]+={0,2}$", RegexOptions.Compiled);

        public static ClassificationResult ClassifyAndExtractData(string data)
        {
            if (MxdPattern.IsMatch(data)) // Check for mxd pattern match
            {
                return ClassificationResult.Create(DataType.Mxd, data);
            }

            if (MbdPattern.IsMatch(data))
            {
                return ClassificationResult.Create(DataType.Mbd, data); // Check for mbd pattern match
            }

            return UnkBase64Pattern.IsMatch(data) ? ClassificationResult.Create(DataType.UnkBase64, data) :
                ClassificationResult.Create(DataType.Unknown, null); // check for unknown base64 or unknown type
        }
    }

}
