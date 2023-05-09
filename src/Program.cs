using BookBrowser;
using BookBrowser.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("BB_");

builder.WebHost.ConfigureKestrel(k =>
{
    k.ListenAnyIP(builder.Configuration.Get<ConfigurationOptions>().Port);
    k.AddServerHeader = false;
    k.AllowResponseHeaderCompression = true;
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddPooledDbContextFactory<MetadataContext>(k =>
{
    var opts = builder.Configuration.Get<ConfigurationOptions>();
    k.UseSqlite($"Data Source={Path.Join(opts.CalibreLibraryPath, "metadata.db")}");
});

builder.Services.AddSingleton<ContextFactory>(f => k => f.GetService<IDbContextFactory<MetadataContext>>().CreateDbContext());

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

app.Run();
