using System.ComponentModel;

namespace Aleepartners.CarFleetManagement.Models
{

    //VIN, Number Plate, Colour, Make, Model, Mileage => properties of a car
    public class Car  : INotifyPropertyChanged
    {
        public string Vin { get; set; }
        public string NumberPlate { get; set; }
        public string Colour { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }


        // Computed properties for fuel details
        private double _totalFuelConsumed;
        public double TotalFuelConsumed
        {
            get => _totalFuelConsumed;
            set { _totalFuelConsumed = value; OnPropertyChanged(nameof(TotalFuelConsumed)); }
        }

        private double _averageFuelConsumption;
        public double AverageFuelConsumption
        {
            get => _averageFuelConsumption;
            set { _averageFuelConsumption = value; OnPropertyChanged(nameof(AverageFuelConsumption)); }
        }

        private int _mileage;
        public int Mileage
        {
            get => _mileage;
            set { _mileage = value; OnPropertyChanged(nameof(Mileage)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
