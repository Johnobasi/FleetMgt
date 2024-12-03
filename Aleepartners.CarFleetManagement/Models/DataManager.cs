using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace FleetMgt.Models
{
    public class DataManager : IDataManager
    {
        private readonly string _carStatusFilePath;
        private readonly string _fuelFilePath;

        private readonly List<FileSystemWatcher> _fileWatchers = new();

        public ObservableCollection<Car> CarStatusCollection { get; private set; }

        public DataManager()
        {
            CarStatusCollection = new ObservableCollection<Car>();

            // Load configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _carStatusFilePath = config["FilePaths:StatusFilePath"];
            _fuelFilePath = config["FilePaths:FuelFilePath"];

            if (string.IsNullOrEmpty(_carStatusFilePath) || string.IsNullOrEmpty(_fuelFilePath))
                throw new FileNotFoundException("One or both file paths are missing in configuration.");

            // Initial load of data
            ReloadData();

            // Setup file watchers
            SetupFileWatcher(_carStatusFilePath);
            SetupFileWatcher(_fuelFilePath);
        }

        public List<Car> LoadCarStatus()
        {
            var carStatuses = new List<Car>();

            Debug.WriteLine("Loading car status data...");
            try
            {
                foreach (var line in File.ReadLines(_carStatusFilePath))
                {
                    var fields = line.Split(',');
                    var car = new Car
                    {
                        Vin = fields[0].Trim(),
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
            catch (Exception)
            {
                Debug.WriteLine("Error loading car status data.");
            }

            return carStatuses;
        }

        public Dictionary<string, FuelStatistics> LoadFuelData()
        {
            var fuelEntries = new Dictionary<string, FuelStatistics>();

            Debug.WriteLine("Loading fuel data...");
            try
            {
                foreach (var line in File.ReadLines(_fuelFilePath))
                {
                    var fields = line.Split(',');
                    var numberPlate = fields[0];
                    var fuelInLitres = double.Parse(fields[1]);
                    var mileage = int.Parse(fields[2]);

                    if (!fuelEntries.ContainsKey(numberPlate))
                    {
                        fuelEntries[numberPlate] = new FuelStatistics();
                    }

                    var fuelStats = fuelEntries[numberPlate];
                    fuelStats.TotalFuel += fuelInLitres;
                    fuelStats.TotalMileage += mileage;
                    fuelStats.AverageFuelConsumption = fuelStats.TotalMileage > 0
                        ? (fuelStats.TotalFuel / fuelStats.TotalMileage) * 100 // L/100 km format
                        : 0;
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Error loading fuel data.");
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
            Debug.WriteLine($"Setting up file watcher for {filePath}...");
            var watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(filePath),
                Filter = Path.GetFileName(filePath),
                NotifyFilter = NotifyFilters.LastWrite
            };

            watcher.Changed += (s, e) => OnFileChanged();
            watcher.EnableRaisingEvents = true;

            _fileWatchers.Add(watcher); // Keep strong reference to avoid garbage collection
        }

        public event Action FileChanged;

        private void OnFileChanged()
        {
            Debug.WriteLine("File changed. Reloading data...");
            try
            {
                ReloadData();
                FileChanged?.Invoke();
            }
            catch (Exception)
            {
               Debug.WriteLine("Error reloading data on file change.");
            }
        }

        private int TryParseMileage(string mileageString)
        {
            return int.TryParse(mileageString, out int mileage) ? mileage : 0;
        }

        public void ReloadData()
        {
            Debug.WriteLine("Reloading data...");
            var carStatuses = LoadCarStatus();
            CarStatusCollection.Clear();

            foreach (var car in carStatuses)
            {
                CarStatusCollection.Add(car);
            }
        }
    }
}
