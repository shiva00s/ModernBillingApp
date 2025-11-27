using Microsoft.EntityFrameworkCore;
using MergeModernApp.Components;
using MergeModernApp.Data;
using MergeModernApp.Models;
using MergeModernApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Get database connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    "Server=(localdb)\\mssqllocaldb;Database=MergeModernAppDb;Trusted_Connection=true;MultipleActiveResultSets=true";

// Add AppDbContext to the application's services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add core services
builder.Services.AddSingleton<SessionService>();
builder.Services.AddSingleton<ThemeService>();
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
builder.Services.AddScoped<CashEntryService>();

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

// Additional Services
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<WhatsAppService>();
builder.Services.AddScoped<ToastService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var authService = services.GetRequiredService<AuthService>();

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

        // Initialize permissions
        var permissionService = services.GetRequiredService<PermissionService>();
        await permissionService.InitializeDefaultPermissions();

        // Create a default admin user if no users exist
        if (!await context.Users.AnyAsync())
        {
            var adminRole = await context.UserRoles.FirstAsync(r => r.RoleName == "Admin");
            
            // Create default admin user using AuthService
            await authService.CreateUserAsync(
                username: "admin",
                email: "admin@mergemodern.com",
                password: "Admin@123",
                roleId: adminRole.Id,
                fullName: "Administrator",
                phone: "9999999999"
            );

            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Default admin user created: admin / Admin@123");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();