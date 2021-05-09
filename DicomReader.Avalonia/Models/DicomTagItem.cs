using System;
using Common.Extensions;

namespace DicomReader.Avalonia.Models
{
    public class DicomTagItem : IEquatable<DicomTagItem>
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

        public string Name { get; }
        public string HexCode { get; }
        public string Content => Name.IsNullOrEmpty() ? HexCode : Name;

        public bool Equals(DicomTagItem? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            return Name == other.Name && HexCode == other.HexCode;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj.GetType() == GetType() && Equals((DicomTagItem) obj);
        }

        public override int GetHashCode() => HashCode.Combine(Name, HexCode);
    }
}
