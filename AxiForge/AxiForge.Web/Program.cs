using AxiCore.Diagnostics;
using AxiForge.Web.Components;
using AxiForge.Web.Services;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddFilter(level => level >= LogLevel.Warning);
builder.Logging.AddAxiCoreEnvironmentLogging(
    builder.Environment.ContentRootPath,
    "AxiForge.Web");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var dataProtectionPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "DataProtectionKeys");

Directory.CreateDirectory(dataProtectionPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("AxiForge.Web");

builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<AxiForgeApiClient>();
builder.Services.AddScoped(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new HttpClient
    {
        BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]!)
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("READY -> AxiForge.Web -> Open http://localhost:5242/login");
    Console.WriteLine($"READY -> AxiForge.Web -> Listening URLs: {string.Join(", ", app.Urls)}");
});

app.Run();
