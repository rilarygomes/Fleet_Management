using FleetManagement.Application.DTOs;
using FleetManagement.Application.Services;
using FleetManagement.Application.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IValidator<VehicleDto>, VehicleValidator>();
builder.Services.AddScoped<IValidator<DriverDto>, DriverValidator>();
builder.Services.AddScoped<IValidator<TripDto>, TripValidator>();

builder.Services.AddScoped<VehicleService>();
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<TripService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
