using FluentAssertions;

namespace Aleepartners.CarFleetManagement.Tests
{
    public class DataManagerTests
    {
        private readonly Mock<IDataManager> _mockDataManager;

        public DataManagerTests()
        {
            _mockDataManager = new Mock<IDataManager>();
        }

        [Fact]
        public void LoadCarStatus_ShouldReturnCorrectCarData()
        {
            // Arrange
            var carList = new List<Car>
            {
                new() { Vin = "VIN123", NumberPlate = "ABC123", Colour = "Red", Make = "Toyota", Model = "Corolla", Mileage = 10000, TotalFuelConsumed = 50, AverageFuelConsumption = 5 },
                new() { Vin = "VIN456", NumberPlate = "DEF456", Colour = "Blue", Make = "Honda", Model = "Civic", Mileage = 20000, TotalFuelConsumed = 60, AverageFuelConsumption = 6 }
            };

            _mockDataManager.Setup(dm => dm.LoadCarStatus()).Returns(carList);

            // Act
            var result = _mockDataManager.Object.LoadCarStatus();

            // Assert
            result.Should().HaveCount(2);
            result[0].Vin.Should().Be("VIN123");
            result[0].NumberPlate.Should().Be("ABC123");
            result[0].Colour.Should().Be("Red");
            result[0].Make.Should().Be("Toyota");
            result[0].Model.Should().Be("Corolla");
            result[0].Mileage.Should().Be(10000);
            result[0].TotalFuelConsumed.Should().Be(50);
            result[0].AverageFuelConsumption.Should().Be(5);

            result[1].Vin.Should().Be("VIN456");
            result[1].NumberPlate.Should().Be("DEF456");
            result[1].Colour.Should().Be("Blue");
            result[1].Make.Should().Be("Honda");
            result[1].Model.Should().Be("Civic");
            result[1].Mileage.Should().Be(20000);
            result[1].TotalFuelConsumed.Should().Be(60);
            result[1].AverageFuelConsumption.Should().Be(6);
        }

        [Fact]
        public void LoadFuelData_ShouldReturnCorrectFuelData()
        {
            // Arrange
            var fuelData = new Dictionary<string, FuelStatistics>
            {
                { "ABC123", new FuelStatistics { TotalFuel = 50, TotalMileage = 10000, EntryCount = 1, AverageFuelConsumption = 50 } },
                { "DEF456", new FuelStatistics { TotalFuel = 60, TotalMileage = 20000, EntryCount = 1, AverageFuelConsumption = 60 } }
            };
            _mockDataManager.Setup(dm => dm.LoadFuelData()).Returns(fuelData);

            // Act
            var result = _mockDataManager.Object.LoadFuelData();

            // Assert
            result.Should().HaveCount(2);
            result["ABC123"].TotalFuel.Should().Be(50);
            result["ABC123"].TotalMileage.Should().Be(10000);
            result["ABC123"].EntryCount.Should().Be(1);
            result["ABC123"].AverageFuelConsumption.Should().Be(50);

            result["DEF456"].TotalFuel.Should().Be(60);
            result["DEF456"].TotalMileage.Should().Be(20000);
            result["DEF456"].EntryCount.Should().Be(1);
            result["DEF456"].AverageFuelConsumption.Should().Be(60);
        }

        [Fact]
        public void ReloadData_ShouldUpdateCarStatusCollection()
        {
            // Arrange
            var carList = new ObservableCollection<Car>
            {
                new() { Vin = "VIN123", NumberPlate = "ABC123", Colour = "Red", Make = "Toyota", Model = "Corolla", Mileage = 10000, TotalFuelConsumed = 50, AverageFuelConsumption = 5 }
            };
            _mockDataManager.Setup(dm => dm.CarStatusCollection).Returns(carList);

            // Act
            _mockDataManager.Object.ReloadData();

            // Assert
            _mockDataManager.Object.CarStatusCollection.Should().ContainSingle();
            var car = _mockDataManager.Object.CarStatusCollection[0];
            car.Vin.Should().Be("VIN123");
            car.NumberPlate.Should().Be("ABC123");
            car.Colour.Should().Be("Red");
            car.Make.Should().Be("Toyota");
            car.Model.Should().Be("Corolla");
            car.Mileage.Should().Be(10000);
            car.TotalFuelConsumed.Should().Be(50);
            car.AverageFuelConsumption.Should().Be(5);
        }
    }
}
