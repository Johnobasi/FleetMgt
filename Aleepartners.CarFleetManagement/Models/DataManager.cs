using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.IO;

namespace Aleepartners.CarFleetManagement.Models
{
    public class DataManager : IDataManager
    {
        private readonly string _carStatusFilePath;
        private readonly string _fuelFilePath;
        public ObservableCollection<Car> CarStatusCollection { get; private set; }
        public DataManager()
        {
            // Initialize ObservableCollection
            CarStatusCollection = new ObservableCollection<Car>();

            // Load configuration
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            try
            {
                _carStatusFilePath = config["FilePaths:StatusFilePath"];
                _fuelFilePath = config["FilePaths:FuelFilePath"];

                if (string.IsNullOrEmpty(_carStatusFilePath) || string.IsNullOrEmpty(_fuelFilePath))
                {
                    throw new FileNotFoundException("One or both file paths are missing in configuration.");
                }

                // Initial load of data
                ReloadData();

                // Setup file watchers
                SetupFileWatcher(_carStatusFilePath);
                SetupFileWatcher(_fuelFilePath);
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to initialize file watchers.", ex);
                throw;
            }

        }
        public List<Car> LoadCarStatus()
        {
            var carStatuses = new List<Car>();

            try
            {
                foreach (var line in File.ReadLines(_carStatusFilePath))
                {
                    var fields = line.Split(',');
                    var car = new Car
                    {
                        Vin = fields[0].Trim(), // Assign VIN directly from fields[0]
                        NumberPlate = ExtractField(fields[1], "Number Plate"),
                        Colour = ExtractField(fields[1], "Colour"),
                        Make = ExtractField(fields[1], "Make"),
                        Model = ExtractField(fields[1], "Model"),
                        Mileage = TryParseMileage(ExtractField(fields[1], "Mileage"))
                    };
                    carStatuses.Add(car);
                }

                // Aggregate fuel data and update car statuses
                var fuelData = LoadFuelData();
                foreach (var car in carStatuses)
                {
                    if (fuelData.TryGetValue(car.NumberPlate, out var fuelStats))
                    {
                        car.TotalFuelConsumed = fuelStats.TotalFuel;
                        car.Mileage = fuelStats.TotalMileage;
                        car.AverageFuelConsumption = fuelStats.AverageFuelConsumption;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error loading car status data.", ex);
            }


            return carStatuses;
        }

        public Dictionary<string, FuelStatistics> LoadFuelData()
        {

            var fuelEntries = new Dictionary<string, FuelStatistics>();

            try
            {
                foreach (var line in File.ReadLines(_fuelFilePath))
                {
                    var fields = line.Split(',');
                    var numberPlate = fields[0];
                    var fuelInLitres = double.Parse(fields[1]);
                    var mileage = int.Parse(fields[2]);
                    var cost = decimal.Parse(fields[3]);
                    var dateAndTimeOfPurchase = DateTime.Parse(fields[4]);

                    if (!fuelEntries.ContainsKey(numberPlate))
                    {
                        fuelEntries[numberPlate] = new FuelStatistics();
                    }

                    var fuelStats = fuelEntries[numberPlate];
                    fuelStats.TotalFuel += fuelInLitres;
                    fuelStats.TotalMileage += mileage;
                    fuelStats.EntryCount++;
                    fuelStats.AverageFuelConsumption = fuelStats.EntryCount > 0 ? fuelStats.TotalFuel / fuelStats.EntryCount : 0;

                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Fuel file not found.", ex);
            }


            return fuelEntries;
        }

        private string ExtractField(string data, string fieldName)
        {
            var keyValues = data.Split('|');
            foreach (var kv in keyValues)
            {
                var parts = kv.Split('=');
                if (parts[0].Trim() == fieldName)
                    return parts[1].Trim();
            }
            return string.Empty;
        }

        private void SetupFileWatcher(string filePath)
        {
            var watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(filePath),
                Filter = Path.GetFileName(filePath),
                NotifyFilter = NotifyFilters.LastWrite
            };
            watcher.Changed += (s, e) => OnFileChanged();
            watcher.EnableRaisingEvents = true;
        }

        public event Action FileChanged;

        private void OnFileChanged()
        {
            try
            {
                ReloadData(); // Reload data and refresh the collection
                FileChanged?.Invoke(); // Notify subscribers that data has changed
            }
            catch (Exception ex)
            {
                Logger.LogError("Error reloading data on file change.", ex);
            }
        }

        private int TryParseMileage(string mileageString)
        {
            // Default to 0 if the mileage field is empty or invalid
            if (int.TryParse(mileageString, out int mileage))
            {
                return mileage;
            }
            return 0;
        }

        public void ReloadData()
        {
            // Clear the existing collection and reload it with fresh data
            CarStatusCollection.Clear();
            var carStatuses = LoadCarStatus();

            foreach (var car in carStatuses)
            {
                CarStatusCollection.Add(car);
            }
        }
    }
}
