
using Microsoft.EntityFrameworkCore;
using ModernBillingApp.Components;
using ModernBillingApp.Data;
using ModernBillingApp.Models;
using ModernBillingApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Entity Framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    "Server=(localdb)\\mssqllocaldb;Database=ModernBillingAppDb;Trusted_Connection=true;MultipleActiveResultSets=true";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add custom services
builder.Services.AddSingleton<SessionService>(); // Singleton for session management

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>(); // From ModernBillingApp
builder.Services.AddScoped<EmployeeService>(); // From ModernBillingApp
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<SupplierService>(); // From ModernBillingApp
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<BillingService>();
builder.Services.AddScoped<ExpenseService>(); // From ModernBillingApp
builder.Services.AddScoped<DashboardService>(); // From ModernBillingApp
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<SettingsService>(); // From ModernBillingApp
builder.Services.AddScoped<DatabaseService>(); // From ModernBillingApp
builder.Services.AddScoped<CashEntryService>(); // From NewModernBillingApp
// Indian Market Services (from ModernBillingApp)
builder.Services.AddScoped<IndianGSTService>();
builder.Services.AddScoped<BarcodeService>();
builder.Services.AddScoped<AuditService>();
// Payment and Return Services (from ModernBillingApp)
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<PurchaseService>();
builder.Services.AddScoped<BillReturnService>();
builder.Services.AddScoped<LoyaltyService>();
builder.Services.AddScoped<ExpiryService>();
// New Services (from both)
builder.Services.AddScoped<PermissionService>();

builder.Services.AddScoped<ToastService>(); // From ModernBillingApp

// Add logging from NewModernBillingApp
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Initialize database and seed data (combined logic from both)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>(); // Get logger here

    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var authService = services.GetRequiredService<AuthService>();
        var permissionService = services.GetRequiredService<PermissionService>();


        // Option to delete and recreate database - uncomment if needed for fresh start
        // await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync(); // Apply pending migrations

        // Create default roles if they don't exist (from ModernBillingApp)
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

        // Initialize permissions (from ModernBillingApp)
        await permissionService.InitializeDefaultPermissions();


        // Create a default admin user if no users exist (combined logic)
        if (!await context.Users.AnyAsync())
        {
            // Use CreateUserAsync from NewModernBillingApp if available and preferred
            // Otherwise, use the ModernBillingApp logic

            // Check if SuperAdmin role exists for CreateUserAsync
            var superAdminRole = await context.UserRoles.FirstOrDefaultAsync(r => r.RoleName == "SuperAdmin");
            if (superAdminRole != null)
            {
                await authService.CreateUserAsync(
                    username: "superadmin",
                    email: "superadmin@modernadmin.com",
                    password: "Admin@123",
                    roleId: superAdminRole.Id, // Use the actual ID
                    fullName: "Super Administrator",
                    phone: "9999999999"
                );
                logger.LogInformation("Default SuperAdmin user created: superadmin / Admin@123");
            }
            else
            {
                // Fallback if SuperAdmin role is not found, use ModernBillingApp's admin creation
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
                logger.LogInformation("Default Admin user created: admin / admin");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database and seeding data.");
    }
}

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
