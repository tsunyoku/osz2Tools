using System.Security.Cryptography;
using System.Text;

namespace osz2Tools.Utilities;

public static class Crypto
{
    public static byte[] ComputeHashBytes(string str)
    {
        return ComputeHashBytes(Encoding.ASCII.GetBytes(str));
    }

    public static byte[] ComputeHashBytes(byte[] buffer)
    {
        return MD5.HashData(buffer);
    }
}