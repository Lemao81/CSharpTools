using System.Collections.Generic;
using System.Threading.Tasks;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IDicomQueryService
    {
        Task<List<DicomResultSet>> ExecuteDicomQuery(DicomQueryInputs queryInputs, PacsConfiguration pacsConfiguration);
    }
}
