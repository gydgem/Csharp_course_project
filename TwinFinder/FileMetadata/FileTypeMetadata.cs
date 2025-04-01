namespace TwinFinder.FileMetadata;

public enum FileTypeMetadata
{
    OtherFile,  // все файлы которые не являются ярлыками или ссылками
    LinkFile,    // Символические ссылки на файлы
    ShortcutFile, // Ярлыки (.lnk) на файлы или папки
}