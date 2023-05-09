using BookBrowser;
using BookBrowser.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("BB_");


var config = builder.Configuration.Get<ConfigurationOptions>()!;
if (!Directory.Exists(config.CalibreLibraryPath))
{
    Console.WriteLine($"The specified path for the calibre library (`{config.CalibreLibraryPath}`) does not exist, please set an existing path and run again:");
    Environment.Exit(-1);
}

builder.WebHost.ConfigureKestrel(k =>
{
    k.ListenAnyIP(config.Port);
    k.AddServerHeader = false;
    k.AllowResponseHeaderCompression = true;
});
builder.Services.AddSingleton(builder.Configuration.Get<ConfigurationOptions>()!);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddPooledDbContextFactory<MetadataContext>(k =>
{
    k.UseSqlite($"Data Source={Path.Join(config.CalibreLibraryPath, "metadata.db")}");
});

builder.Services.AddSingleton<ContextFactory>(f => k => f.GetService<IDbContextFactory<MetadataContext>>()!.CreateDbContext());

builder.Services.AddSingleton<BookListingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapControllers();

app.Map("/api/books/{bookid:long}/cover", (long bookid) =>
{
    
});

app.Run();
