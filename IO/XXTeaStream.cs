namespace osz2Tools.IO;

// ReSharper disable once InconsistentNaming
public class XXTeaStream : Stream
{
    private readonly XXTea _xxtea;

    public readonly Stream InternalStream;

    public unsafe XXTeaStream(Stream stream, byte[] key)
    {
        InternalStream = stream;

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

        _xxtea = new XXTea(keyBuffer);
    }

    public override bool CanRead => InternalStream.CanRead;

    public override bool CanSeek => InternalStream.CanSeek;

    public override bool CanWrite => InternalStream.CanWrite;

    public override long Length => InternalStream.Length;

    public override long Position
    {
        get => InternalStream.Position;
        set => InternalStream.Position = value;
    }

    private void Decrypt(byte[] buffer, int start, int count)
    {
        _xxtea.Decrypt(buffer, start, count);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var bytesRead = InternalStream.Read(buffer, offset, count);
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