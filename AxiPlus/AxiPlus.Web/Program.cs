using AxiCore.Diagnostics;
using AxiPlus.Web.Components;
using AxiPlus.Web.Services;
using AxiPlus.Web.Models;
using MudBlazor.Services;
using Microsoft.AspNetCore.DataProtection;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddFilter(level => level >= LogLevel.Warning);
builder.Logging.AddAxiCoreEnvironmentLogging(
    builder.Environment.ContentRootPath,
    "AxiPlus.Web");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var dataProtectionPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "DataProtectionKeys");

Directory.CreateDirectory(dataProtectionPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
    .SetApplicationName("AxiPlus.Web");

builder.Services.AddScoped(sp =>
{
    var config =
        sp.GetRequiredService<
            Microsoft.Extensions.Options.IOptions<ApiSettings>>();

    return new HttpClient
   {
        BaseAddress =
            new Uri(config.Value.BaseUrl)
    };
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthorizedApiClient>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<DashboardApiService>();

builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<ModuleApiService>();
builder.Services.AddScoped<LessonApiService>();
builder.Services.AddScoped<LessonLiveClassApiService>();
builder.Services.AddScoped<AssignmentApiService>();
builder.Services.AddScoped<AttendanceApiService>();
builder.Services.AddScoped<StudentPortalApiService>();
builder.Services.AddScoped<MentorPortalApiService>();
builder.Services.AddScoped<AdminPortalApiService>();
builder.Services.AddScoped<OperationsApiService>();
builder.Services.AddMudServices();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("READY -> AxiPlus.Web -> Open http://localhost:5094/login");
    Console.WriteLine($"READY -> AxiPlus.Web -> Listening URLs: {string.Join(", ", app.Urls)}");
});

app.Run();
