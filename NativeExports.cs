using System.Runtime.InteropServices;

namespace osz2Tools;

public static class NativeExports
{
    [UnmanagedCallersOnly(EntryPoint = "decrypt_osz2")]
    public static void DecryptOsz2(IntPtr osz2Ptr, IntPtr targetPtr)
    {
        var osz2Path = Marshal.PtrToStringAnsi(osz2Ptr)!;
        var targetPath = Marshal.PtrToStringAnsi(targetPtr)!;

        var osz2Package = new Osz2Package(osz2Path);

        foreach (var (fileName, fileBytes) in osz2Package.Files)
            File.WriteAllBytes(Path.Combine(targetPath, fileName), fileBytes);
    }
}