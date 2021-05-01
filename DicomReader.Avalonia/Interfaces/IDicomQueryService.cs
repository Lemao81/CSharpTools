using System.Threading.Tasks;
using DicomReader.Avalonia.Models;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IDicomQueryService
    {
        Task<TResult> ExecuteDicomQuery<TResult>(DicomQueryInputs queryInputs, PacsConfiguration pacsConfiguration, int? take = null);
    }
}
