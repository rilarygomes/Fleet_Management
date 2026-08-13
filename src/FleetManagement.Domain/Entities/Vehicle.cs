namespace FleetManagement.Domain.Entities
{
    public class Vehicle
    {
        public Guid Id { get; private set; }
        public string LicensePlate { get; private set; }
        public string Model { get; private set; }
        public int Year { get; private set; }

        public Vehicle(Guid id, string licensePlate, string model, int year)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Vehicle Id is required.");

            if (string.IsNullOrWhiteSpace(licensePlate))
                throw new ArgumentException("License plate is required.");

            if (licensePlate.Length < 7 || licensePlate.Length > 7)
                throw new ArgumentException("License plate must be 7 characters.");

            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("Model is required.");

            if (model.Length < 2)
                throw new ArgumentException("Model must have at least 2 characters.");

            if (year < 1960 || year > DateTime.Now.Year + 1)
                throw new ArgumentException("Year must be between 1960 and next year.");

            Id = id;
            LicensePlate = licensePlate;
            Model = model;
            Year = year;
        }
    }
}
