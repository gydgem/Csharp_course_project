using TwinFinder.FileMetadata;

namespace TwinFinder.Dao;

public interface IFileMetadataDao
{
    void AddFile(FileMetadataInfo fileMetadata);
    void AddFiles(IEnumerable<FileMetadataInfo> fileMetadatas);
    List<FileMetadataInfo> GetFilesByNames(IEnumerable<string> fileNames);
    List<FileMetadataInfo> GetAllFiles();
}