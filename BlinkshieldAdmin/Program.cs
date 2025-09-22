using BlinkshieldAdmin.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Use connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register DbContext with MySQL
builder.Services.AddDbContext<BlinkDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serve wwwroot files

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
 