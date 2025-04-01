using TwinFinder.FileMetadata;

namespace TwinFinder.Dao;

public class FileMetadataCsvDao : IFileMetadataDao
{
    private readonly string _csvFilePath;
    public FileMetadataCsvDao(string csvFilePath)
    {
        _csvFilePath = csvFilePath;
        if (!File.Exists(_csvFilePath))
        {
            File.WriteAllText(_csvFilePath, "FullPath,CreationDate,FileHash\n");
        }
    }
    
    public void AddFile(FileMetadataInfo fileMetadata)
    {
        var line = fileMetadata.ToStringCsv();
        File.AppendAllLines(_csvFilePath, new[] { line });
    }
    
    public void AddFiles(IEnumerable<FileMetadataInfo> fileMetadatas)
    {
        var lines = fileMetadatas.Select(fm => fm.ToStringCsv());
        File.AppendAllLines(_csvFilePath, lines);
    }
    
    public List<FileMetadataInfo> GetFilesByNames(IEnumerable<string> fileNames)
    {
        var allFiles = File.ReadAllLines(_csvFilePath)
            .Skip(1) 
            .Select(FileMetadataInfo.FromCsv)
            .ToList();

        return allFiles
            .Where(fm => fileNames.Contains(fm.FullFilePath)) 
            .GroupBy(fm => fm.FullFilePath) 
            .Select(group => group.OrderBy(fm => fm.LastModified).First()) 
            .ToList();
    }
    
    public List<FileMetadataInfo> GetAllFiles()
    {
        return File.ReadAllLines(_csvFilePath)
            .Skip(1)  
            .Select(FileMetadataInfo.FromCsv)
            .ToList();
    }
}