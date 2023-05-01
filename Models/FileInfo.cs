namespace osz2Tools.Models;

public class FileInfo
{
    public FileInfo(string name, int offset, int size, byte[] hash, DateTime created, DateTime modified)
    {
        Name = name;
        Offset = offset;
        Size = size;
        Hash = hash;
        Created = created;
        Modified = modified;
    }

    public string Name { get; }
    public int Offset { get; }
    public int Size { get; }
    public byte[] Hash { get; }
    public DateTime Created { get; }
    public DateTime Modified { get; }
}