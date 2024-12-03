namespace FleetMgt.Models
{

    //field 1 = Number Plate
    //field 2 = fuel in litres
    //field 3 = mileage
    //field 4 = cost(£)
    //filed 5 = date and time of purchase
    public class FuelEntry
    {
        public string NumberPlate { get; set; }
        public double FuelInLitres { get; set; }
        public int Mileage { get; set; }
        public decimal Cost { get; set; }
        public DateTime? DateAndTimeOfPurchase { get; set; }
    }
}
