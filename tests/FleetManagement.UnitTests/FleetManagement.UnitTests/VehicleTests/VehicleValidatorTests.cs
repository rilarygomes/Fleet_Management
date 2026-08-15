using FleetManagement.Application.DTOs;
using FleetManagement.Application.Validators;
using FluentValidation.TestHelper;

public class VehicleValidatorsTests
{
    private readonly VehicleValidator _vehicleValidator;
    private readonly CreateVehicleDtoValidator _createValidator;
    private readonly UpdateVehicleValidator _updateValidator;

    public VehicleValidatorsTests()
    {
        _vehicleValidator = new VehicleValidator();
        _createValidator = new CreateVehicleDtoValidator();
        _updateValidator = new UpdateVehicleValidator();
    }

    // --- VehicleDto ---
    [Fact]
    public void VehicleValidator_Should_Fail_When_LicensePlate_Is_Invalid()
    {
        var dto = new VehicleDto
        {
            Id = Guid.NewGuid(),
            LicensePlate = "ABC12", // apenas 5 chars
            Model = "Fiat Uno",
            Year = 2020
        };

        var result = _vehicleValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(v => v.LicensePlate);
    }

    [Fact]
    public void VehicleValidator_Should_Pass_When_All_Valid()
    {
        var dto = new VehicleDto
        {
            Id = Guid.NewGuid(),
            LicensePlate = "ABC1234",
            Model = "Fiat Uno",
            Year = 2020
        };

        var result = _vehicleValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- CreateVehicleDto ---
    [Fact]
    public void CreateValidator_Should_Fail_When_Year_Is_Below_1960()
    {
        var dto = new CreateVehicleDto
        {
            LicensePlate = "XYZ9876",
            Model = "Toyota Corolla",
            Year = 1950
        };

        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(v => v.Year);
    }

    [Fact]
    public void CreateValidator_Should_Pass_When_All_Valid()
    {
        var dto = new CreateVehicleDto
        {
            LicensePlate = "XYZ9876",
            Model = "Toyota Corolla",
            Year = DateTime.Now.Year
        };

        var result = _createValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- UpdateVehicleDto ---
    [Fact]
    public void UpdateValidator_Should_Fail_When_Model_Is_Too_Short()
    {
        var dto = new UpdateVehicleDto
        {
            LicensePlate = "XYZ9876",
            Model = "T",
            Year = DateTime.Now.Year
        };

        var result = _updateValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(v => v.Model);
    }

    [Fact]
    public void UpdateValidator_Should_Pass_When_All_Valid()
    {
        var dto = new UpdateVehicleDto
        {
            LicensePlate = "XYZ9876",
            Model = "Toyota Corolla",
            Year = DateTime.Now.Year
        };

        var result = _updateValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
