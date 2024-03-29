﻿using System;
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
        private FieldInfo[]? _knownDicomTagFields;

        public FieldInfo[] KnownDicomTagFields => _knownDicomTagFields ??= typeof(DicomTag).GetFields(BindingFlags.Public | BindingFlags.Static);

        public Result<DicomTag> ProvideDicomTag(string keywordOrHexCode) =>
            IsHexCode(keywordOrHexCode)
                ? ParseDicomTagHexCode(keywordOrHexCode)
                : FindKnownDicomTag(keywordOrHexCode);

        public static bool IsHexCode(string keywordOrHexCode) => keywordOrHexCode.Contains(':');

        public static Result<DicomTag> ParseDicomTagHexCode(string hexCode)
        {
            var split = hexCode.Split(':');
            if (split.Length != 2) return Result<DicomTag>.Fail();

            try
            {
                return DicomTag.Parse($"({split[0]},{split[1]})");
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
                var foundDicomTag = foundDicomTagField != null ? (DicomTag?) foundDicomTagField.GetValue(null) : null;

                return foundDicomTag ?? Result<DicomTag>.Fail();
            }
            catch (Exception exception)
            {
                return exception;
            }
        }
    }
}
