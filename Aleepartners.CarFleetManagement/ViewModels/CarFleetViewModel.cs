using Aleepartners.CarFleetManagement.Models;
using System.Collections.ObjectModel;

namespace Aleepartners.CarFleetManagement.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly DataManager _dataManager;
        public ObservableCollection<Car> _carStatuses;
        private ObservableCollection<FuelEntry> _fuelEntryCollection;
        public ObservableCollection<Car> CarStatusCollection
        {
            get => _carStatuses;
            set => SetProperty(ref _carStatuses, value);
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
            LoadData();
        }

        private void LoadData()
        {
            CarStatusCollection = new ObservableCollection<Car>(_dataManager.LoadCarStatus());
            FuelEntryCollection = new ObservableCollection<FuelEntry>(_dataManager.LoadFuelData());
            UpdateFuelConsumption();
        }

        private void UpdateFuelConsumption()
        {
            var fuelEntries = _dataManager.LoadFuelData();
            foreach (var car in CarStatusCollection)
            {
                var carFuelEntries = FuelEntryCollection.Where(entry => entry.NumberPlate == car.NumberPlate);

                car.Mileage = carFuelEntries.Sum(entry => entry.Mileage);
                car.TotalFuelConsumed = carFuelEntries.Sum(entry => entry.FuelInLitres);
                car.AverageFuelConsumption = car.Mileage > 0
                    ? car.TotalFuelConsumed / car.Mileage
                    : 0;
            }

            // Notify UI about updated CarStatuses collection
            RaisePropertyChanged(nameof(CarStatusCollection));
        }

        // Handle real-time updates when files change
        private void OnFileChanged()
        {
            LoadData();
        }
    }
}
