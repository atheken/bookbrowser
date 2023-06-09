using BookBrowser;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("BB_");


var config = builder.Configuration.Get<ConfigurationOptions>()!;
if (!Directory.Exists(config.CalibreLibraryPath))
{
    Console.WriteLine(
        $"The specified path for the calibre library (`{config.CalibreLibraryPath}`) does not exist, please set an existing path and run again:");
    Environment.Exit(-1);
}

builder.WebHost.ConfigureKestrel(k =>
{
    k.ListenAnyIP(config.Port);
    k.AddServerHeader = false;
    k.AllowResponseHeaderCompression = true;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddControllers();

builder.Services.AddSingleton(builder.Configuration.Get<ConfigurationOptions>()!);

builder.Services.AddPooledDbContextFactory<CalibreDbContext>(k =>
{
    k.UseSqlite($"Data Source={Path.Join(config.CalibreLibraryPath, "metadata.db")}")
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
});

builder.Services.AddSingleton<ContextFactory>(f =>
    _ => f.GetService<IDbContextFactory<CalibreDbContext>>().CreateDbContext());

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler("/api/error");
app.UseStaticFiles();
app.MapControllers();


app.Run();