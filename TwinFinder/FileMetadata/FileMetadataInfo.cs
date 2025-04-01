using System.Text;
using TwinFinder.CsvParsers;
using TwinFinder.ScannersInFileSystem;

namespace TwinFinder.FileMetadata;

public struct FileMetadataInfo
{
    public string FullFilePath { get; }
    public DateTime LastModified { get; }
    public FileTypeMetadata FileTypeMetadata { get; }
    public byte[] HashFile { get; }

    public FileMetadataInfo(string fullFilePath, DateTime lastModified, FileTypeMetadata fileTypeMetadata,
        byte[] hashFile)
    {
        FullFilePath = fullFilePath;
        LastModified = lastModified;
        FileTypeMetadata = fileTypeMetadata;
        HashFile = hashFile;
    }

    public FileMetadataInfo(string fullFilePath)
    {
        FullFilePath = FileScanner.GetFullFilePath(fullFilePath);
        LastModified = FileScanner.GetLastWriteTime(fullFilePath);
        FileTypeMetadata = FileScanner.GetFileType(fullFilePath);
        HashFile = FileScanner.GetHashFile(fullFilePath);
    }

    public string ToStringCsv()
    {
        string hashString = Convert.ToBase64String(HashFile);

        // Escape and quote values to handle commas
        string escapedFullFilePath = CsvParser.EscapeCsvValue(FullFilePath);
        string escapedFileTypeMetadata = CsvParser.EscapeCsvValue(FileTypeMetadata.ToString());

        return $"{escapedFullFilePath},{LastModified:O},{escapedFileTypeMetadata},{hashString}";
    }

    public static FileMetadataInfo FromCsv(string csvLine)
    {
        var parts = CsvParser.ParseCsvLine(csvLine);

        if (parts.Length != 4)
            throw new FormatException("Invalid CSV format for FileMetadataInfo.");

        string fullFilePath = parts[0];
        DateTime lastModified = DateTime.Parse(parts[1], null, System.Globalization.DateTimeStyles.RoundtripKind);
        FileTypeMetadata fileTypeMetadata = Enum.Parse<FileTypeMetadata>(parts[2]);
        byte[] hashFile = Convert.FromBase64String(parts[3]);

        return new FileMetadataInfo(fullFilePath, lastModified, fileTypeMetadata, hashFile);
    }
}