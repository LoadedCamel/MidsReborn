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

        public static ClassificationResult ClassifyAndExtractData(string data)
        {
            // Check for .mxd match
            if (MxdPattern.IsMatch(data))
            {
                return ClassificationResult.Create(DataType.Mxd, data);
            }
            // Check for .mbd match

            return MbdPattern.IsMatch(data) ? ClassificationResult.Create(DataType.Mbd, data) :
                // If neither, return unknown
                ClassificationResult.Create(DataType.Unknown, null);
        }
    }

}
