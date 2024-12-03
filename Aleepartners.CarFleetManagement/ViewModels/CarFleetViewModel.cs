using FleetMgt.Models;
using System.Collections.ObjectModel;

namespace FleetMgt.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IDataManager _dataManager;
        private ObservableCollection<Car> _carStatusCollection;
        private ObservableCollection<FuelEntry> _fuelEntryCollection;

        public ObservableCollection<Car> CarStatusCollection
        {
            get => _carStatusCollection;
            set => SetProperty(ref _carStatusCollection, value);
        }

        public ObservableCollection<FuelEntry> FuelEntryCollection
        {
            get => _fuelEntryCollection;
            set => SetProperty(ref _fuelEntryCollection, value);
        }

        public MainViewModel()
        {
            _dataManager = new DataManager();
            _dataManager.FileChanged += OnFileChanged;
            CarStatusCollection = _dataManager.CarStatusCollection;
            FuelEntryCollection = new ObservableCollection<FuelEntry>();
            LoadData();
        }

        private void LoadData()
        {
            // Reload data from DataManager
            var carStatuses = _dataManager.LoadCarStatus();
            var fuelData = _dataManager.LoadFuelData();

            // Update CarStatusCollection
            CarStatusCollection.Clear();
            foreach (var car in carStatuses)
            {
                CarStatusCollection.Add(car);
            }

            // Update FuelEntryCollection
            FuelEntryCollection.Clear();
            foreach (var entry in fuelData)
            {
                var fuelStats = entry.Value;
                FuelEntryCollection.Add(new FuelEntry
                {
                    NumberPlate = entry.Key,
                    FuelInLitres = fuelStats.TotalFuel,
                    Mileage = fuelStats.TotalMileage
                });
            }
        }

        private void OnFileChanged()
        {
            RaisePropertyChanged(nameof(CarStatusCollection));
            // Ensure file changes update on the UI thread
            //App.Current.Dispatcher.Invoke(LoadData);
        }
    }
}
