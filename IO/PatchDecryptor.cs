using ICSharpCode.SharpZipLib.BZip2;
using Ionic.Zlib;

namespace osz2Tools.IO;

public class PatchDecryptor
{
    private static int offtin(byte[] buf, int startOffset)
    {
        var num = buf[7 + startOffset] & 127;
        num *= 256;
        num += buf[6 + startOffset];
        num *= 256;
        num += buf[5 + startOffset];
        num *= 256;
        num += buf[4 + startOffset];
        num *= 256;
        num += buf[3 + startOffset];
        num *= 256;
        num += buf[2 + startOffset];
        num *= 256;
        num += buf[1 + startOffset];
        num *= 256;
        num += buf[startOffset];
        if ((buf[7 + startOffset] & 128) > 0) num = -num;
        return num;
    }

    public void Patch(Stream oldFile, Stream newFile, Stream patchFile, int compression)
    {
        long num;
        long num2;
        long num3;
        
        var array = new byte[32];
        if (patchFile.Read(array, 0, 32) < 32) throw new Exception("invalid patch file (too small)");
        if (!buffcmp(array, "BSDIFF40", 0, 8)) throw new Exception("invalid patch file (no magic)");
        num = offtin(array, 8);
        num2 = offtin(array, 16);
        num3 = offtin(array, 24);
        if (num < 0L || num2 < 0L || num3 < 0L) throw new Exception("invalid patch file (sizes are corrupt)");

        patchFile.Position = 0;

        var size = num3 * 3L;
        byte[] array2;
        byte[] array3;
        byte[] array4;

        patchFile.Seek(32L, SeekOrigin.Begin);
        array2 = new byte[num];
        patchFile.Read(array2, 0, array2.Length);
        array3 = new byte[num2];
        patchFile.Read(array3, 0, array3.Length);
        array4 = new byte[patchFile.Length - patchFile.Position];
        patchFile.Read(array4, 0, array4.Length);

        Stream stream;
        Stream stream2;
        Stream stream3;
        if (compression == 1)
        {
            stream = new BZip2InputStream(new MemoryStream(array2));
            stream2 = new BZip2InputStream(new MemoryStream(array3));
            stream3 = new BZip2InputStream(new MemoryStream(array4));
        }
        else
        {
            stream = new GZipStream(new MemoryStream(array2), (CompressionMode)1, (CompressionLevel)9);
            stream2 = new GZipStream(new MemoryStream(array3), (CompressionMode)1, (CompressionLevel)9);
            stream3 = new GZipStream(new MemoryStream(array4), (CompressionMode)1, (CompressionLevel)9);
        }

        var num4 = 1048573;
        byte[] array5;

        array5 = new byte[(int)oldFile.Length];
        for (var i = 0; i < array5.Length; i += num4)
            oldFile.Read(array5, i, Math.Min(num4, array5.Length - i));

        var array6 = new byte[num3];
        long num5 = array5.Length;
        var num6 = 0;
        var num7 = 0;
        var array7 = new int[3];
        var array8 = new byte[8];
        while (num6 < num3)
        {
            int num8;
            for (var j = 0; j <= 2; j++)
            {
                num8 = stream.Read(array8, 0, 8);
                array7[j] = offtin(array8, 0);
            }

            num8 = 0;
            for (var k = num6; k < num6 + array7[0]; k += 65536)
                num8 += stream2.Read(array6, k, Math.Min(65536, num6 + array7[0] - k));
            for (var l = 0; l < array7[0]; l++)
                if (num7 + l >= 0 && num7 + l < num5)
                {
                    var array9 = array6;
                    var num9 = num6 + l;
                    array9[num9] += array5[num7 + l];
                }

            num6 += array7[0];
            num7 += array7[0];
            num8 = stream3.Read(array6, num6, array7[1]);
            num6 += array7[1];
            num7 += array7[2];
        }

        stream.Close();
        stream2.Close();
        stream3.Close();

        for (var m = 0; m < array6.Length; m += num4)
            newFile.Write(array6, m, Math.Min(num4, array6.Length - m));
    }
    
    // Token: 0x0600343D RID: 13373 RVA: 0x00151C8C File Offset: 0x0014FE8C
    public void Patch(string oldFile, string newFile, string patchfile, int compression)
    {
        long num;
        long num2;
        long num3;
        using (var fileStream = File.OpenRead(patchfile))
        {
            var array = new byte[32];
            if (fileStream.Read(array, 0, 32) < 32) throw new Exception("invalid patch file (too small)");
            if (!buffcmp(array, "BSDIFF40", 0, 8)) throw new Exception("invalid patch file (no magic)");
            num = offtin(array, 8);
            num2 = offtin(array, 16);
            num3 = offtin(array, 24);
            if (num < 0L || num2 < 0L || num3 < 0L) throw new Exception("invalid patch file (sizes are corrupt)");
        }

        var size = num3 * 3L;
        byte[] array2;
        byte[] array3;
        byte[] array4;
        using (var fileStream2 = File.OpenRead(patchfile))
        {
            fileStream2.Seek(32L, SeekOrigin.Begin);
            array2 = new byte[num];
            fileStream2.Read(array2, 0, array2.Length);
            array3 = new byte[num2];
            fileStream2.Read(array3, 0, array3.Length);
            array4 = new byte[fileStream2.Length - fileStream2.Position];
            fileStream2.Read(array4, 0, array4.Length);
        }

        Stream stream;
        Stream stream2;
        Stream stream3;
        if (compression == 1)
        {
            stream = new BZip2InputStream(new MemoryStream(array2));
            stream2 = new BZip2InputStream(new MemoryStream(array3));
            stream3 = new BZip2InputStream(new MemoryStream(array4));
        }
        else
        {
            stream = new GZipStream(new MemoryStream(array2), (CompressionMode)1, (CompressionLevel)9);
            stream2 = new GZipStream(new MemoryStream(array3), (CompressionMode)1, (CompressionLevel)9);
            stream3 = new GZipStream(new MemoryStream(array4), (CompressionMode)1, (CompressionLevel)9);
        }

        var num4 = 1048573;
        var fileStream3 = new FileStream(oldFile, FileMode.Open, FileAccess.Read, FileShare.Read);
        byte[] array5;
        try
        {
            array5 = new byte[(int)fileStream3.Length];
            for (var i = 0; i < array5.Length; i += num4)
                fileStream3.Read(array5, i, Math.Min(num4, array5.Length - i));
        }
        finally
        {
            ((IDisposable)fileStream3).Dispose();
        }

        var array6 = new byte[num3];
        long num5 = array5.Length;
        var num6 = 0;
        var num7 = 0;
        var array7 = new int[3];
        var array8 = new byte[8];
        while (num6 < num3)
        {
            int num8;
            for (var j = 0; j <= 2; j++)
            {
                num8 = stream.Read(array8, 0, 8);
                array7[j] = offtin(array8, 0);
            }

            num8 = 0;
            for (var k = num6; k < num6 + array7[0]; k += 65536)
                num8 += stream2.Read(array6, k, Math.Min(65536, num6 + array7[0] - k));
            for (var l = 0; l < array7[0]; l++)
                if (num7 + l >= 0 && num7 + l < num5)
                {
                    var array9 = array6;
                    var num9 = num6 + l;
                    array9[num9] += array5[num7 + l];
                }

            num6 += array7[0];
            num7 += array7[0];
            num8 = stream3.Read(array6, num6, array7[1]);
            num6 += array7[1];
            num7 += array7[2];
        }

        stream.Close();
        stream2.Close();
        stream3.Close();
        using (var fileStream4 = File.Create(newFile))
        {
            for (var m = 0; m < array6.Length; m += num4)
                fileStream4.Write(array6, m, Math.Min(num4, array6.Length - m));
        }
    }

    private static bool buffcmp(byte[] buf, string s, int start, int count)
    {
        for (var i = start; i < start + count; i++)
            if ((char)buf[i] != s[i])
                return false;

        return true;
    }
}