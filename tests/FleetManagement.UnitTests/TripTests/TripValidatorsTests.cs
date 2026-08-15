using FleetManagement.Application.DTOs;
using FleetManagement.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

public class TripValidatorsTests
{
    private readonly TripValidator _tripValidator;
    private readonly CreateTripDtoValidator _createValidator;
    private readonly UpdateTripValidator _updateValidator;

    public TripValidatorsTests()
    {
        _tripValidator = new TripValidator();
        _createValidator = new CreateTripDtoValidator();
        _updateValidator = new UpdateTripValidator();
    }

    // --- TripDto ---
    [Fact]
    public void TripValidator_Should_Fail_When_StartDate_Is_In_The_Past()
    {
        var dto = new TripDto
        {
            Id = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today.AddDays(1)
        };

        var result = _tripValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(t => t.StartDate);
    }

    [Fact]
    public void TripValidator_Should_Pass_When_All_Valid()
    {
        var dto = new TripDto
        {
            Id = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(2)
        };

        var result = _tripValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- CreateTripDto ---
    [Fact]
    public void CreateValidator_Should_Fail_When_EndDate_Before_StartDate()
    {
        var dto = new CreateTripDto
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.Today.AddDays(2),
            EndDate = DateTime.Today.AddDays(1)
        };

        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(t => t.EndDate);
    }

    [Fact]
    public void CreateValidator_Should_Pass_When_All_Valid()
    {
        var dto = new CreateTripDto
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(2)
        };

        var result = _createValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- UpdateTripDto ---
    [Fact]
    public void UpdateValidator_Should_Fail_When_VehicleId_Is_Empty()
    {
        var dto = new UpdateTripDto
        {
            VehicleId = Guid.Empty,
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(2)
        };

        var result = _updateValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(t => t.VehicleId);
    }

    [Fact]
    public void UpdateValidator_Should_Pass_When_All_Valid()
    {
        var dto = new UpdateTripDto
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(2)
        };

        var result = _updateValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
