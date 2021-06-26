using System.Threading.Tasks;
using DicomReader.Avalonia.Models;
using DicomReader.Avalonia.ViewModels;

namespace DicomReader.Avalonia.Interfaces
{
    public interface IStartQueryHandler
    {
        Task StartQueryAsync(MainWindowViewModel mainWindowViewModel, DicomQueryInputs dicomQueryInputs);
    }
}
