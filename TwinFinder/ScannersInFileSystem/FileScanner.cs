using System.Security.Cryptography;
using TwinFinder.FileMetadata;

namespace TwinFinder.ScannersInFileSystem;

public static class FileScanner
{
    public static string GetFullFilePath(string fullFilePath)
    {
        FileScannerChecker.CheckFilePath(fullFilePath);
        return fullFilePath;
    }

    public static DateTime GetLastWriteTime(string fullFilePath)
    {
        try
        {
            FileScannerChecker.CheckFilePath(fullFilePath);
            return File.GetLastWriteTime(fullFilePath);
        }
        catch (Exception ex)
        {
            return DateTime.MinValue;
        }
    }

    public static FileTypeMetadata GetFileType(string fullFilePath)
    {
        try
        {
            FileScannerChecker.CheckFilePath(fullFilePath);

            string fileExtension = Path.GetExtension(fullFilePath).ToLowerInvariant();

            if (fileExtension == ".lnk")
            {
                return FileTypeMetadata.ShortcutFile;
            }

            if (IsSymbolicLink(fullFilePath))
            {
                return FileTypeMetadata.LinkFile;
            }
        }
        catch (Exception ex)
        {
            // ignored
        }

        return FileTypeMetadata.OtherFile;
    }

    public static byte[] GetHashFile(string fullFilePath)
    {
        try
        {
            FileScannerChecker.CheckFilePath(fullFilePath);

            using (SHA256 sha256Hash = SHA256.Create())
            {
                using (FileStream stream = File.OpenRead(fullFilePath))
                {
                    return sha256Hash.ComputeHash(stream);
                }
            }
        }
        catch (Exception ex)
        {
            return [];
        }
    }

    private static bool IsSymbolicLink(string fullFilePath)
    {
        try
        {
            var attributes = File.GetAttributes(fullFilePath);
            return (attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
        }
        catch (Exception)
        {
            return false;
        }
    }
}