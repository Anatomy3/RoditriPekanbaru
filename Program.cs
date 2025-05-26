using Microsoft.EntityFrameworkCore;
using RoditriPekanbaru.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext with SQLite Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add session support for login
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Add HTTP Context accessor for session
builder.Services.AddHttpContextAccessor();

// Add memory cache for better performance
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use session before authorization
app.UseSession();

app.UseAuthorization();

// Updated routing - default to Landing page instead of Auth
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Landing}/{action=Index}/{id?}");

// Additional routes for better SEO and user experience
app.MapControllerRoute(
    name: "carDetails",
    pattern: "mobil/{id:int}",
    defaults: new { controller = "Landing", action = "CarDetails" });

app.MapControllerRoute(
    name: "brandCars",
    pattern: "merek/{merek}",
    defaults: new { controller = "Landing", action = "Index" });

app.MapControllerRoute(
    name: "search",
    pattern: "cari/{query?}",
    defaults: new { controller = "Landing", action = "Search" });

// Admin routes with prefix
app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=Home}/{action=Index}/{id?}");

// Initialize database with seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Ensure database is created
        context.Database.EnsureCreated();

        // Log database initialization
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Database initialized successfully");

        // Check if seed data exists
        if (!context.Admins.Any())
        {
            logger.LogInformation("No seed data found, database will be seeded on first access");
        }
        else
        {
            logger.LogInformation("Seed data already exists, skipping initialization");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database");
    }
}

app.Run();