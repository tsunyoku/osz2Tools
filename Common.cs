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

    public static void DecryptPatch(string oldFilePath, string patchPath, string targetPath)
    {
        var patcher = new PatchDecryptor();
        patcher.Patch(oldFilePath, oldFilePath, patchPath, 0);
        
        DecryptOsz2(oldFilePath, targetPath);
    }
}