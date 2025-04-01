using TwinFinder.FileMetadata;
using TwinFinder.ScannersInFileSystem;

var allFileNamesInDirectory = DirectoryScanner.GetAllFullFileNamesInDirectory("D:\\TestCurse");
foreach (var v in allFileNamesInDirectory)
{
    Console.WriteLine("*****************************");
    FileMetadataInfo temp = new(v);
    Console.WriteLine(temp.FullFilePath);
    Console.WriteLine(temp.FileTypeMetadata);
    Console.WriteLine(temp.LastModified);
    foreach (var valueHash in temp.HashFile)
    {
        Console.Write(valueHash);
    }

    Console.WriteLine();
    Console.WriteLine("*****************************");
}