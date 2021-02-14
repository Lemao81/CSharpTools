using DicomReader.WPF.Models;

namespace DicomReader.WPF.Interfaces
{
    public interface IFileSystemService
    {
        bool FileExists(string filePath);
        Result<string> ReadFile(string filePath);
        Result WriteFile(string filePath, string text);
    }
}
