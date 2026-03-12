using Microsoft.AspNetCore.Components.Web;
using VanguardFSM.Web.Components;
using NetTopologySuite.IO.Converters;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Interactive Server Services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2. Setup JSON options for the HttpClient
var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
jsonOptions.Converters.Add(new GeoJsonConverterFactory());

builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5085/") 
});

// 3. Make these options available for manual parsing
builder.Services.AddSingleton(jsonOptions);

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();