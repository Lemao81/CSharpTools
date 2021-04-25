using DicomReader.Avalonia.Models;
using Newtonsoft.Json;

namespace DicomReader.Avalonia.Extensions
{
    public static class CommonExtensions
    {
        public static string AsJson(this object obj) => JsonConvert.SerializeObject(obj);

        public static string AsIndentedJson(this object obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);

        public static T? FromJson<T>(this string str) => JsonConvert.DeserializeObject<T>(str);
        
        public static void Emit(this LogEntry logEntry) => LogEntry.Emit(logEntry);
    }
}
