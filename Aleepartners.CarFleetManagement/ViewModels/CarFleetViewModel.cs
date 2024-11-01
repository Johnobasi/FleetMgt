using Aleepartners.CarFleetManagement.Models;
using System.Collections.ObjectModel;

namespace Aleepartners.CarFleetManagement.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly DataManager _dataManager;
        public ObservableCollection<Car> _carStatusCollection;
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
            LoadData();
        }

        private void LoadData()
        {
            // Load data from DataManager
            CarStatusCollection = new ObservableCollection<Car>(_dataManager.LoadCarStatus());
            
            // Load fuel data from DataManager
            var fuelData = _dataManager.LoadFuelData();
            FuelEntryCollection = new ObservableCollection<FuelEntry>();

            // Convert FuelStatistics into FuelEntry objects for binding
            foreach (var entry in fuelData)
            {
                var numberPlate = entry.Key;
                var fuelStats = entry.Value;

                // Create a new FuelEntry for each number plate
                FuelEntryCollection.Add(new FuelEntry
                {
                    NumberPlate = numberPlate,
                    FuelInLitres = fuelStats.TotalFuel,
                    Mileage = fuelStats.TotalMileage
                });
            }

            // Calculate and update fuel consumption and mileage
            UpdateFuelConsumption();

            // Notify UI about updated CarStatusCollection
            RaisePropertyChanged(nameof(CarStatusCollection));
            RaisePropertyChanged(nameof(FuelEntryCollection));
        }

        private void UpdateFuelConsumption()
        {
            // Clear previous values to avoid accumulation
            foreach (var car in CarStatusCollection)
            {
                car.TotalFuelConsumed = 0;
                car.Mileage = 0;
                car.AverageFuelConsumption = 0;
            }

            foreach (var car in CarStatusCollection)
            {
                var carFuelEntries = FuelEntryCollection.Where(entry => entry.NumberPlate == car.NumberPlate);

                // If there are fuel entries, proceed with calculations
                if (carFuelEntries.Any())
                {
                    car.Mileage = carFuelEntries.Sum(entry => entry.Mileage);
                    car.TotalFuelConsumed = carFuelEntries.Sum(entry => entry.FuelInLitres);

                    car.AverageFuelConsumption = car.Mileage > 0
                        ? (car.TotalFuelConsumed / car.Mileage) * 100 // Assuming L/100 km format
                        : 0;
                }
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
