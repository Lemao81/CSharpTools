namespace DicomReader.Avalonia.Interfaces
{
    public interface IFileSystemService
    {
        bool FileExists(string filePath);
        string ReadFile(string filePath);
        void WriteFile(string filePath, string text);
    }
}
