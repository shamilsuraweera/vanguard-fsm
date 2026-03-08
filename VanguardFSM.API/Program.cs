using VanguardFSM.API.Data;
using Microsoft.EntityFrameworkCore;
using VanguardFSM.API.Hubs;
using NetTopologySuite.IO.Converters;

var builder = WebApplication.CreateBuilder(args);

// 1. Database & Spatial Services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    x => x.UseNetTopologySuite()));

// 2. JSON Configuration for Geometry (The GeoJSON translation fix)
builder.Services.AddControllers().AddJsonOptions(options => {
    // Translates spatial coordinates into readable JSON strings
    options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
    options.JsonSerializerOptions.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals;
});

// 3. SignalR & Swagger
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. CORS (Allows the Web app to talk to the API)
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<NotificationHub>("/notificationHub");
app.MapControllers();

app.Run();