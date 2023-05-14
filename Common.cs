using osz2Tools.IO;

namespace osz2Tools;

public static class Common
{
    public static void DecryptOsz2(string osz2Path, string targetPath)
    {
        var osz2Package = new Osz2Package(osz2Path);

        foreach (var (fileName, fileBytes) in osz2Package.Files)
            File.WriteAllBytes(Path.Combine(targetPath, fileName), fileBytes);
    }

    public static void DecryptOsz2(Stream osz2Stream, string targetPath)
    {
        var osz2Package = new Osz2Package(osz2Stream);

        foreach (var (fileName, fileBytes) in osz2Package.Files)
            File.WriteAllBytes(Path.Combine(targetPath, fileName), fileBytes);
    }
    
    public static void DecryptPatch(string oldFilePath, string patchPath, string targetPath)
    {
        var patcher = new PatchDecryptor();
        patcher.Patch(oldFilePath, oldFilePath, patchPath, 0);
        
        DecryptOsz2(oldFilePath, targetPath);
    }

    public static void DecryptPatch(Stream oldOsz2Content, Stream patchContent, string targetPath)
    {
        using var newData = new MemoryStream();
        
        var patcher = new PatchDecryptor();
        patcher.Patch(oldOsz2Content, newData, patchContent, 0);

        newData.Seek(0, SeekOrigin.Begin);
        
        DecryptOsz2(newData, targetPath);
    }
}