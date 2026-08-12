namespace FleetManagement.Domain.Entities
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public required string LicensePlate { get; set; }
        public required string Model { get; set; }
        public int Year { get; set; }

        public Vehicle(string licensePlate, string model, int year)
        {
            if (string.IsNullOrWhiteSpace(licensePlate))
                throw new ArgumentException("License plate is required.");

            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("Model is required.");

            if (year < 1900 || year > DateTime.Now.Year + 1)
                throw new ArgumentException("Year is invalid.");

            Id = Guid.NewGuid();
            LicensePlate = licensePlate;
            Model = model;
            Year = year;
        }
    }
}
