using Microsoft.EntityFrameworkCore;
using NewModernBillingApp.Components;
using NewModernBillingApp.Data;
using NewModernBillingApp.Services;

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
builder.Services.AddSingleton<SessionService>();
builder.Services.AddSingleton<ThemeService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<StaffService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<BillingService>();
builder.Services.AddScoped<CashEntryService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<WhatsAppService>();

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

        // Delete and recreate database to fix schema issues
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        // Create default super admin user if no users exist
        if (!await context.Users.AnyAsync())
        {
            var authService = services.GetRequiredService<AuthService>();

            // Create default super admin user
            await authService.CreateUserAsync(
                username: "superadmin",
                email: "superadmin@modernadmin.com",
                password: "Admin@123",
                roleId: 1, // SuperAdmin role
                fullName: "Super Administrator",
                phone: "9999999999"
            );

            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Default SuperAdmin user created: superadmin / Admin@123");
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
