# Entity Framework Core Migration Guide
## ModernBillingApp Database Migration

This guide provides step-by-step instructions for setting up the database using Entity Framework Core migrations.

---

## Prerequisites

1. .NET 8.0 SDK installed
2. SQL Server (Express/Developer/Standard) or SQLite for development
3. Entity Framework Core tools installed

---

## Installing EF Core Tools

If not already installed, run:

```bash
dotnet tool install --global dotnet-ef
```

Or update existing installation:

```bash
dotnet tool update --global dotnet-ef
```

---

## Configuration Steps

### 1. Update Connection String

Open `appsettings.json` and configure your database connection:

#### For SQL Server:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ModernBillingDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

#### For SQL Server with Authentication:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ModernBillingDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;MultipleActiveResultSets=true"
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

---

## Creating and Applying Migrations

### Step 1: Navigate to Project Directory
```bash
cd ModernBillingApp/ModernBillingApp
```

### Step 2: Remove Existing Migrations (If Any)
```bash
# Remove Migrations folder if it exists
rm -rf Migrations
```

### Step 3: Create Initial Migration
```bash
dotnet ef migrations add InitialCreate
```

This will create a `Migrations` folder with the initial migration files.

### Step 4: Review Migration Files
Check the generated migration files in the `Migrations` folder:
- `YYYYMMDDHHMMSS_InitialCreate.cs` - The migration code
- `YYYYMMDDHHMMSS_InitialCreate.Designer.cs` - Migration metadata
- `AppDbContextModelSnapshot.cs` - Current model snapshot

### Step 5: Apply Migration to Database
```bash
dotnet ef database update
```

This will:
- Create the database if it doesn't exist
- Create all tables with proper relationships
- Set up foreign keys and indexes

---

## Useful EF Core Commands

### List All Migrations
```bash
dotnet ef migrations list
```

### Remove Last Migration (Before Applying)
```bash
dotnet ef migrations remove
```

### Update to Specific Migration
```bash
dotnet ef database update MigrationName
```

### Rollback to Previous Migration
```bash
dotnet ef database update PreviousMigrationName
```

### Generate SQL Script (Without Applying)
```bash
dotnet ef migrations script
```

### Generate SQL Script for Specific Migration Range
```bash
dotnet ef migrations script FromMigration ToMigration
```

### Drop Database
```bash
dotnet ef database drop
```

### Drop Database (Force - No Confirmation)
```bash
dotnet ef database drop --force
```

---

## Adding New Migrations (After Model Changes)

When you modify any model classes:

1. **Add New Migration:**
   ```bash
   dotnet ef migrations add DescriptiveNameForChange
   ```
   Examples:
   - `dotnet ef migrations add AddProductBarcodeField`
   - `dotnet ef migrations add UpdateCustomerTable`
   - `dotnet ef migrations add AddStaffModule`

2. **Review Generated Migration:**
   Check the generated migration file to ensure it matches your intent.

3. **Apply Migration:**
   ```bash
   dotnet ef database update
   ```

---

## Database Seeding

### Option 1: Using EF Core Seeding (Recommended)

Add seed data in `AppDbContext.cs` using `OnModelCreating`:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Seed Roles
    modelBuilder.Entity<UserRole>().HasData(
        new UserRole { Id = 1, RoleName = "SuperAdmin", Description = "Full access" },
        new UserRole { Id = 2, RoleName = "Admin", Description = "Admin access" },
        new UserRole { Id = 3, RoleName = "Staff", Description = "Staff access" }
    );

    // Add more seed data...
}
```

Then create and apply migration:
```bash
dotnet ef migrations add SeedInitialData
dotnet ef database update
```

### Option 2: Using SQL Script

Execute the provided `Database_Migration_Script.sql` using SQL Server Management Studio or command line:

```bash
sqlcmd -S localhost -d ModernBillingDB -i Database_Migration_Script.sql
```

---

## Troubleshooting

### Error: "Build Failed"
**Solution:** Ensure your project builds successfully first:
```bash
dotnet build
```

### Error: "Cannot connect to database"
**Solutions:**
1. Verify SQL Server is running
2. Check connection string in `appsettings.json`
3. Ensure firewall allows SQL Server connections
4. Test connection using SQL Server Management Studio

### Error: "A network-related or instance-specific error"
**Solution:** Add `TrustServerCertificate=True` to connection string.

### Error: "Foreign key constraint conflicts"
**Solution:** Drop database and recreate:
```bash
dotnet ef database drop --force
dotnet ef database update
```

### Error: "Migration already applied"
**Solution:** Remove the migration and recreate:
```bash
dotnet ef migrations remove
dotnet ef migrations add NewMigrationName
```

### Error: "The entity type requires a primary key"
**Solution:** Ensure all model classes have an `Id` property or `[Key]` attribute.

---

## Production Deployment

### 1. Generate SQL Script for Production
```bash
dotnet ef migrations script --idempotent --output Deploy.sql
```

The `--idempotent` flag ensures the script can run multiple times safely.

### 2. Review and Test Script
- Review the generated `Deploy.sql`
- Test on staging environment first
- Backup production database before applying

### 3. Apply to Production Database
```bash
# Using sqlcmd
sqlcmd -S ProductionServer -d ProductionDB -U username -P password -i Deploy.sql

# Or manually in SSMS
```

### 4. Alternative: Direct Update (Not Recommended for Production)
```bash
dotnet ef database update --connection "Server=prod;Database=ProdDB;..."
```

---

## Best Practices

1. **Always backup database before migrations**
   ```sql
   BACKUP DATABASE ModernBillingDB TO DISK = 'C:\Backup\ModernBillingDB.bak'
   ```

2. **Use descriptive migration names**
   - Good: `AddProductExpiryTracking`
   - Bad: `Update1`, `Fix`, `Changes`

3. **Review migrations before applying**
   - Check generated migration code
   - Verify no data loss operations

4. **Keep migrations small and focused**
   - One logical change per migration
   - Easier to rollback if needed

5. **Never modify applied migrations**
   - Always create new migration for changes
   - Modifying applied migrations causes conflicts

6. **Test migrations on development/staging first**
   - Never apply untested migrations to production

7. **Document breaking changes**
   - Note any manual steps required
   - Document data migration scripts if needed

---

## Database Schema Verification

After applying migrations, verify the schema:

### Check Tables
```sql
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

### Check Foreign Keys
```sql
SELECT 
    fk.name AS FK_Name,
    tp.name AS Parent_Table,
    cp.name AS Parent_Column,
    tr.name AS Referenced_Table,
    cr.name AS Referenced_Column
FROM sys.foreign_keys fk
INNER JOIN sys.tables tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.tables tr ON fk.referenced_object_id = tr.object_id
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id
INNER JOIN sys.columns cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id
ORDER BY tp.name, fk.name;
```

### Check Indexes
```sql
SELECT 
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.name IS NOT NULL
ORDER BY t.name, i.name;
```

---

## Quick Start Commands

For a fresh database setup:

```bash
# 1. Clean start
dotnet ef database drop --force
dotnet ef migrations remove

# 2. Create and apply migration
dotnet ef migrations add InitialCreate
dotnet ef database update

# 3. Verify
dotnet ef migrations list
```

---

## Support and Documentation

- **EF Core Documentation:** https://docs.microsoft.com/ef/core/
- **Migrations Overview:** https://docs.microsoft.com/ef/core/managing-schemas/migrations/
- **SQL Server Setup:** https://docs.microsoft.com/sql/

---

## Migration Checklist

Before applying migrations to production:

- [ ] Database backup completed
- [ ] Migration tested on development environment
- [ ] Migration tested on staging environment
- [ ] SQL script reviewed and approved
- [ ] Downtime window scheduled (if needed)
- [ ] Rollback plan prepared
- [ ] Team notified of deployment
- [ ] Monitoring in place post-deployment

---

**Last Updated:** 2024
**Version:** 1.0