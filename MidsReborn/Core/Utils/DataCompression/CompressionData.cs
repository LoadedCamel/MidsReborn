namespace Mids_Reborn.Core.Utils.DataCompression
{
    public class CompressionData
    {
        public static CompressionData CreateInstance()
        {
            return new CompressionData();
        }

        public int UncompressedLength { get; set; }
        public int CompressedLength { get; set; }
        public int EncodedLength { get; set; }
        public string EncodedString { get; set; }
        public string EncodingType { get; set; }
    }
}
