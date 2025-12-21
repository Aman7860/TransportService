
using TransportService.Endpoints;
using TransportService.Extensions;
using TransportService.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.UseApplicationPipeline();

app.MapVehicleEndpoints();
//app.MapGet("/", () => "TransportService API is running");
app.Run();