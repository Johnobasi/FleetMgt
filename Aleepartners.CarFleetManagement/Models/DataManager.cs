using System.IO;

namespace Aleepartners.CarFleetManagement.Models
{
    public class DataManager
    {
        private const string StatusFilePath = "FileSystem\\status.ledger";
        private const string FuelFilePath = "FileSystem\\fuel.ledger";
        public DataManager()
        {
            SetupFileWatcher(StatusFilePath);
            SetupFileWatcher(FuelFilePath);
        }
        public List<Car> LoadCarStatus()
        {
            var carStatuses = new List<Car>();

            foreach (var line in File.ReadLines(StatusFilePath))
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

            return carStatuses;
        }

        public List<FuelEntry> LoadFuelData()
        {
            var fuelEntries = new List<FuelEntry>();

            foreach (var line in File.ReadLines(FuelFilePath))
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
