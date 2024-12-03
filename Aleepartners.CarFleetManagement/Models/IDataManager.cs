using System.Collections.ObjectModel;

namespace FleetMgt.Models
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