using FleetManagement.Application.DTOs;
using FleetManagement.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

public class DriverValidatorsTests
{
    private readonly DriverValidator _driverValidator;
    private readonly CreateDriverDtoValidator _createValidator;
    private readonly UpdateDriverValidator _updateValidator;

    public DriverValidatorsTests()
    {
        _driverValidator = new DriverValidator();
        _createValidator = new CreateDriverDtoValidator();
        _updateValidator = new UpdateDriverValidator();
    }

    // --- DriverDto ---
    [Fact]
    public void DriverValidator_Should_Fail_When_Name_Is_Too_Short()
    {
        var dto = new DriverDto
        {
            Id = Guid.NewGuid(),
            Name = "Jo",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        var result = _driverValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(d => d.Name);
    }

    [Fact]
    public void DriverValidator_Should_Fail_When_LicenseExpirationDate_Is_Past()
    {
        var dto = new DriverDto
        {
            Id = Guid.NewGuid(),
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.Today.AddDays(-1)
        };

        var result = _driverValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(d => d.LicenseExpirationDate);
    }

    [Fact]
    public void DriverValidator_Should_Pass_When_All_Valid()
    {
        var dto = new DriverDto
        {
            Id = Guid.NewGuid(),
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        var result = _driverValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- CreateDriverDto ---
    [Fact]
    public void CreateValidator_Should_Fail_When_LicenseNumber_Is_Invalid_Length()
    {
        var dto = new CreateDriverDto
        {
            Name = "Carlos",
            LicenseNumber = "12345", // apenas 5 chars
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        var result = _createValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(d => d.LicenseNumber);
    }

    [Fact]
    public void CreateValidator_Should_Pass_When_All_Valid()
    {
        var dto = new CreateDriverDto
        {
            Name = "Maria",
            LicenseNumber = "98765432101",
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        var result = _createValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // --- UpdateDriverDto ---
    [Fact]
    public void UpdateValidator_Should_Fail_When_Name_Is_Empty()
    {
        var dto = new UpdateDriverDto
        {
            Name = "",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        var result = _updateValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(d => d.Name);
    }

    [Fact]
    public void UpdateValidator_Should_Fail_When_LicenseExpirationDate_Is_Past()
    {
        var dto = new UpdateDriverDto
        {
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.Today.AddDays(-1)
        };

        var result = _updateValidator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(d => d.LicenseExpirationDate);
    }

    [Fact]
    public void UpdateValidator_Should_Pass_When_All_Valid()
    {
        var dto = new UpdateDriverDto
        {
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        var result = _updateValidator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
