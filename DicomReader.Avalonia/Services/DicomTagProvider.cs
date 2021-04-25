using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Dicom;
using DicomReader.Avalonia.Interfaces;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Services
{
    public class DicomTagProvider : IDicomTagProvider
    {
        private FieldInfo[] _knownDicomTagFields;

        public FieldInfo[] KnownDicomTagFields => _knownDicomTagFields;

        public Result<DicomTag> ProvideDicomTag(string keywordOrHexCode) =>
            IsHexCode(keywordOrHexCode)
                ? ParseDicomTagHexCode(keywordOrHexCode)
                : FindKnownDicomTag(keywordOrHexCode);

        public bool IsHexCode(string keywordOrHexCode) => keywordOrHexCode.Contains(':');

        public Result<DicomTag> ParseDicomTagHexCode(string hexCode)
        {
            var split = hexCode.Split(':');
            try
            {
                var group = ushort.Parse(split[0].Trim(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                var element = ushort.Parse(split[1].Trim(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                return new DicomTag(group, element);
            }
            catch (Exception exception)
            {
                return exception;
            }
        }

        public Result<DicomTag> FindKnownDicomTag(string tagName)
        {
            try
            {
                var foundDicomTagField = KnownDicomTagFields.FirstOrDefault(f => f.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));

                return foundDicomTagField != null
                    ? (DicomTag) foundDicomTagField.GetValue(null)
                    : Result<DicomTag>.Fail();
            }
            catch (Exception exception)
            {
                return exception;
            }
        }
    }
}
