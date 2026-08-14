using Bogus;
using FleetManagement.Application.DTOs;
using FleetManagement.Application.Services;
using FleetManagement.Application.Validators;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FleetManagement.Infrastructure.Persistence;
using FleetManagement.Infrastructure.Persistence.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    c.ExampleFilters(); 
});

// registra os exemplos
builder.Services.AddSwaggerExamplesFromAssemblyOf<DriverDto>();

// DbContext
builder.Services.AddDbContext<FleetManagementDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<ITripRepository, TripRepository>();

// Validators
builder.Services.AddScoped<IValidator<VehicleDto>, VehicleValidator>();
builder.Services.AddScoped<IValidator<DriverDto>, DriverValidator>();
builder.Services.AddScoped<IValidator<TripDto>, TripValidator>();
builder.Services.AddScoped<IValidator<UpdateDriverDto>, UpdateDriverValidator>();
builder.Services.AddScoped<IValidator<UpdateVehicleDto>, UpdateVehicleValidator>();
builder.Services.AddScoped<IValidator<UpdateTripDto>, UpdateTripValidator>();

// Services
builder.Services.AddScoped<VehicleService>();
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<TripService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FleetManagement API V1");
        c.RoutePrefix = string.Empty; 
    });
}

// Seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FleetManagementDbContext>();
    db.Database.Migrate();

    if (!db.Vehicles.Any())
    {
        var vehicleFaker = new Faker<Vehicle>()
            .RuleFor(v => v.Id, f => Guid.NewGuid())
            .RuleFor(v => v.LicensePlate, f => f.Random.String2(7, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"))
            .RuleFor(v => v.Model, f => f.Vehicle.Model())
            .RuleFor(v => v.Year, f => f.Date.Past(20).Year);

        var driverFaker = new Faker<Driver>()
            .RuleFor(d => d.Id, f => Guid.NewGuid())
            .RuleFor(d => d.Name, f => f.Name.FullName())
            .RuleFor(d => d.LicenseNumber, f => f.Random.String2(11, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"))
            .RuleFor(d => d.LicenseExpirationDate, f => f.Date.Future(5));

        var vehicles = vehicleFaker.Generate(50);
        var drivers = driverFaker.Generate(50);

        db.Vehicles.AddRange(vehicles);
        db.Drivers.AddRange(drivers);

        var trips = new List<Trip>();
        for (int i = 0; i < 50; i++)
        {
            var startDate = DateTime.UtcNow.AddDays(i + 1);
            var endDate = startDate.AddDays(1);

            trips.Add(new Trip(
                Guid.NewGuid(),
                vehicles[i % vehicles.Count].Id,
                drivers[i % drivers.Count].Id,
                startDate,
                endDate
            ));
        }
        db.Trips.AddRange(trips);

        db.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
