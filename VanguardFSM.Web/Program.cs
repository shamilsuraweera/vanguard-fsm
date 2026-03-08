using Microsoft.AspNetCore.Components.Web;
using VanguardFSM.Web.Components;
using NetTopologySuite.IO.Converters; // This requires the package above

var builder = WebApplication.CreateBuilder(args);

// 1. Add Blazor Server services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2. Register HttpClient for the Dashboard
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5085/") 
});

// 3. Configure JSON to handle NetTopologySuite Points
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new GeoJsonConverterFactory());
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();