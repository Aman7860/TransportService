using Serilog;
using Serilog.Events;
using TransportService.Endpoints;
using TransportService.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/transportservice-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        restrictedToMinimumLevel: LogEventLevel.Warning)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddApplicationServices(builder.Configuration);
var app = builder.Build();

//// Seed initial admin user
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//    if (!await db.Users.AnyAsync())
//    {
//        var admin = new User
//        {
//            Id = Guid.NewGuid(),
//            Email = "admin@transport.com",
//            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
//            Role = "Admin",
//            IsActive = true
//        };

//        db.Users.Add(admin);
//        await db.SaveChangesAsync();
//    }
//}

app.UseApplicationPipeline();

app.MapVehicleEndpoints();
app.MapAuthEndpoints();
////Testing Middleware 
//app.MapGet("/", () => "TransportService API is running");

app.Run();