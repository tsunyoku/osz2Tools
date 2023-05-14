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

    [UnmanagedCallersOnly(EntryPoint = "decrypt_osz2_bytes")]
    public static unsafe void DecryptOsz2Array(byte* osz2BytesPtr, int osz2BytesLength, IntPtr targetPtr)
    {
        var osz2Bytes = new byte[osz2BytesLength];
        Marshal.Copy((IntPtr)osz2BytesPtr, osz2Bytes, 0, osz2BytesLength);
        
        var targetPath = Marshal.PtrToStringAnsi(targetPtr)!;

        using var ms = new MemoryStream(osz2Bytes);
        Common.DecryptOsz2(ms, targetPath);
    }

    [UnmanagedCallersOnly(EntryPoint = "decrypt_patch_bytes")]
    public static unsafe void DecryptPatchArray(byte* patchBytesPtr, int patchBytesLength, byte* osz2BytesPtr,
        int osz2BytesLength, IntPtr targetPtr)
    {
        var patchBytes = new byte[patchBytesLength];
        Marshal.Copy((IntPtr)patchBytesPtr, patchBytes, 0, patchBytesLength);

        var osz2Bytes = new byte[osz2BytesLength];
        Marshal.Copy((IntPtr)osz2BytesPtr, osz2Bytes, 0, osz2BytesLength);
        
        var targetPath = Marshal.PtrToStringAnsi(targetPtr)!;
        
        using var patchStream = new MemoryStream(patchBytes);
        using var osz2Stream = new MemoryStream(osz2Bytes);
        
        Common.DecryptPatch(osz2Stream, patchStream, targetPath);
    }
}