using System;
using System.IO;
using DicomReader.WPF.Interfaces;
using DicomReader.WPF.Models;

namespace DicomReader.WPF.Services
{
    public class FileSystemService : IFileSystemService
    {
        public bool FileExists(string filePath) => File.Exists(filePath);

        public Result<string> ReadFile(string filePath)
        {
            try
            {
                return Result<string>.Success(File.ReadAllText(filePath));
            }
            catch (Exception exception)
            {
                return Result<string>.Exception(exception);
            }
        }

        public Result WriteFile(string filePath, string text)
        {
            try
            {
                File.WriteAllText(filePath, text);

                return Result.Success();
            }
            catch (Exception exception)
            {
                return Result.Exception(exception);
            }
        }
    }
}
