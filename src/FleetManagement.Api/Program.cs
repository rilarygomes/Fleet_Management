using Bogus;
using FleetManagement.Api.Swagger;
using FleetManagement.Application.Drivers.Commands.CreateDriver;
using FleetManagement.Application.Drivers.Commands.DeleteDriver;
using FleetManagement.Application.Drivers.Commands.UpdateDriver;
using FleetManagement.Application.Drivers.GetDriver;
using FleetManagement.Application.Drivers.GetDrivers;
using FleetManagement.Application.Trips.Commands.CreateTrip;
using FleetManagement.Application.Trips.Commands.DeleteTrip;
using FleetManagement.Application.Trips.Commands.UpdateTrip;
using FleetManagement.Application.Trips.GetTrip;
using FleetManagement.Application.Trips.GetTrips;
using FleetManagement.Application.Vehicles.Commands.CreateVehicle;
using FleetManagement.Application.Vehicles.Commands.DeleteVehicle;
using FleetManagement.Application.Vehicles.Commands.UpdateVehicle;
using FleetManagement.Application.Vehicles.GetVehicle;
using FleetManagement.Application.Vehicles.GetVehicles;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FleetManagement.Infrastructure.Persistence;
using FleetManagement.Infrastructure.Persistence.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    options.ExampleFilters();
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<CreateDriverCommandExample>();

//-----------------------------------------------------
// Serilog
//-----------------------------------------------------

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/api-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

//-----------------------------------------------------
// Validation Response
//-----------------------------------------------------

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .Select(x => new
            {
                Field = x.Key,
                Messages = x.Value!.Errors
                    .Select(e => e.ErrorMessage)
            });

        return new BadRequestObjectResult(new
        {
            Title = "Validation failed",
            Status = 400,
            Errors = errors
        });
    };
});

//-----------------------------------------------------
// Database
//-----------------------------------------------------

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<FleetManagementDbContext>(options =>
        options.UseSqlite(
            builder.Configuration.GetConnectionString("DefaultConnection")));
}

//-----------------------------------------------------
// Repositories
//-----------------------------------------------------

builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<ITripRepository, TripRepository>();

//-----------------------------------------------------
// FluentValidation
//-----------------------------------------------------

builder.Services.AddValidatorsFromAssemblyContaining<CreateDriverCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateTripCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateVehicleCommandValidator>();

//-----------------------------------------------------
// Driver Handlers
//-----------------------------------------------------

builder.Services.AddScoped<CreateDriverCommandHandler>();
builder.Services.AddScoped<UpdateDriverCommandHandler>();
builder.Services.AddScoped<DeleteDriverCommandHandler>();
builder.Services.AddScoped<GetDriverQueryHandler>();
builder.Services.AddScoped<GetDriversQueryHandler>();

//-----------------------------------------------------
// Vehicle Handlers
//-----------------------------------------------------

builder.Services.AddScoped<CreateVehicleCommandHandler>();
builder.Services.AddScoped<UpdateVehicleCommandHandler>();
builder.Services.AddScoped<DeleteVehicleCommandHandler>();
builder.Services.AddScoped<GetVehicleQueryHandler>();
builder.Services.AddScoped<GetVehiclesQueryHandler>();

//-----------------------------------------------------
// Trip Handlers
//-----------------------------------------------------

builder.Services.AddScoped<CreateTripCommandHandler>();
builder.Services.AddScoped<UpdateTripCommandHandler>();
builder.Services.AddScoped<DeleteTripCommandHandler>();
builder.Services.AddScoped<GetTripQueryHandler>();
builder.Services.AddScoped<GetTripsQueryHandler>();

var app = builder.Build();

//-----------------------------------------------------
// Swagger
//-----------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FleetManagement API V1");
        c.RoutePrefix = string.Empty;
    });
}

//-----------------------------------------------------
// Database Seed
//-----------------------------------------------------

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<FleetManagementDbContext>();

    db.Database.Migrate();

    if (!db.Vehicles.Any())
    {
        var vehicleFaker = new Faker<Vehicle>()
            .RuleFor(v => v.Id, _ => Guid.NewGuid())
            .RuleFor(v => v.LicensePlate,
                f => f.Random.String2(7, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"))
            .RuleFor(v => v.Model, f => f.Vehicle.Model())
            .RuleFor(v => v.Year, f => f.Date.Past(20).Year);

        var driverFaker = new Faker<Driver>()
            .RuleFor(d => d.Id, _ => Guid.NewGuid())
            .RuleFor(d => d.Name, f => f.Name.FullName())
            .RuleFor(d => d.LicenseNumber,
                f => f.Random.String2(11, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"))
            .RuleFor(d => d.LicenseExpirationDate,
                f => f.Date.Future(5));

        var vehicles = vehicleFaker.Generate(50);
        var drivers = driverFaker.Generate(50);

        db.Vehicles.AddRange(vehicles);
        db.Drivers.AddRange(drivers);

        var trips = new List<Trip>();

        for (int i = 0; i < 50; i++)
        {
            var start = DateTime.UtcNow.AddDays(i + 1);

            trips.Add(new Trip(
                Guid.NewGuid(),
                vehicles[i % vehicles.Count].Id,
                drivers[i % drivers.Count].Id,
                start,
                start.AddDays(1)));
        }

        db.Trips.AddRange(trips);

        db.SaveChanges();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}