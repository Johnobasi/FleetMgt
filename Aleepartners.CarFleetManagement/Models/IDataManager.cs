using System.Collections.ObjectModel;

namespace Aleepartners.CarFleetManagement.Models
{
    public interface IDataManager
    {
        ObservableCollection<Car> CarStatusCollection { get; }

        event Action FileChanged;

        List<Car> LoadCarStatus();
        Dictionary<string, FuelStatistics> LoadFuelData();
        void ReloadData();
    }
}