using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Components;
using ModernBillingApp.Data;
using ModernBillingApp.Models;
using ModernBillingApp.Services;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// --- PASTE THIS CODE ---

// 1. Get the database connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    "Server=(localdb)\\mssqllocaldb;Database=ModernBillingAppDb;Trusted_Connection=true;MultipleActiveResultSets=true";

// 2. Add our AppDbContext to the application's services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- PASTE THIS CODE ---

// 3. Add our new services
builder.Services.AddSingleton<SessionService>(); // Singleton for session management
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<BillingService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<SettingsService>();
builder.Services.AddScoped<DatabaseService>();
// Indian Market Services
builder.Services.AddScoped<IndianGSTService>();
builder.Services.AddScoped<BarcodeService>();
builder.Services.AddScoped<AuditService>();
// Payment and Return Services
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<PurchaseService>();
builder.Services.AddScoped<BillReturnService>();
builder.Services.AddScoped<LoyaltyService>();
builder.Services.AddScoped<ExpiryService>();
// New Services
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<WhatsAppService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddSingleton<ThemeService>();
builder.Services.AddScoped<StaffService>(); // New service from NewModernBillingApp
builder.Services.AddScoped<CashEntryService>(); // New service from NewModernBillingApp

// Add logging from NewModernBillingApp
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();



// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// --- PASTE THIS CODE ---

// This code runs once on startup to create default roles and an admin user
// This code runs once on startup to create default roles and an admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var authService = services.GetRequiredService<AuthService>();
        var logger = services.GetRequiredService<ILogger<Program>>(); // Get logger

        // For development, ensure the database is reset to fix schema issues.
        // This is destructive and should not be used in production without careful consideration.
        if (app.Environment.IsDevelopment())
        {
            logger.LogInformation("Attempting to delete and recreate database in Development environment.");
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            logger.LogInformation("Database deleted and recreated.");
        }

        // Create default roles if they don't exist
        if (!await context.UserRoles.AnyAsync(r => r.RoleName == "SuperAdmin"))
        {
            await context.UserRoles.AddAsync(new UserRole { RoleName = "SuperAdmin" });
        }
        if (!await context.UserRoles.AnyAsync(r => r.RoleName == "Admin"))
        {
            await context.UserRoles.AddAsync(new UserRole { RoleName = "Admin" });
        }
        if (!await context.UserRoles.AnyAsync(r => r.RoleName == "Staff"))
        {
            await context.UserRoles.AddAsync(new UserRole { RoleName = "Staff" });
        }
        if (!await context.UserRoles.AnyAsync(r => r.RoleName == "User"))
        {
            await context.UserRoles.AddAsync(new UserRole { RoleName = "User" });
        }
        await context.SaveChangesAsync();
        logger.LogInformation("Default roles ensured.");


        // Initialize permissions
        var permissionService = services.GetRequiredService<PermissionService>();
        await permissionService.InitializeDefaultPermissions();
        logger.LogInformation("Default permissions initialized.");


        // Create a default admin user if no users exist
        if (!await context.Users.AnyAsync())
        {
            var adminRole = await context.UserRoles.FirstAsync(r => r.RoleName == "Admin");
            authService.CreatePasswordHash("admin", out byte[] hash, out byte[] salt);

            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@store.com",
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = adminRole
            };
            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
            logger.LogInformation("Default 'admin' user created: admin / admin");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database or initializing roles/users.");
    }
}

// --- END OF PASTE ---


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
