using osz2Tools.IO;
using osz2Tools.Utilities;
using FileInfo = osz2Tools.Models.FileInfo;

namespace osz2Tools;

public class Osz2Package
{
    private readonly Dictionary<string, FileInfo> _info = new();
    private readonly Dictionary<int, string> _metadata = new();

    public readonly Dictionary<string, byte[]> Files = new();
    private byte[] _bodyHash = null!;
    private byte[] _encryptKey = null!;
    private byte[] _infoHash = null!;
    private byte[] _metaHash = null!;

    public Osz2Package(string path)
    {
        Read(File.OpenRead(path));
    }

    private void Read(Stream file)
    {
        var reader = new BinaryReader(file);
        var identifier = reader.ReadBytes(3); // magic number, needed to check for validity

        if (identifier.Length < 3 || identifier[0] != 0xEC || identifier[1] != 0x48 || identifier[2] != 0x4F)
            throw new Exception("Invalid osz2 file!");

        reader.ReadByte(); // version, irrelevant
        reader.ReadBytes(16); // iv, also irrelevant

        _metaHash = reader.ReadBytes(16);
        _infoHash = reader.ReadBytes(16);
        _bodyHash = reader.ReadBytes(16);

        using (var ms = new MemoryStream())
        {
            using (var writer = new BinaryWriter(ms))
            {
                var count = reader.ReadInt32();
                writer.Write(count);

                foreach (var i in Enumerable.Range(0, count))
                {
                    var type = reader.ReadInt16();
                    var val = reader.ReadString();

                    _metadata.Add(type, val);
                    writer.Write(type);
                    writer.Write(val);
                }

                var hash = CalculateHash(ms.ToArray(), count * 3, 0xa7);
                if (!hash.SequenceEqual(_metaHash))
                    throw new Exception("Invalid metadata");
            }
        }

        var mapCount = reader.ReadInt32();

        foreach (var val in Enumerable.Range(0, mapCount))
        {
            var fileName = reader.ReadString(); // unused?
            var beatmapId = reader.ReadInt32(); // unused?
        }

        var seed = _metadata[2] + "yhxyfjo5" + _metadata[9];
        _encryptKey = Crypto.ComputeHashBytes(seed);

        ReadFiles(reader);
    }

    private static IEnumerable<byte> CalculateHash(byte[] buffer, int position, byte swap)
    {
        buffer[position] ^= swap;

        var hash = Crypto.ComputeHashBytes(buffer);

        buffer[position] ^= swap;

        foreach (var val in Enumerable.Range(0, 8))
            (hash[val], hash[val + 8]) = (hash[val + 8], hash[val]);

        hash[5] ^= 0x2d;
        return hash;
    }

    private void ReadFiles(BinaryReader reader)
    {
        using (var decryptor = new XTeaStream(reader.BaseStream, _encryptKey))
        {
            var plain = new byte[64];
            var read = decryptor.Read(plain, 0, 64);
        }

        var length = reader.ReadInt32();
        for (var i = 0; i < 16; i += 2)
            length -= _infoHash[i] | (_infoHash[i + 1] << 17);

        var fileData = reader.ReadBytes(length);
        var fileOffset = (int)reader.BaseStream.Position;

        using (var ms = new MemoryStream(fileData))
        {
            using (var xxTeaStream = new XXTeaStream(ms, _encryptKey))
            {
                using (var fileReader = new BinaryReader(xxTeaStream))
                {
                    var count = fileReader.ReadInt32();
                    var hash = CalculateHash(fileData, count * 4, 0xd1);
                    if (!hash.SequenceEqual(_infoHash))
                        throw new Exception("Invalid file info");

                    var offset = fileReader.ReadInt32();
                    foreach (var val in Enumerable.Range(0, count))
                    {
                        var fileName = fileReader.ReadString();
                        var fileHash = fileReader.ReadBytes(16);
                        var dateCreated = DateTime.FromBinary(fileReader.ReadInt64());
                        var dateModified = DateTime.FromBinary(fileReader.ReadInt64());

                        var next = 0;
                        if (val + 1 < count) next = fileReader.ReadInt32();
                        else next = (int)reader.BaseStream.Length - fileOffset;

                        var fileLength = next - offset;
                        var fileInfo = new FileInfo(fileName, offset, fileLength, fileHash, dateCreated, dateModified);

                        _info.Add(fileName, fileInfo);

                        offset = next;
                    }
                }

                foreach (var (key, value) in _info)
                {
                    using var osz2Stream = new Osz2Stream(reader.BaseStream, fileOffset + value.Offset, _encryptKey);
                    using var osz2Reader = new BinaryReader(osz2Stream);

                    Files.Add(key, osz2Reader.ReadBytes(value.Size - 4));
                }
            }
        }
    }
}