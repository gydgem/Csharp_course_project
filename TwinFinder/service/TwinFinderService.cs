using TwinFinder.Dao;
using TwinFinder.FileMetadata;
using TwinFinder.ScannersInFileSystem;

namespace TwinFinder.service;

public class TwinFinderService
{
    private IFileMetadataDao _fileMetadataDao;
    private string? _directoryPath = null;
    private List<List<FileMetadataInfo>> _duplicates = new();

    public string? DirectoryPath
    {
        get => _directoryPath;
        set
        {
            _directoryPath = value;
            _duplicates.Clear();
        }
    }

    public IReadOnlyList<List<FileMetadataInfo>> Duplicates => _duplicates;

    public TwinFinderService(IFileMetadataDao fileMetadataDao)
    {
        _fileMetadataDao = fileMetadataDao;
    }

    public TwinFinderService(string directoryPath, IFileMetadataDao fileMetadataDao)
    {
        _directoryPath = directoryPath;
        _fileMetadataDao = fileMetadataDao;
    }

    public void FindTwin()
    {
        FileScannerChecker.CheckFileDictionary(_directoryPath);

        var currentFilePaths = DirectoryScanner.GetAllFullFileNamesInDirectory(_directoryPath);

        var existingFiles = _fileMetadataDao.GetFilesByNames(currentFilePaths);
        var existingFilePaths = new HashSet<string>(existingFiles.Select(f => f.FullFilePath));

        var newFilePaths = currentFilePaths.Except(existingFilePaths).ToList();

        var updatedFiles = new List<FileMetadataInfo>();

        foreach (var existingFile in existingFiles)
        {
            var currentLastWriteTime = FileScanner.GetLastWriteTime(existingFile.FullFilePath);

            if (currentLastWriteTime > existingFile.LastModified)
            {
                var updatedFile = new FileMetadataInfo(existingFile.FullFilePath);
                updatedFiles.Add(updatedFile);
            }
        }

        if (newFilePaths.Any())
        {
            var newFileMetadatas = newFilePaths.Select(path => new FileMetadataInfo(path)).ToList();
            _fileMetadataDao.AddFiles(newFileMetadatas);
            existingFiles.AddRange(newFileMetadatas);
        }

        if (updatedFiles.Any())
        {
            _fileMetadataDao.AddFiles(updatedFiles);
            existingFiles = existingFiles
                .Where(f => !updatedFiles.Any(u => u.FullFilePath == f.FullFilePath))
                .Concat(updatedFiles)
                .ToList();
        }

        var fileHashMap = existingFiles
            .GroupBy(f => Convert.ToBase64String(f.HashFile))
            .Where(group => group.Count() > 1)
            .Select(group => group.ToList())
            .ToList();

        _duplicates = fileHashMap;
    }

    public void EraseFile(string filePath)
    {
        FileScannerChecker.CheckFilePath(filePath);
        File.Delete(filePath);
    }
}