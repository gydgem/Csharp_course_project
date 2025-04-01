using System.Security;

namespace TwinFinder.ScannersInFileSystem;

public static class FileScannerChecker
{
    public static void CheckFilePath(string? fullFilePath)
    {
        if (fullFilePath is null)
        {
            throw new ArgumentNullException(nameof(fullFilePath));
        }
        
        try
        {
            if (!File.Exists(fullFilePath))
            {
                throw new FileNotFoundException($"Error: File {fullFilePath} not found");
            }
        }
        catch (UnauthorizedAccessException)
        {
            throw new UnauthorizedAccessException($"Error: Access to the file {fullFilePath} is denied.");
        }
        catch (PathTooLongException)
        {
            throw new PathTooLongException($"Error: The specified path is too long: {fullFilePath}");
        }
        catch (DirectoryNotFoundException)
        {
            throw new DirectoryNotFoundException($"Error: The directory for the file {fullFilePath} does not exist.");
        }
        catch (NotSupportedException)
        {
            throw new NotSupportedException(
                $"Error: The specified path has invalid characters or is not supported: {fullFilePath}");
        }
        catch (SecurityException)
        {
            throw new SecurityException($"Error: Security error when trying to access the file {fullFilePath}.");
        }
        catch (FileLoadException)
        {
            throw new FileLoadException($"Error: The file {fullFilePath} could not be loaded.");
        }
        catch (IOException ex)
        {
            throw new IOException($"Error: An I/O error occurred while accessing the file: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error: {ex.Message}");
        }
    }

    public static void CheckFileDictionary(string? directoryPath)
    {
        if (directoryPath is null)
        {
            throw new ArgumentNullException(nameof(directoryPath));
        }

        try
        {
            // Check if the directory exists
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Error: The directory {directoryPath} does not exist.");
            }
        
            // Attempt to get files to ensure directory is accessible
            var files = Directory.GetFiles(directoryPath);
        }
        catch (UnauthorizedAccessException)
        {
            throw new UnauthorizedAccessException($"Error: Access to the directory {directoryPath} is denied.");
        }
        catch (PathTooLongException)
        {
            throw new PathTooLongException($"Error: The specified path is too long: {directoryPath}");
        }
        catch (DirectoryNotFoundException)
        {
            throw new DirectoryNotFoundException($"Error: The directory {directoryPath} does not exist.");
        }
        catch (NotSupportedException)
        {
            throw new NotSupportedException(
                $"Error: The specified path has invalid characters or is not supported: {directoryPath}");
        }
        catch (SecurityException)
        {
            throw new SecurityException($"Error: Security error when trying to access the directory {directoryPath}.");
        }
        catch (IOException ex)
        {
            throw new IOException($"Error: An I/O error occurred while accessing the directory: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error: {ex.Message}");
        }
    }

}