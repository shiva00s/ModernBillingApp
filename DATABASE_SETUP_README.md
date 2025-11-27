# Database Setup Guide
## ModernBillingApp - Complete Database Migration Instructions

This guide provides comprehensive instructions for setting up the database for ModernBillingApp using multiple approaches.

---

## 📋 Table of Contents

1. [Prerequisites](#prerequisites)
2. [Database Options](#database-options)
3. [Method 1: Entity Framework Core Migrations (Recommended)](#method-1-entity-framework-core-migrations-recommended)
4. [Method 2: SQL Server Direct Script](#method-2-sql-server-direct-script)
5. [Method 3: MySQL/MariaDB Script](#method-3-mysqlmariadb-script)
6. [Post-Installation Steps](#post-installation-steps)
7. [Troubleshooting](#troubleshooting)
8. [Default Credentials](#default-credentials)

---

## Prerequisites

### Required Software

- **.NET 8.0 SDK** or later
- One of the following databases:
  - **SQL Server** (Express/Developer/Standard) - Recommended
  - **MySQL 8.0+** or **MariaDB 10.5+**
  - **SQLite** (for development only)

### Optional Tools

- **SQL Server Management Studio (SSMS)** - For SQL Server
- **MySQL Workbench** - For MySQL/MariaDB
- **Azure Data Studio** - Cross-platform alternative
- **Visual Studio 2022** or **VS Code** with C# extension

---

## Database Options

### SQL Server (Recommended for Production)
- ✅ Best performance for Windows environments
- ✅ Full support for advanced features
- ✅ Excellent for multi-user scenarios
- ✅ Free Express edition available

### MySQL/MariaDB
- ✅ Cross-platform compatibility
- ✅ Open-source and free
- ✅ Good for Linux/cloud deployments
- ⚠️ Some minor syntax differences

### SQLite
- ✅ Zero configuration
- ✅ Perfect for development/testing
- ⚠️ Not recommended for production
- ⚠️ Limited concurrent users

---

## Method 1: Entity Framework Core Migrations (Recommended)

This is the **preferred method** as it's database-agnostic and version-controlled.

### Step 1: Install EF Core Tools

```bash
# Install globally (one-time)
dotnet tool install --global dotnet-ef

# Or update if already installed
dotnet tool update --global dotnet-ef
```

### Step 2: Configure Connection String

Open `appsettings.json` and update the connection string:

#### For SQL Server (Local):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ModernBillingDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

#### For SQL Server (With Password):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ModernBillingDB;User Id=sa;Password=YourStrongPassword123;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

#### For MySQL/MariaDB:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ModernBillingDB;User=root;Password=YourPassword;Port=3306;"
  }
}
```

#### For SQLite (Development):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=ModernBilling.db"
  }
}
```

### Step 3: Update Program.cs (If Using MySQL)

If using MySQL, ensure you have the correct provider in `Program.cs`:

```csharp
// For SQL Server (default)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// For MySQL - Change to:
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// For SQLite - Change to:
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
```

### Step 4: Install Required NuGet Package

```bash
cd ModernBillingApp

# For SQL Server (already included)
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# For MySQL
dotnet add package Pomelo.EntityFrameworkCore.MySql

# For SQLite
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

### Step 5: Create and Apply Migration

```bash
# Navigate to project directory
cd ModernBillingApp

# Remove old migrations if any
Remove-Item -Recurse -Force Migrations

# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migration to database
dotnet ef database update
```

### Step 6: Verify Database Creation

```bash
# List all migrations
dotnet ef migrations list

# Check database connection
dotnet ef database drop --dry-run
```

**✅ Done! Your database is now ready.**

---

## Method 2: SQL Server Direct Script

Use this method for quick setup on SQL Server without EF Core.

### Step 1: Prepare SQL Server

1. Ensure SQL Server is running
2. Open SQL Server Management Studio (SSMS)
3. Connect to your server instance

### Step 2: Create Database

```sql
CREATE DATABASE ModernBillingDB;
GO
USE ModernBillingDB;
GO
```

### Step 3: Execute Migration Script

Option A: Using SSMS
1. Open `Database_Migration_Script.sql` in SSMS
2. Press F5 to execute
3. Wait for completion message

Option B: Using Command Line
```bash
sqlcmd -S localhost -E -i Database_Migration_Script.sql
```

Option C: Using PowerShell
```powershell
Invoke-Sqlcmd -ServerInstance "localhost" -InputFile "Database_Migration_Script.sql"
```

### Step 4: Update Connection String

Update `appsettings.json` with your SQL Server details:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ModernBillingDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**✅ Done! SQL Server database is configured.**

---

## Method 3: MySQL/MariaDB Script

Use this for MySQL or MariaDB deployment.

### Step 1: Prepare MySQL

1. Ensure MySQL/MariaDB is running
2. Open MySQL Workbench or command line
3. Connect to your server

### Step 2: Create Database

```sql
CREATE DATABASE ModernBillingDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE ModernBillingDB;
```

### Step 3: Execute Migration Script

Option A: Using MySQL Workbench
1. Open `Database_Migration_MySQL.sql`
2. Execute the script

Option B: Using Command Line
```bash
mysql -u root -p < Database_Migration_MySQL.sql
```

Option C: Using mysql command
```bash
mysql -u root -p
source /path/to/Database_Migration_MySQL.sql
```

### Step 4: Update Connection String

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ModernBillingDB;User=root;Password=yourpassword;Port=3306;"
  }
}
```

### Step 5: Update Program.cs for MySQL

Ensure you're using MySQL provider:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString, 
        ServerVersion.AutoDetect(connectionString)
    ));
```

**✅ Done! MySQL database is configured.**

---

## Post-Installation Steps

### 1. Verify Database Tables

Run this query to check all tables were created:

**SQL Server:**
```sql
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

**MySQL:**
```sql
SHOW TABLES;
```

Expected tables: 39 tables including:
- Users, UserRoles, RolePermissions
- Customers, Suppliers, Employees
- Products, Categories, StockLedgers
- Bills, BillItems, BillReturns
- PurchaseEntries, PurchaseItems
- Staff, StaffAttendances, StaffSalaries
- CashEntries, Notifications, etc.

### 2. Test Application Connection

```bash
# Build the application
dotnet build

# Run the application
dotnet run
```

### 3. Login with Default Credentials

Open browser and navigate to: `https://localhost:5001` or `http://localhost:5000`

**Default Login:**
- Username: `admin`
- Password: `Admin@123`

### 4. Change Default Password

⚠️ **IMPORTANT:** Change the default admin password immediately after first login!

1. Login with default credentials
2. Navigate to Settings → User Management
3. Select admin user
4. Update password
5. Save changes

### 5. Configure Shop Settings

1. Navigate to Settings → Shop Settings
2. Update:
   - Shop Name
   - Address and contact details
   - GST Number (if applicable)
   - Invoice prefixes
   - Tax rates
3. Save settings

---

## Troubleshooting

### Issue: "Cannot connect to database"

**Solutions:**
1. Verify database service is running:
   ```bash
   # SQL Server
   net start MSSQLSERVER
   
   # MySQL
   net start MySQL80
   ```

2. Check firewall settings
3. Verify connection string is correct
4. Test connection using database management tool

### Issue: "Login failed for user"

**Solutions:**
1. Check username and password in connection string
2. Verify SQL Server authentication mode (Windows vs Mixed)
3. Enable Mixed Mode authentication if needed
4. Restart SQL Server service after changes

### Issue: "Database already exists"

**Solutions:**
```bash
# Drop and recreate using EF Core
dotnet ef database drop --force
dotnet ef database update

# Or manually in SQL Server
DROP DATABASE ModernBillingDB;
```

### Issue: "Migration already applied"

**Solutions:**
```bash
# Remove last migration
dotnet ef migrations remove

# Or remove all and start fresh
Remove-Item -Recurse Migrations
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Issue: "Foreign key constraint conflict"

**Solutions:**
1. Drop database completely and recreate
2. Ensure correct table creation order
3. Disable foreign key checks temporarily (MySQL):
   ```sql
   SET FOREIGN_KEY_CHECKS = 0;
   -- Run scripts
   SET FOREIGN_KEY_CHECKS = 1;
   ```

### Issue: "The entity type requires a primary key"

**Solution:** This should not occur if using provided scripts. If it does:
1. Check all model classes have `Id` property
2. Rebuild project: `dotnet build`
3. Recreate migrations

### Issue: Build Warnings or Errors

**Solutions:**
```bash
# Clean and rebuild
dotnet clean
dotnet build --no-incremental

# Restore packages
dotnet restore

# Update tools
dotnet tool update --global dotnet-ef
```

---

## Default Credentials

### Admin User
- **Username:** `admin`
- **Password:** `Admin@123`
- **Role:** SuperAdmin
- **Email:** admin@modernbilling.com

### Default Roles Created
1. **SuperAdmin** - Full system access
2. **Admin** - Administrative access
3. **Manager** - Manager level access
4. **Staff** - Limited staff access
5. **Cashier** - Billing access only

### Default Categories
- Electronics
- Groceries
- Stationery
- Clothing
- Cosmetics
- Hardware
- Medicines
- Food Items
- Beverages
- Others

---

## Database Schema Overview

### Core Modules

#### 1. User Management
- `Users` - User accounts
- `UserRoles` - Role definitions
- `RolePermissions` - Permission matrix

#### 2. Customers
- `Customers` - Customer master
- `CustomerPayments` - Payment tracking
- `CustomerPaymentEnhanced` - Partial payments
- `LoyaltyTransactions` - Loyalty points

#### 3. Suppliers
- `Suppliers` - Supplier master
- `SupplierPayments` - Payment tracking

#### 4. Products & Inventory
- `Products` - Product catalog
- `Categories` - Product categories
- `StockLedgers` - Stock movement log
- `StockReturns` - Stock return entries
- `ExpiryAlerts` - Expiry notifications

#### 5. Sales & Billing
- `Bills` - Sales invoices
- `BillItems` - Invoice line items
- `BillReturns` - Sales returns
- `BillReturnItems` - Return line items

#### 6. Purchase Management
- `PurchaseEntries` - Purchase orders
- `PurchaseItems` - Purchase line items
- `PurchaseReturns` - Purchase returns
- `PurchaseReturnItems` - Return line items

#### 7. Staff Management
- `Staff` - Employee master
- `StaffAttendances` - Attendance tracking
- `StaffSalaries` - Salary processing

#### 8. Other Modules
- `Employees` - Employee records
- `CashEntries` - Income/Expense log
- `Notifications` - System notifications
- `ShopSettings` - Application settings

---

## Backup and Restore

### SQL Server Backup

```sql
-- Create backup
BACKUP DATABASE ModernBillingDB 
TO DISK = 'C:\Backups\ModernBillingDB.bak'
WITH FORMAT, COMPRESSION;

-- Restore backup
RESTORE DATABASE ModernBillingDB 
FROM DISK = 'C:\Backups\ModernBillingDB.bak'
WITH REPLACE;
```

### MySQL Backup

```bash
# Create backup
mysqldump -u root -p ModernBillingDB > ModernBillingDB_backup.sql

# Restore backup
mysql -u root -p ModernBillingDB < ModernBillingDB_backup.sql
```

---

## Production Deployment Checklist

- [ ] Database server configured and secured
- [ ] Strong admin password set
- [ ] Connection string using environment variables
- [ ] Database backup strategy in place
- [ ] Firewall rules configured
- [ ] SSL/TLS enabled for database connections
- [ ] Regular backup schedule automated
- [ ] Monitoring and alerting configured
- [ ] Access logs enabled
- [ ] Security audit completed

---

## Additional Resources

### Documentation
- [Entity Framework Core Docs](https://docs.microsoft.com/ef/core/)
- [SQL Server Documentation](https://docs.microsoft.com/sql/)
- [MySQL Documentation](https://dev.mysql.com/doc/)

### Support Files
- `Database_Migration_Script.sql` - SQL Server migration
- `Database_Migration_MySQL.sql` - MySQL migration
- `Database_EF_Migration_Guide.md` - Detailed EF guide

### Community
- Report issues on GitHub
- Check FAQ in documentation
- Contact support team

---

## Version History

- **v1.0** - Initial release
- Database schema version: 1.0
- Last updated: 2024

---

## Security Notes

1. **Never commit connection strings** with production credentials
2. **Use environment variables** for sensitive data
3. **Change default passwords** immediately
4. **Enable encryption** for database connections
5. **Regular backups** are essential
6. **Audit user access** regularly
7. **Keep database updated** with security patches

---

**Need Help?** Refer to the troubleshooting section or check the detailed guides in the repository.

**Happy Billing! 🎉**