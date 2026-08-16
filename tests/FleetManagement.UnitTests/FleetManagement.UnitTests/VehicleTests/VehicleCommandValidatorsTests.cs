using FleetManagement.Application.Vehicles.Commands.CreateVehicle;
using FleetManagement.Application.Vehicles.Commands.UpdateVehicle;
using FluentValidation.TestHelper;
using Xunit;

namespace FleetManagement.UnitTests.Vehicles;

public class VehicleCommandValidatorsTests
{
    private readonly CreateVehicleCommandValidator _createValidator;
    private readonly UpdateVehicleCommandValidator _updateValidator;

    public VehicleCommandValidatorsTests()
    {
        _createValidator = new CreateVehicleCommandValidator();
        _updateValidator = new UpdateVehicleCommandValidator();
    }

    [Fact]
    public void Create_Should_Fail_When_LicensePlate_Is_Empty()
    {
        var command = CreateValidCreateCommand();
        command.LicensePlate = string.Empty;

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LicensePlate)
            .WithErrorMessage("License plate is required.");
    }

    [Fact]
    public void Create_Should_Fail_When_LicensePlate_Does_Not_Have_Seven_Characters()
    {
        var command = CreateValidCreateCommand();
        command.LicensePlate = "ABC12";

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LicensePlate);
    }

    [Fact]
    public void Create_Should_Fail_When_Model_Is_Too_Short()
    {
        var command = CreateValidCreateCommand();
        command.Model = "T";

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Model);
    }

    [Fact]
    public void Create_Should_Fail_When_Year_Is_Below_1960()
    {
        var command = CreateValidCreateCommand();
        command.Year = 1950;

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Year);
    }

    [Fact]
    public void Create_Should_Pass_When_Command_Is_Valid()
    {
        var result = _createValidator.TestValidate(
            CreateValidCreateCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Update_Should_Fail_When_LicensePlate_Is_Empty()
    {
        var command = CreateValidUpdateCommand();
        command.LicensePlate = string.Empty;

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LicensePlate);
    }

    [Fact]
    public void Update_Should_Fail_When_LicensePlate_Does_Not_Have_Seven_Characters()
    {
        var command = CreateValidUpdateCommand();
        command.LicensePlate = "ABC12";

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LicensePlate);
    }

    [Fact]
    public void Update_Should_Fail_When_Model_Is_Too_Short()
    {
        var command = CreateValidUpdateCommand();
        command.Model = "T";

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Model);
    }

    [Fact]
    public void Update_Should_Fail_When_Year_Is_Invalid()
    {
        var command = CreateValidUpdateCommand();
        command.Year = 1950;

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Year);
    }

    [Fact]
    public void Update_Should_Pass_When_Command_Is_Valid()
    {
        var result = _updateValidator.TestValidate(
            CreateValidUpdateCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateVehicleCommand CreateValidCreateCommand()
    {
        return new CreateVehicleCommand
        {
            LicensePlate = "ABC1234",
            Model = "Toyota Corolla",
            Year = DateTime.Now.Year
        };
    }

    private static UpdateVehicleCommand CreateValidUpdateCommand()
    {
        return new UpdateVehicleCommand
        {
            LicensePlate = "XYZ9876",
            Model = "Honda Civic",
            Year = DateTime.Now.Year
        };
    }
}