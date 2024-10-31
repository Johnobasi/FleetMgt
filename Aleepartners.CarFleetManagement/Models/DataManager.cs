using Microsoft.Extensions.Configuration;
using System.IO;

namespace Aleepartners.CarFleetManagement.Models
{
    public class DataManager
    {
        private readonly string _carStatusFilePath; 
        private readonly string  _fuelFilePath;
        public DataManager()
        {
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
            }
            catch (Exception ex)
            {
                Logger.LogError("Error loading car status data.", ex);
            }


            return carStatuses;
        }

        public List<FuelEntry> LoadFuelData()
        {
            var fuelEntries = new List<FuelEntry>();

            try
            {
                foreach (var line in File.ReadLines(_fuelFilePath))
                {
                    var fields = line.Split(',');
                    var entry = new FuelEntry
                    {
                        NumberPlate = fields[0],
                        FuelInLitres = double.Parse(fields[1]),
                        Mileage = int.Parse(fields[2]),
                        Cost = decimal.Parse(fields[3]),
                        DateAndTimeOfPurchase = DateTime.Parse(fields[4])
                    };
                    fuelEntries.Add(entry);
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
            FileChanged?.Invoke();
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
    }
}
