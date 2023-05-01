namespace osz2Tools.IO;

public class XTeaStream : Stream
{
    private readonly Stream _internalStream;
    private readonly XTea _xtea;

    public unsafe XTeaStream(Stream stream, byte[] key)
    {
        _internalStream = stream;

        var keyBuffer = new uint[4];
        fixed (byte* ptr = key)
        {
            fixed (uint* keyPtr = keyBuffer)
            {
                var tmp = (uint*)ptr;
                *keyPtr = *tmp;
                keyPtr[1] = tmp[1];
                keyPtr[2] = tmp[2];
                keyPtr[3] = tmp[3];
            }
        }

        _xtea = new XTea(keyBuffer);
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => 0;
    public override long Position { get; set; }

    private void Decrypt(byte[] buffer, int start, int count)
    {
        _xtea.Decrypt(buffer, start, count);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var bytesRead = _internalStream.Read(buffer, offset, count);
        Decrypt(buffer, offset, count);

        return bytesRead;
    }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}