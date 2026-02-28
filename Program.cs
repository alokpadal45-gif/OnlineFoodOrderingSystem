using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineFoodOrderingSystem.Data;
using OnlineFoodOrderingSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Configure Database Context with SQLite
// This sets up the connection to the database using the connection string from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure ASP.NET Core Identity with custom User and Role models
// This sets up the authentication and authorization system
builder.Services.AddIdentity<User, Role>(options =>
{
    // Password requirements configuration
    options.Password.RequireDigit = true;              // Password must contain at least one digit (0-9)
    options.Password.RequireLowercase = true;          // Password must contain at least one lowercase letter
    options.Password.RequireUppercase = true;          // Password must contain at least one uppercase letter
    options.Password.RequireNonAlphanumeric = true;    // Password must contain at least one special character
    options.Password.RequiredLength = 6;               // Minimum password length is 6 characters

    // User requirements configuration
    options.User.RequireUniqueEmail = true;            // Each user must have a unique email address

    // Sign-in requirements configuration
    options.SignIn.RequireConfirmedEmail = false;      // Users can sign in without confirming email
    options.SignIn.RequireConfirmedAccount = false;    // Users can sign in without confirming account

    // Lockout settings (for security against brute force attacks)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);  // Account locked for 5 minutes
    options.Lockout.MaxFailedAccessAttempts = 5;                       // Lock after 5 failed attempts
    options.Lockout.AllowedForNewUsers = true;                         // Apply lockout to new users
})
.AddEntityFrameworkStores<ApplicationDbContext>()      // Use Entity Framework for storing user data
.AddDefaultTokenProviders();                            // Add token providers for password reset, etc.

// Configure cookie-based authentication
// This sets up how the application handles user sessions
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";               // Redirect to this path when user is not authenticated
    options.LogoutPath = "/Account/Logout";             // Path for logout
    options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect when user doesn't have permission
    options.ExpireTimeSpan = TimeSpan.FromHours(2);     // Cookie expires after 2 hours of inactivity
    options.SlidingExpiration = true;                   // Reset expiration time with each request
    options.Cookie.HttpOnly = true;                     // Cookie not accessible via JavaScript (security)
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Cookie security based on request
});

// Add MVC services (Controllers and Views)
// This enables the MVC pattern in the application
builder.Services.AddControllersWithViews();

// register application services
builder.Services.AddScoped<OnlineFoodOrderingSystem.Services.IFoodService, OnlineFoodOrderingSystem.Services.FoodService>();

// Reduce noise from EF Core during startup by only logging warnings and above
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command",
    Microsoft.Extensions.Logging.LogLevel.Warning);

var app = builder.Build();

// Seed database with initial data on first launch only
// This creates default roles, admin user, and menu items
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Get required services
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();

        // Only run seeding if important tables are empty; the methods themselves
        // already guard against duplicates, but checking early avoids some EF work.
        if (!context.Menus.Any() || !context.Users.Any() || !context.Roles.Any())
        {
            await DbSeeder.SeedDatabase(context, userManager, roleManager);
        }
    }
    catch (Exception ex)
    {
        // Log any errors during seeding
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline

// Configure error handling based on environment
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");  // Use custom error page in production
    app.UseHsts();                           // Use HTTP Strict Transport Security
}

app.UseHttpsRedirection();    // Redirect HTTP requests to HTTPS
app.UseStaticFiles();         // Enable serving static files (CSS, JS, images)

app.UseRouting();             // Enable routing

// Add custom middleware that logs unhandled exceptions before they are handled by the framework
app.UseMiddleware<OnlineFoodOrderingSystem.Middleware.ExceptionLoggingMiddleware>();

// Enable authentication and authorization middleware
// IMPORTANT: Authentication must come before Authorization
app.UseAuthentication();      // Enable authentication (who is the user?)
app.UseAuthorization();       // Enable authorization (what can the user do?)

// Configure default route pattern for MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();