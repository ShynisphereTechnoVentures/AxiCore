using AxiCore.Diagnostics;
using AxiHire.Web.Components;
using AxiHire.Web.Services;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddFilter(level => level >= LogLevel.Warning);
builder.Logging.AddAxiCoreEnvironmentLogging(
    builder.Environment.ContentRootPath,
    "AxiHire.Web");

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var dataProtectionPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "DataProtectionKeys");

Directory.CreateDirectory(dataProtectionPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("AxiHire.Web");

builder.Services.AddScoped<AxiHireApiClient>();
builder.Services.AddScoped(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new HttpClient
    {
        BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]!)
    };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("READY -> AxiHire.Web -> Open http://localhost:5267/");
    Console.WriteLine($"READY -> AxiHire.Web -> Listening URLs: {string.Join(", ", app.Urls)}");
});

app.Run();
