using BlinkshieldAdmin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Read connection string from env var (Railway) or fallback to appsettings.json
var envConn = Environment.GetEnvironmentVariable("MYSQLCONN");
var connectionString = !string.IsNullOrEmpty(envConn)
    ? envConn
    : builder.Configuration.GetConnectionString("DefaultConnection");

// Register DbContext with MySQL
builder.Services.AddDbContext<BlinkDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// If Railway (or environment) provides a PORT, bind to it (0.0.0.0 to accept external traffic)
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    app.Urls.Clear();
    app.Urls.Add($"http://0.0.0.0:{port}");
}

// Ensure uploads folder exists (so static files won't fail)
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serve wwwroot files

// Serve /uploads as static content
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseSession();

app.UseRouting();
app.UseAuthorization();

// Default redirect to login page
app.MapGet("/", context =>
{
    context.Response.Redirect("/AdminLogin");
    return Task.CompletedTask;
});

app.MapControllers();
app.MapRazorPages();

app.Run();
