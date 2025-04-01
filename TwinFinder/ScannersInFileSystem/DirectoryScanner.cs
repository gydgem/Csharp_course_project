namespace TwinFinder.ScannersInFileSystem;

public static class DirectoryScanner
{
    /// <summary>
    /// Получает полный путь ко всем файлам в заданной директории и вложенных директориях.
    /// </summary>
    /// <param name="directoryPath">Путь к директории для сканирования.</param>
    /// <returns>Список полных путей к файлам.</returns>
    /// <exception cref="DirectoryNotFoundException">Если директория не найдена.</exception>
    /// <exception cref="UnauthorizedAccessException">Если доступ запрещён.</exception>
    /// <exception cref="IOException">Если произошла ошибка ввода-вывода.</exception>
    /// <exception cref="PathTooLongException">Если путь слишком длинный.</exception>
    /// <exception cref="FileNotFoundException">Если файл не найден.</exception>
    public static List<string> GetAllFullFileNamesInDirectory(string directoryPath)
    {
        var fileNames = new List<string>();

        foreach (string filePath in Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories))
        {
            try
            {
                fileNames.Add(filePath);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return fileNames;
    }
}