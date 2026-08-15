using FleetManagement.Application.Drivers.Commands.CreateDriver;
using FleetManagement.Application.Drivers.Commands.UpdateDriver;
using FluentValidation.TestHelper;
using Xunit;

namespace FleetManagement.UnitTests.Drivers;

public class DriverCommandValidatorsTests
{
    private readonly CreateDriverCommandValidator _createValidator;
    private readonly UpdateDriverCommandValidator _updateValidator;

    public DriverCommandValidatorsTests()
    {
        _createValidator = new CreateDriverCommandValidator();
        _updateValidator = new UpdateDriverCommandValidator();
    }

    // CreateDriverCommandValidator

    [Fact]
    public void Create_Should_Fail_When_Name_Is_Empty()
    {
        var command = new CreateDriverCommand
        {
            Name = string.Empty,
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Driver name is required.");
    }

    [Fact]
    public void Create_Should_Fail_When_Name_Has_Less_Than_Three_Characters()
    {
        var command = new CreateDriverCommand
        {
            Name = "Jo",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Driver name must have at least 3 characters.");
    }

    [Fact]
    public void Create_Should_Fail_When_LicenseNumber_Is_Empty()
    {
        var command = new CreateDriverCommand
        {
            Name = "Carlos",
            LicenseNumber = string.Empty,
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LicenseNumber);
    }

    [Fact]
    public void Create_Should_Fail_When_LicenseNumber_Does_Not_Have_Eleven_Digits()
    {
        var command = new CreateDriverCommand
        {
            Name = "Carlos",
            LicenseNumber = "12345",
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LicenseNumber);
    }

    [Fact]
    public void Create_Should_Fail_When_LicenseNumber_Contains_NonNumeric_Characters()
    {
        var command = new CreateDriverCommand
        {
            Name = "Carlos",
            LicenseNumber = "ABC12345678",
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LicenseNumber);
    }

    [Fact]
    public void Create_Should_Fail_When_LicenseExpirationDate_Is_Not_In_The_Future()
    {
        var command = new CreateDriverCommand
        {
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.Today
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LicenseExpirationDate)
            .WithErrorMessage("License expiration date must be in the future.");
    }

    [Fact]
    public void Create_Should_Pass_When_Command_Is_Valid()
    {
        var command = new CreateDriverCommand
        {
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    // UpdateDriverCommandValidator

    [Fact]
    public void Update_Should_Fail_When_Name_Is_Empty()
    {
        var command = CreateValidUpdateCommand();
        command.Name = string.Empty;

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Driver name is required.");
    }

    [Fact]
    public void Update_Should_Fail_When_Name_Has_Less_Than_Three_Characters()
    {
        var command = CreateValidUpdateCommand();
        command.Name = "Jo";

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Driver name must have at least 3 characters.");
    }

    [Fact]
    public void Update_Should_Fail_When_LicenseNumber_Is_Invalid()
    {
        var command = CreateValidUpdateCommand();
        command.LicenseNumber = "12345";

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LicenseNumber);
    }

    [Fact]
    public void Update_Should_Fail_When_LicenseNumber_Contains_NonNumeric_Characters()
    {
        var command = CreateValidUpdateCommand();
        command.LicenseNumber = "ABC12345678";

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LicenseNumber);
    }

    [Fact]
    public void Update_Should_Fail_When_LicenseExpirationDate_Is_In_The_Past()
    {
        var command = CreateValidUpdateCommand();
        command.LicenseExpirationDate = DateTime.Today.AddDays(-1);

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LicenseExpirationDate)
            .WithErrorMessage("License expiration date cannot be in the past.");
    }

    [Fact]
    public void Update_Should_Pass_When_Command_Is_Valid()
    {
        var command = CreateValidUpdateCommand();

        var result = _updateValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdateDriverCommand CreateValidUpdateCommand()
    {
        return new UpdateDriverCommand
        {
            Id = Guid.NewGuid(),
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };
    }
}