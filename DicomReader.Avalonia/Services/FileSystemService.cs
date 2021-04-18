using System;
using System.Diagnostics;
using System.IO;
using DicomReader.Avalonia.Interfaces;

namespace DicomReader.Avalonia.Services
{
    public class FileSystemService : IFileSystemService
    {
        public bool FileExists(string filePath) => File.Exists(filePath);

        public string ReadFile(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.Message);

                return string.Empty;
            }
        }

        public void WriteFile(string filePath, string text)
        {
            try
            {
                File.WriteAllText(filePath, text);
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.Message);
            }
        }
    }
}
