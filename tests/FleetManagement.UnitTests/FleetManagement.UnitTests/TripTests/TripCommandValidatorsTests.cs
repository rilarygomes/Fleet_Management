using FleetManagement.Application.Trips.Commands.CreateTrip;
using FleetManagement.Application.Trips.Commands.UpdateTrip;
using FluentValidation.TestHelper;
using Xunit;

namespace FleetManagement.UnitTests.Trips;

public class TripCommandValidatorsTests
{
    private readonly CreateTripCommandValidator _createValidator;
    private readonly UpdateTripCommandValidator _updateValidator;

    public TripCommandValidatorsTests()
    {
        _createValidator = new CreateTripCommandValidator();
        _updateValidator = new UpdateTripCommandValidator();
    }

    [Fact]
    public void Create_Should_Fail_When_VehicleId_Is_Empty()
    {
        var command = CreateValidCreateCommand();
        command.VehicleId = Guid.Empty;

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.VehicleId);
    }

    [Fact]
    public void Create_Should_Fail_When_DriverId_Is_Empty()
    {
        var command = CreateValidCreateCommand();
        command.DriverId = Guid.Empty;

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DriverId);
    }

    [Fact]
    public void Create_Should_Fail_When_StartDate_Is_In_The_Past()
    {
        var command = CreateValidCreateCommand();
        command.StartDate = DateTime.Today.AddDays(-1);

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void Create_Should_Fail_When_EndDate_Is_Before_StartDate()
    {
        var command = CreateValidCreateCommand();

        command.StartDate = DateTime.Today.AddDays(3);
        command.EndDate = DateTime.Today.AddDays(2);

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Create_Should_Pass_When_Command_Is_Valid()
    {
        var result = _createValidator.TestValidate(
            CreateValidCreateCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Update_Should_Fail_When_VehicleId_Is_Empty()
    {
        var command = CreateValidUpdateCommand();
        command.VehicleId = Guid.Empty;

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.VehicleId);
    }

    [Fact]
    public void Update_Should_Fail_When_DriverId_Is_Empty()
    {
        var command = CreateValidUpdateCommand();
        command.DriverId = Guid.Empty;

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.DriverId);
    }

    [Fact]
    public void Update_Should_Fail_When_EndDate_Is_Before_StartDate()
    {
        var command = CreateValidUpdateCommand();

        command.StartDate = DateTime.Today.AddDays(5);
        command.EndDate = DateTime.Today.AddDays(4);

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Update_Should_Pass_When_Command_Is_Valid()
    {
        var result = _updateValidator.TestValidate(
            CreateValidUpdateCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateTripCommand CreateValidCreateCommand()
    {
        return new CreateTripCommand
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(2)
        };
    }

    private static UpdateTripCommand CreateValidUpdateCommand()
    {
        return new UpdateTripCommand
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(2)
        };
    }
}