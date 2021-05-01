using System.Collections.Generic;
using Dicom;
using Dicom.Serialization;
using DicomReader.Avalonia.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DicomReader.Avalonia.Services
{
    public class SerializedJsonDicomResponseProcessingStrategy : IDicomResponseProcessingStrategy<string>
    {
        public string ProcessResponse(List<DicomDataset> responseDatasets)
        {
            var datasetJsonKeywords = JsonConvert.SerializeObject(responseDatasets, Formatting.Indented, new JsonDicomConverter(true));
            var datasetJsonHex = JsonConvert.SerializeObject(responseDatasets, Formatting.Indented, new JsonDicomConverter());
            var jsonObject = new JObject
            {
                { "keywords", JToken.Parse(datasetJsonKeywords) },
                { "hex", JToken.Parse(datasetJsonHex) },
            };

            return jsonObject.ToString();
        }
    }
}
