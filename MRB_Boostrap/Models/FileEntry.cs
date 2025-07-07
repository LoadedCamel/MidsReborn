namespace MRB_Boostrap.Models;

public class FileEntry
{
    public string FileName { get; set; } = string.Empty;
    public string Directory { get; set; } = string.Empty;
    public byte[] Data { get; set; } = [];
}