namespace Aleepartners.CarFleetManagement.Models
{

    //VIN, Number Plate, Colour, Make, Model, Mileage => properties of a car
    public class Car
    {
        public string Vin { get; set; }
        public string NumberPlate { get; set; }
        public string Colour { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Mileage { get; set; }

        // Computed properties for fuel details
        public double TotalFuelConsumed { get; set; }
        public double AverageFuelConsumption { get; set; }
    }

}
