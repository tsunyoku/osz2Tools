namespace osz2Tools.IO;

// ReSharper disable once InconsistentNaming
public class XXTea
{
    private const uint Max = 16;
    private const uint MaxBytes = Max * 4;
    private readonly uint[] _key;
    private readonly SimpleCryptor _simpleCryptor;

    private uint _n;

    public XXTea(uint[] key)
    {
        _key = key;
        _simpleCryptor = new SimpleCryptor(_key);
    }

    public void Decrypt(byte[] buffer, int start, int count)
    {
        EncryptDecrypt(buffer, start, count, false);
    }

    private unsafe void EncryptDecrypt(byte* bufferPtr, int bufferLength, bool encrypt)
    {
        EncryptDecryptXXTea(bufferPtr, bufferLength, encrypt);
    }

    private unsafe void EncryptDecrypt(byte[] buffer, int bufStart, int count, bool encrypt)
    {
        fixed (byte* bufferPtr = buffer)
        {
            EncryptDecrypt(bufferPtr + bufStart, count, encrypt);
        }
    }

    // ReSharper disable once InconsistentNaming
    private unsafe void EncryptDecryptXXTea(byte* bufferPtr, int bufferLength, bool encrypt)
    {
        var fullWordCount = unchecked((uint)bufferLength / MaxBytes);
        var leftOver = unchecked((uint)bufferLength) % MaxBytes;

        var uBufferPtr = (uint*)bufferPtr;

        _n = Max;

        var rounds = 6 + 52 / _n;

        if (encrypt)
            for (var wordCount = 0; wordCount < fullWordCount; wordCount++)
            {
                EncryptWords(uBufferPtr);
                uBufferPtr += Max;
            }
        else
            for (uint wordCount = 0; wordCount < fullWordCount; wordCount++)
            {
                uint y, z, sum;
                uint p, e;
                sum = rounds * Constants.TeaDelta;

                y = uBufferPtr[0];
                do
                {
                    e = (sum >> 2) & 3;
                    for (p = Max - 1; p > 0; p--)
                    {
                        z = uBufferPtr[p - 1];
                        y = uBufferPtr[p] -= (((z >> 5) ^ (y << 2)) + ((y >> 3) ^ (z << 4))) ^
                                             ((sum ^ y) + (_key[(p & 3) ^ e] ^ z));
                    }

                    z = uBufferPtr[Max - 1];
                    y = uBufferPtr[0] -= (((z >> 5) ^ (y << 2)) + ((y >> 3) ^ (z << 4))) ^
                                         ((sum ^ y) + (_key[(p & 3) ^ e] ^ z));
                } while ((sum -= Constants.TeaDelta) != 0);

                uBufferPtr += Max;
            }

        if (leftOver == 0)
            return;

        _n = leftOver / 4;
        if (_n > 1)
        {
            if (encrypt)
                EncryptWords(uBufferPtr);
            else
                DecryptWords(uBufferPtr);

            leftOver -= _n * 4;
            if (leftOver == 0)
                return;
        }

        var resultBuffer = bufferPtr;
        resultBuffer += bufferLength - leftOver;

        if (encrypt)
            _simpleCryptor.EncryptBytes(resultBuffer, unchecked((int)leftOver));
        else
            _simpleCryptor.DecryptBytes(resultBuffer, unchecked((int)leftOver));
    }

    // Modified XXTea algorithm
    private unsafe void EncryptWords(uint* v)
    {
        var rounds = 6 + 52 / _n;
        uint sum = 0;
        var z = v[_n - 1];

        do
        {
            sum += Constants.TeaDelta;
            var e = (sum >> 2) & 3;
            uint y;
            uint p;
            for (p = 0; p < _n - 1; p++)
            {
                y = v[p + 1];
                z = v[p] += (((z >> 5) ^ (y << 2)) + ((y >> 3) ^ (z << 4))) ^ ((sum ^ y) + (_key[(p & 3) ^ e] ^ z));
            }

            y = v[0];
            z = v[_n - 1] += (((z >> 5) ^ (y << 2)) + ((y >> 3) ^ (z << 4))) ^ ((sum ^ y) + (_key[(p & 3) ^ e] ^ z));
        } while (--rounds > 0);
    }

    private unsafe void DecryptWords(uint* v)
    {
        var rounds = 6 + 52 / _n;

        // ReSharper disable once IntVariableOverflowInUncheckedContext
        var sum = rounds * Constants.TeaDelta;
        var y = v[0];

        do
        {
            var e = (sum >> 2) & 3;
            uint p;
            uint z;
            for (p = _n - 1; p > 0; p--)
            {
                z = v[p - 1];
                y = v[p] -= (((z >> 5) ^ (y << 2)) + ((y >> 3) ^ (z << 4))) ^ ((sum ^ y) + (_key[(p & 3) ^ e] ^ z));
            }

            z = v[_n - 1];
            y = v[0] -= (((z >> 5) ^ (y << 2)) + ((y >> 3) ^ (z << 4))) ^ ((sum ^ y) + (_key[(p & 3) ^ e] ^ z));
        } while ((sum -= Constants.TeaDelta) != 0);
    }
}