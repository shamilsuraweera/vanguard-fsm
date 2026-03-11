using Microsoft.AspNetCore.Components.Web;
using VanguardFSM.Web.Components;
using NetTopologySuite.IO.Converters;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Blazor Interactive Server services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2. Configure Global JSON Options for NetTopologySuite
// This ensures that any data coming from the API with "Location" points is understood
var jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};
jsonOptions.Converters.Add(new GeoJsonConverterFactory());

// 3. Register HttpClient with the custom JSON options
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5085/") 
});

// 4. Inject the Serializer Options so components can use them for manual parsing if needed
builder.Services.AddSingleton(jsonOptions);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Ensure the RenderMode is set to InteractiveServer to allow Drag and Drop and Map Interop
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();