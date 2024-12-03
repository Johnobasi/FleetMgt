using System.ComponentModel;

namespace FleetMgt.Models
{
    public class Car : INotifyPropertyChanged
    {
        private string _vin;
        private string _numberPlate;
        private string _colour;
        private string _make;
        private string _model;
        private double _totalFuelConsumed;
        private double _averageFuelConsumption;
        private int    _mileage;

        public string Vin
        {
            get => _vin;
            set => SetProperty(ref _vin, value);
        }

        public string NumberPlate
        {
            get => _numberPlate;
            set => SetProperty(ref _numberPlate, value);
        }

        public string Colour
        {
            get => _colour;
            set => SetProperty(ref _colour, value);
        }

        public string Make
        {
            get => _make;
            set => SetProperty(ref _make, value);
        }

        public string Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        public double TotalFuelConsumed
        {
            get => _totalFuelConsumed;
            set => SetProperty(ref _totalFuelConsumed, value);
        }

        public double AverageFuelConsumption
        {
            get => _averageFuelConsumption;
            set => SetProperty(ref _averageFuelConsumption, value);
        }

        public int Mileage
        {
            get => _mileage;
            set => SetProperty(ref _mileage, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
    }
}
