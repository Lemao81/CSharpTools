using Dicom.Network;

namespace DicomReader.Avalonia.Services
{
    public class PagedDicomResponseCollector : DicomResponseCollectorBase
    {
        private readonly int _pageSize;
        private int _skipCount;
        private readonly int _skipLimit;

        public PagedDicomResponseCollector(int pageSize, int page)
        {
            _pageSize = pageSize;
            _skipLimit = pageSize * page - pageSize;
        }

        public override bool CollectResponse(DicomResponse response)
        {
            if (_skipCount < _skipLimit)
            {
                _skipCount++;

                return true;
            }

            if (ResponseDatasets.Count >= _pageSize) return false;

            ResponseDatasets.Add(response.Dataset);

            return true;
        }
    }
}
