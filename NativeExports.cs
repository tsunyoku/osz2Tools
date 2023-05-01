using System.Runtime.InteropServices;
using osz2Tools.IO;

namespace osz2Tools;

public static class NativeExports
{
    [UnmanagedCallersOnly(EntryPoint = "decrypt_osz2")]
    public static void DecryptOsz2(IntPtr osz2Ptr, IntPtr targetPtr)
    {
        var osz2Path = Marshal.PtrToStringAnsi(osz2Ptr)!;
        var targetPath = Marshal.PtrToStringAnsi(targetPtr)!;

        Common.DecryptOsz2(osz2Path, targetPath);
    }

    [UnmanagedCallersOnly(EntryPoint = "decrypt_patch")]
    public static void DecryptPatch(IntPtr patchPtr, IntPtr oldOsz2Ptr, IntPtr targetPtr)
    {
        var patchPath = Marshal.PtrToStringAnsi(patchPtr)!;
        var oldOsz2Path = Marshal.PtrToStringAnsi(oldOsz2Ptr)!;
        var targetPath = Marshal.PtrToStringAnsi(targetPtr)!;

        Common.DecryptPatch(oldOsz2Path, patchPath, targetPath);

    }
}