namespace FleetMgt.Models
{
    //Helper class to store aggregated fuel data per car
    public class FuelStatistics
    {
        public double TotalFuel { get; set; }
        public int TotalMileage { get; set; }
        public int EntryCount { get; set; }
        public double AverageFuelConsumption { get; set; }
    }
}
