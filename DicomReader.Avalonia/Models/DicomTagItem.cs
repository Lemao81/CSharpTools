using Common.Extensions;

namespace DicomReader.Avalonia.Models
{
    public class DicomTagItem
    {
        public DicomTagItem(string input)
        {
            if (input.Contains(":"))
            {
                HexCode = input;
                Name = string.Empty;
            }
            else
            {
                Name = input;
                HexCode = string.Empty;
            }
        }

        public string Name { get; set; }
        public string HexCode { get; set; }
        public string Label => Name.IsNullOrEmpty() ? HexCode : Name;
    }
}
