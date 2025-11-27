-- =============================================
-- ModernBillingApp Database Migration Script
-- SQL Server Compatible
-- =============================================
-- This script creates all tables for the Modern Billing Application
-- Execute this script on a new or existing SQL Server database
-- =============================================

USE [ModernBillingDB]
GO

-- =============================================
-- SECTION 1: DROP EXISTING TABLES (In Reverse Dependency Order)
-- =============================================
PRINT 'Dropping existing tables...'

IF OBJECT_ID('dbo.Notifications', 'U') IS NOT NULL DROP TABLE dbo.Notifications;
IF OBJECT_ID('dbo.RolePermissions', 'U') IS NOT NULL DROP TABLE dbo.RolePermissions;
IF OBJECT_ID('dbo.ExpiryAlerts', 'U') IS NOT NULL DROP TABLE dbo.ExpiryAlerts;
IF OBJECT_ID('dbo.LoyaltyTransactions', 'U') IS NOT NULL DROP TABLE dbo.LoyaltyTransactions;
IF OBJECT_ID('dbo.StaffSalaries', 'U') IS NOT NULL DROP TABLE dbo.StaffSalaries;
IF OBJECT_ID('dbo.StaffAttendances', 'U') IS NOT NULL DROP TABLE dbo.StaffAttendances;
IF OBJECT_ID('dbo.Staff', 'U') IS NOT NULL DROP TABLE dbo.Staff;
IF OBJECT_ID('dbo.BillReturnItems', 'U') IS NOT NULL DROP TABLE dbo.BillReturnItems;
IF OBJECT_ID('dbo.BillReturns', 'U') IS NOT NULL DROP TABLE dbo.BillReturns;
IF OBJECT_ID('dbo.BillItems', 'U') IS NOT NULL DROP TABLE dbo.BillItems;
IF OBJECT_ID('dbo.Bills', 'U') IS NOT NULL DROP TABLE dbo.Bills;
IF OBJECT_ID('dbo.PurchaseReturnItems', 'U') IS NOT NULL DROP TABLE dbo.PurchaseReturnItems;
IF OBJECT_ID('dbo.PurchaseReturns', 'U') IS NOT NULL DROP TABLE dbo.PurchaseReturns;
IF OBJECT_ID('dbo.PurchaseItems', 'U') IS NOT NULL DROP TABLE dbo.PurchaseItems;
IF OBJECT_ID('dbo.PurchaseEntries', 'U') IS NOT NULL DROP TABLE dbo.PurchaseEntries;
IF OBJECT_ID('dbo.StockReturns', 'U') IS NOT NULL DROP TABLE dbo.StockReturns;
IF OBJECT_ID('dbo.StockLedgers', 'U') IS NOT NULL DROP TABLE dbo.StockLedgers;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.CustomerPaymentEnhanced', 'U') IS NOT NULL DROP TABLE dbo.CustomerPaymentEnhanced;
IF OBJECT_ID('dbo.CustomerPayments', 'U') IS NOT NULL DROP TABLE dbo.CustomerPayments;
IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL DROP TABLE dbo.Customers;
IF OBJECT_ID('dbo.SupplierPayments', 'U') IS NOT NULL DROP TABLE dbo.SupplierPayments;
IF OBJECT_ID('dbo.Suppliers', 'U') IS NOT NULL DROP TABLE dbo.Suppliers;
IF OBJECT_ID('dbo.CashEntries', 'U') IS NOT NULL DROP TABLE dbo.CashEntries;
IF OBJECT_ID('dbo.Employees', 'U') IS NOT NULL DROP TABLE dbo.Employees;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.UserRoles', 'U') IS NOT NULL DROP TABLE dbo.UserRoles;
IF OBJECT_ID('dbo.ShopSettings', 'U') IS NOT NULL DROP TABLE dbo.ShopSettings;

PRINT 'Tables dropped successfully.'
GO

-- =============================================
-- SECTION 2: CREATE TABLES (In Dependency Order)
-- =============================================

-- =============================================
-- 1. USER ROLES & AUTHENTICATION
-- =============================================

CREATE TABLE dbo.UserRoles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE dbo.Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    PasswordSalt NVARCHAR(MAX) NOT NULL,
    RoleId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastLogin DATETIME2 NULL,
    CONSTRAINT FK_Users_UserRoles FOREIGN KEY (RoleId) REFERENCES dbo.UserRoles(Id)
);
GO

CREATE TABLE dbo.RolePermissions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoleId INT NOT NULL,
    PermissionKey NVARCHAR(100) NOT NULL,
    CanView BIT NOT NULL DEFAULT 0,
    CanCreate BIT NOT NULL DEFAULT 0,
    CanEdit BIT NOT NULL DEFAULT 0,
    CanDelete BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_RolePermissions_UserRoles FOREIGN KEY (RoleId) REFERENCES dbo.UserRoles(Id) ON DELETE CASCADE
);
GO

-- =============================================
-- 2. EMPLOYEES
-- =============================================

CREATE TABLE dbo.Employees (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EmpID NVARCHAR(50) NULL,
    Name NVARCHAR(100) NOT NULL,
    Contact NVARCHAR(15) NULL,
    Address NVARCHAR(300) NULL,
    Gender NVARCHAR(10) NULL,
    Salary DECIMAL(18, 2) NOT NULL DEFAULT 0,
    JoiningDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================
-- 3. CUSTOMERS
-- =============================================

CREATE TABLE dbo.Customers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CID NVARCHAR(50) NULL,
    Name NVARCHAR(100) NOT NULL,
    Address NVARCHAR(300) NULL,
    Gender NVARCHAR(10) NULL,
    Phone NVARCHAR(15) NULL,
    City NVARCHAR(50) NULL,
    State NVARCHAR(50) NULL,
    GSTNumber NVARCHAR(15) NULL,
    Email NVARCHAR(100) NULL,
    PinCode NVARCHAR(10) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastUpdated DATETIME2 NULL,
    CardNo NVARCHAR(50) NULL,
    Points DECIMAL(18, 2) NOT NULL DEFAULT 0,
    TotalPointsEarned DECIMAL(18, 2) NOT NULL DEFAULT 0,
    TotalPointsRedeemed DECIMAL(18, 2) NOT NULL DEFAULT 0,
    CustomerType NVARCHAR(50) NULL DEFAULT 'Regular',
    CreditLimit DECIMAL(18, 2) NOT NULL DEFAULT 0,
    OutstandingBalance DECIMAL(18, 2) NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    Notes NVARCHAR(200) NULL
);
GO

CREATE TABLE dbo.CustomerPayments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    PaymentDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    PaymentMode NVARCHAR(50) NOT NULL DEFAULT 'Cash',
    Amount DECIMAL(18, 2) NOT NULL,
    BillId INT NULL,
    TransactionReference NVARCHAR(100) NULL,
    Notes NVARCHAR(300) NULL,
    CreatedByUserId INT NOT NULL,
    CONSTRAINT FK_CustomerPayments_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CustomerPayments_Users FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id)
);
GO

CREATE TABLE dbo.CustomerPaymentEnhanced (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    BillId INT NULL,
    PaymentDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    AmountPaid DECIMAL(18, 2) NOT NULL,
    PaymentMode NVARCHAR(50) NOT NULL DEFAULT 'Cash',
    TransactionReference NVARCHAR(100) NULL,
    Notes NVARCHAR(300) NULL,
    RemainingBalance DECIMAL(18, 2) NOT NULL DEFAULT 0,
    CreatedByUserId INT NOT NULL,
    CONSTRAINT FK_CustomerPaymentEnhanced_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CustomerPaymentEnhanced_Users FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id)
);
GO

CREATE TABLE dbo.LoyaltyTransactions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    BillId INT NULL,
    TransactionType NVARCHAR(50) NOT NULL,
    Points DECIMAL(18, 2) NOT NULL,
    TransactionDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    Description NVARCHAR(200) NULL,
    CONSTRAINT FK_LoyaltyTransactions_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(Id) ON DELETE CASCADE
);
GO

-- =============================================
-- 4. SUPPLIERS
-- =============================================

CREATE TABLE dbo.Suppliers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SupplierID NVARCHAR(50) NULL,
    Name NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15) NULL,
    Email NVARCHAR(100) NULL,
    Address NVARCHAR(300) NULL,
    City NVARCHAR(50) NULL,
    State NVARCHAR(50) NULL,
    GSTNumber NVARCHAR(15) NULL,
    PinCode NVARCHAR(10) NULL,
    ContactPerson NVARCHAR(100) NULL,
    CreditLimit DECIMAL(18, 2) NOT NULL DEFAULT 0,
    OutstandingBalance DECIMAL(18, 2) NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastUpdated DATETIME2 NULL,
    Notes NVARCHAR(200) NULL
);
GO

CREATE TABLE dbo.SupplierPayments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SupplierId INT NOT NULL,
    PaymentDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    PaymentMode NVARCHAR(50) NOT NULL DEFAULT 'Cash',
    Amount DECIMAL(18, 2) NOT NULL,
    PurchaseEntryId INT NULL,
    TransactionReference NVARCHAR(100) NULL,
    Notes NVARCHAR(300) NULL,
    CreatedByUserId INT NOT NULL,
    CONSTRAINT FK_SupplierPayments_Suppliers FOREIGN KEY (SupplierId) REFERENCES dbo.Suppliers(Id) ON DELETE CASCADE,
    CONSTRAINT FK_SupplierPayments_Users FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id)
);
GO

-- =============================================
-- 5. PRODUCTS & INVENTORY
-- =============================================

CREATE TABLE dbo.Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CatName NVARCHAR(100) NULL
);
GO

CREATE TABLE dbo.Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PID NVARCHAR(50) NOT NULL,
    PName NVARCHAR(100) NOT NULL,
    HSN NVARCHAR(20) NULL,
    SPrice DECIMAL(18, 2) NOT NULL DEFAULT 0,
    MRP DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Gst DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Barcode NVARCHAR(50) NULL,
    Unit NVARCHAR(20) NULL DEFAULT 'PCS',
    ExpiryDate DATETIME2 NULL,
    BatchNo NVARCHAR(50) NULL,
    CurrentStock FLOAT NOT NULL DEFAULT 0,
    CategoryId INT NULL,
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id)
);
GO

CREATE TABLE dbo.StockLedgers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Date DATETIME2 NOT NULL DEFAULT GETDATE(),
    BatchNo NVARCHAR(50) NULL,
    PurNo NVARCHAR(50) NULL,
    QtyAdded FLOAT NOT NULL,
    PPrice DECIMAL(18, 2) NOT NULL,
    ProductId INT NOT NULL,
    SupplierId INT NULL,
    CONSTRAINT FK_StockLedgers_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id) ON DELETE CASCADE,
    CONSTRAINT FK_StockLedgers_Suppliers FOREIGN KEY (SupplierId) REFERENCES dbo.Suppliers(Id)
);
GO

CREATE TABLE dbo.StockReturns (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PID NVARCHAR(50) NULL,
    PName NVARCHAR(100) NULL,
    Scat NVARCHAR(100) NULL,
    HSN NVARCHAR(20) NULL,
    Qty FLOAT NOT NULL DEFAULT 0,
    TQty FLOAT NOT NULL DEFAULT 0,
    CQty FLOAT NOT NULL DEFAULT 0,
    PPrice DECIMAL(18, 2) NOT NULL DEFAULT 0,
    SPrice DECIMAL(18, 2) NOT NULL DEFAULT 0,
    MRP DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Gst FLOAT NOT NULL DEFAULT 0,
    Size NVARCHAR(50) NULL,
    Type NVARCHAR(50) NULL,
    VID NVARCHAR(50) NULL,
    VName NVARCHAR(100) NULL,
    PurNo NVARCHAR(50) NULL,
    Date DATETIME2 NULL DEFAULT GETDATE(),
    Remarks NVARCHAR(300) NULL,
    BatchNo NVARCHAR(50) NULL
);
GO

CREATE TABLE dbo.ExpiryAlerts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    AlertDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ExpiryDate DATETIME2 NOT NULL,
    DaysToExpiry INT NOT NULL,
    Quantity FLOAT NOT NULL,
    IsAcknowledged BIT NOT NULL DEFAULT 0,
    AcknowledgedByUserId INT NULL,
    AcknowledgedDate DATETIME2 NULL,
    CONSTRAINT FK_ExpiryAlerts_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ExpiryAlerts_Users FOREIGN KEY (AcknowledgedByUserId) REFERENCES dbo.Users(Id)
);
GO

-- =============================================
-- 6. PURCHASE MANAGEMENT
-- =============================================

CREATE TABLE dbo.PurchaseEntries (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PurchaseNo NVARCHAR(50) NOT NULL,
    PurchaseDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    SupplierId INT NOT NULL,
    SubTotal DECIMAL(18, 2) NOT NULL DEFAULT 0,
    DiscountAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    GSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    CGSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    SGSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    IGSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    TotalAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    PaidAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    BalanceAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    PaymentMode NVARCHAR(50) NOT NULL DEFAULT 'Cash',
    PaymentStatus NVARCHAR(50) NOT NULL DEFAULT 'Paid',
    TransactionReference NVARCHAR(100) NULL,
    Notes NVARCHAR(200) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedByUserId INT NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastUpdated DATETIME2 NULL,
    CONSTRAINT FK_PurchaseEntries_Suppliers FOREIGN KEY (SupplierId) REFERENCES dbo.Suppliers(Id),
    CONSTRAINT FK_PurchaseEntries_Users FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id)
);
GO

CREATE TABLE dbo.PurchaseItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PurchaseEntryId INT NOT NULL,
    ProductId INT NOT NULL,
    ProductName NVARCHAR(100) NOT NULL,
    ProductCode NVARCHAR(50) NULL,
    Quantity DECIMAL(10, 2) NOT NULL,
    Unit NVARCHAR(20) NOT NULL DEFAULT 'PCS',
    Rate DECIMAL(18, 2) NOT NULL,
    DiscountPercentage DECIMAL(8, 2) NOT NULL DEFAULT 0,
    DiscountAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    GST DECIMAL(8, 2) NOT NULL DEFAULT 0,
    GSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    CGSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    SGSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    IGSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Total DECIMAL(18, 2) NOT NULL,
    HSNCode NVARCHAR(20) NULL,
    BatchNo NVARCHAR(50) NULL,
    ExpiryDate DATETIME2 NULL,
    CONSTRAINT FK_PurchaseItems_PurchaseEntries FOREIGN KEY (PurchaseEntryId) REFERENCES dbo.PurchaseEntries(Id) ON DELETE CASCADE,
    CONSTRAINT FK_PurchaseItems_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
);
GO

CREATE TABLE dbo.PurchaseReturns (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ReturnNo NVARCHAR(50) NOT NULL,
    ReturnDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    PurchaseEntryId INT NOT NULL,
    SupplierId INT NOT NULL,
    SubTotal DECIMAL(18, 2) NOT NULL DEFAULT 0,
    GSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    TotalAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    RefundAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    RefundMode NVARCHAR(50) NOT NULL DEFAULT 'Cash',
    Notes NVARCHAR(200) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedByUserId INT NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_PurchaseReturns_PurchaseEntries FOREIGN KEY (PurchaseEntryId) REFERENCES dbo.PurchaseEntries(Id),
    CONSTRAINT FK_PurchaseReturns_Suppliers FOREIGN KEY (SupplierId) REFERENCES dbo.Suppliers(Id),
    CONSTRAINT FK_PurchaseReturns_Users FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id)
);
GO

CREATE TABLE dbo.PurchaseReturnItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PurchaseReturnId INT NOT NULL,
    PurchaseItemId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity DECIMAL(10, 2) NOT NULL,
    Rate DECIMAL(18, 2) NOT NULL,
    GST DECIMAL(8, 2) NOT NULL DEFAULT 0,
    GSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Total DECIMAL(18, 2) NOT NULL,
    Reason NVARCHAR(200) NULL,
    CONSTRAINT FK_PurchaseReturnItems_PurchaseReturns FOREIGN KEY (PurchaseReturnId) REFERENCES dbo.PurchaseReturns(Id) ON DELETE CASCADE,
    CONSTRAINT FK_PurchaseReturnItems_PurchaseItems FOREIGN KEY (PurchaseItemId) REFERENCES dbo.PurchaseItems(Id),
    CONSTRAINT FK_PurchaseReturnItems_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
);
GO

-- =============================================
-- 7. BILLING & SALES
-- =============================================

CREATE TABLE dbo.Bills (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BillNo NVARCHAR(50) NOT NULL UNIQUE,
    BDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CustomerId INT NULL,
    CName NVARCHAR(100) NULL,
    CContact NVARCHAR(15) NULL,
    CustomerAddress NVARCHAR(300) NULL,
    SalesPerson NVARCHAR(100) NULL,
    PaymentMode NVARCHAR(50) NOT NULL DEFAULT 'Cash',
    UPIReference NVARCHAR(50) NULL,
    PaymentDetails NVARCHAR(200) NULL,
    PaymentReference NVARCHAR(100) NULL,
    PaymentStatus NVARCHAR(50) NOT NULL DEFAULT 'Paid',
    SubTotal DECIMAL(18, 2) NOT NULL DEFAULT 0,
    DiscountAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    DiscountPercentage DECIMAL(8, 2) NOT NULL DEFAULT 0,
    GstAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    CGSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    SGSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    IGSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    IsInterState BIT NOT NULL DEFAULT 0,
    TotalAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    PaidAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    BalanceAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Notes NVARCHAR(200) NULL,
    IsPrinted BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedByUserId INT NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastUpdated DATETIME2 NULL,
    CONSTRAINT FK_Bills_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(Id),
    CONSTRAINT FK_Bills_Users FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id)
);
GO

CREATE TABLE dbo.BillItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BillId INT NOT NULL,
    ProductId INT NOT NULL,
    ProductName NVARCHAR(100) NOT NULL,
    ProductCode NVARCHAR(50) NULL,
    HSNCode NVARCHAR(20) NULL,
    Quantity DECIMAL(10, 2) NOT NULL,
    Rate DECIMAL(18, 2) NOT NULL,
    MRP DECIMAL(18, 2) NOT NULL DEFAULT 0,
    DiscountPercentage DECIMAL(8, 2) NOT NULL DEFAULT 0,
    DiscountAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    PurchasePrice DECIMAL(18, 2) NOT NULL DEFAULT 0,
    GST DECIMAL(8, 2) NOT NULL DEFAULT 0,
    GSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    CGSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    SGSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    IGSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Total DECIMAL(18, 2) NOT NULL,
    Unit NVARCHAR(20) NOT NULL DEFAULT 'PCS',
    BatchNo NVARCHAR(50) NULL,
    ExpiryDate DATETIME2 NULL,
    CONSTRAINT FK_BillItems_Bills FOREIGN KEY (BillId) REFERENCES dbo.Bills(Id) ON DELETE CASCADE,
    CONSTRAINT FK_BillItems_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
);
GO

CREATE TABLE dbo.BillReturns (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ReturnNo NVARCHAR(50) NOT NULL,
    ReturnDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    BillId INT NOT NULL,
    CustomerId INT NULL,
    SubTotal DECIMAL(18, 2) NOT NULL DEFAULT 0,
    GSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    TotalAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    RefundAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    RefundMode NVARCHAR(50) NOT NULL DEFAULT 'Cash',
    Notes NVARCHAR(200) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedByUserId INT NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_BillReturns_Bills FOREIGN KEY (BillId) REFERENCES dbo.Bills(Id),
    CONSTRAINT FK_BillReturns_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(Id),
    CONSTRAINT FK_BillReturns_Users FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(Id)
);
GO

CREATE TABLE dbo.BillReturnItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BillReturnId INT NOT NULL,
    BillItemId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity DECIMAL(10, 2) NOT NULL,
    Rate DECIMAL(18, 2) NOT NULL,
    GST DECIMAL(8, 2) NOT NULL DEFAULT 0,
    GSTAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Total DECIMAL(18, 2) NOT NULL,
    Reason NVARCHAR(200) NULL,
    CONSTRAINT FK_BillReturnItems_BillReturns FOREIGN KEY (BillReturnId) REFERENCES dbo.BillReturns(Id) ON DELETE CASCADE,
    CONSTRAINT FK_BillReturnItems_BillItems FOREIGN KEY (BillItemId) REFERENCES dbo.BillItems(Id),
    CONSTRAINT FK_BillReturnItems_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
);
GO

-- =============================================
-- 8. CASH MANAGEMENT
-- =============================================

CREATE TABLE dbo.CashEntries (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CEID NVARCHAR(50) NULL,
    CEName NVARCHAR(100) NOT NULL,
    CEType NVARCHAR(50) NOT NULL,
    Date DATETIME2 NOT NULL DEFAULT GETDATE(),
    Amount DECIMAL(18, 2) NOT NULL,
    Remark NVARCHAR(300) NULL
);
GO

-- =============================================
-- 9. STAFF MANAGEMENT
-- =============================================

CREATE TABLE dbo.Staff (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StaffID NVARCHAR(50) NULL,
    Name NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15) NULL,
    Email NVARCHAR(100) NULL,
    Address NVARCHAR(300) NULL,
    City NVARCHAR(50) NULL,
    State NVARCHAR(50) NULL,
    PinCode NVARCHAR(10) NULL,
    DateOfBirth DATETIME2 NULL,
    Gender NVARCHAR(10) NULL,
    Position NVARCHAR(100) NULL,
    Department NVARCHAR(100) NULL,
    JoiningDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    BasicSalary DECIMAL(18, 2) NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    EmergencyContact NVARCHAR(15) NULL,
    EmergencyContactName NVARCHAR(100) NULL,
    AadharNumber NVARCHAR(12) NULL,
    PANNumber NVARCHAR(10) NULL,
    BankAccountNumber NVARCHAR(20) NULL,
    BankName NVARCHAR(100) NULL,
    BankIFSC NVARCHAR(11) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastUpdated DATETIME2 NULL,
    Notes NVARCHAR(200) NULL
);
GO

CREATE TABLE dbo.StaffAttendances (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StaffId INT NOT NULL,
    AttendanceDate DATE NOT NULL,
    CheckInTime DATETIME2 NULL,
    CheckOutTime DATETIME2 NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Present',
    WorkHours DECIMAL(5, 2) NOT NULL DEFAULT 0,
    OvertimeHours DECIMAL(5, 2) NOT NULL DEFAULT 0,
    Notes NVARCHAR(200) NULL,
    RecordedByUserId INT NULL,
    CONSTRAINT FK_StaffAttendances_Staff FOREIGN KEY (StaffId) REFERENCES dbo.Staff(Id) ON DELETE CASCADE,
    CONSTRAINT FK_StaffAttendances_Users FOREIGN KEY (RecordedByUserId) REFERENCES dbo.Users(Id)
);
GO

CREATE TABLE dbo.StaffSalaries (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StaffId INT NOT NULL,
    SalaryMonth NVARCHAR(7) NOT NULL,
    BasicSalary DECIMAL(18, 2) NOT NULL,
    Allowances DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Deductions DECIMAL(18, 2) NOT NULL DEFAULT 0,
    OvertimePay DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Bonus DECIMAL(18, 2) NOT NULL DEFAULT 0,
    NetSalary DECIMAL(18, 2) NOT NULL,
    PaymentDate DATETIME2 NULL,
    PaymentMode NVARCHAR(50) NULL DEFAULT 'Cash',
    TransactionReference NVARCHAR(100) NULL,
    IsPaid BIT NOT NULL DEFAULT 0,
    WorkingDays INT NOT NULL DEFAULT 0,
    PresentDays INT NOT NULL DEFAULT 0,
    AbsentDays INT NOT NULL DEFAULT 0,
    LeaveDays INT NOT NULL DEFAULT 0,
    Notes NVARCHAR(200) NULL,
    ProcessedByUserId INT NULL,
    ProcessedDate DATETIME2 NULL,
    CONSTRAINT FK_StaffSalaries_Staff FOREIGN KEY (StaffId) REFERENCES dbo.Staff(Id) ON DELETE CASCADE,
    CONSTRAINT FK_StaffSalaries_Users FOREIGN KEY (ProcessedByUserId) REFERENCES dbo.Users(Id)
);
GO

-- =============================================
-- 10. NOTIFICATIONS & SETTINGS
-- =============================================

CREATE TABLE dbo.Notifications (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NULL,
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    Type NVARCHAR(50) NOT NULL DEFAULT 'Info',
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ReadDate DATETIME2 NULL,
    ActionLink NVARCHAR(200) NULL,
    CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
);
GO

CREATE TABLE dbo.ShopSettings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ShopName NVARCHAR(200) NOT NULL,
    Address NVARCHAR(300) NULL,
    City NVARCHAR(50) NULL,
    State NVARCHAR(50) NULL,
    PinCode NVARCHAR(10) NULL,
    Phone NVARCHAR(15) NULL,
    Email NVARCHAR(100) NULL,
    GSTNumber NVARCHAR(15) NULL,
    LogoPath NVARCHAR(500) NULL,
    InvoicePrefix NVARCHAR(10) NULL DEFAULT 'INV',
    ReceiptPrefix NVARCHAR(10) NULL DEFAULT 'REC',
    PurchasePrefix NVARCHAR(10) NULL DEFAULT 'PUR',
    TaxRate DECIMAL(5, 2) NOT NULL DEFAULT 0,
    CurrencySymbol NVARCHAR(5) NULL DEFAULT '₹',
    FinancialYearStart INT NOT NULL DEFAULT 4,
    LowStockThreshold INT NOT NULL DEFAULT 10,
    EnableSMS BIT NOT NULL DEFAULT 0,
    EnableWhatsApp BIT NOT NULL DEFAULT 0,
    SMSAPIKey NVARCHAR(200) NULL,
    WhatsAppAPIKey NVARCHAR(200) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastUpdated DATETIME2 NULL
);
GO

PRINT 'All tables created successfully.'
GO

-- =============================================
-- SECTION 3: CREATE INDEXES FOR PERFORMANCE
-- =============================================
PRINT 'Creating indexes...'

-- User Indexes
CREATE INDEX IX_Users_Username ON dbo.Users(Username);
CREATE INDEX IX_Users_Email ON dbo.Users(Email);
CREATE INDEX IX_Users_RoleId ON dbo.Users(RoleId);
CREATE INDEX IX_Users_IsActive ON dbo.Users(IsActive);

-- Customer Indexes
CREATE INDEX IX_Customers_Name ON dbo.Customers(Name);
CREATE INDEX IX_Customers_Phone ON dbo.Customers(Phone);
CREATE INDEX IX_Customers_GSTNumber ON dbo.Customers(GSTNumber);
CREATE INDEX IX_Customers_IsActive ON dbo.Customers(IsActive);
CREATE INDEX IX_Customers_CreatedDate ON dbo.Customers(CreatedDate);

-- Supplier Indexes
CREATE INDEX IX_Suppliers_Name ON dbo.Suppliers(Name);
CREATE INDEX IX_Suppliers_GSTNumber ON dbo.Suppliers(GSTNumber);
CREATE INDEX IX_Suppliers_IsActive ON dbo.Suppliers(IsActive);

-- Product Indexes
CREATE INDEX IX_Products_PID ON dbo.Products(PID);
CREATE INDEX IX_Products_PName ON dbo.Products(PName);
CREATE INDEX IX_Products_Barcode ON dbo.Products(Barcode);
CREATE INDEX IX_Products_CategoryId ON dbo.Products(CategoryId);
CREATE INDEX IX_Products_CurrentStock ON dbo.Products(CurrentStock);

-- Bill Indexes
CREATE INDEX IX_Bills_BillNo ON dbo.Bills(BillNo);
CREATE INDEX IX_Bills_BDate ON dbo.Bills(BDate);
CREATE INDEX IX_Bills_CustomerId ON dbo.Bills(CustomerId);
CREATE INDEX IX_Bills_CreatedByUserId ON dbo.Bills(CreatedByUserId);
CREATE INDEX IX_Bills_PaymentMode ON dbo.Bills(PaymentMode);
CREATE INDEX IX_Bills_PaymentStatus ON dbo.Bills(PaymentStatus);
CREATE INDEX IX_Bills_IsActive ON dbo.Bills(IsActive);

-- BillItem Indexes
CREATE INDEX IX_BillItems_BillId ON dbo.BillItems(BillId);
CREATE INDEX IX_BillItems_ProductId ON dbo.BillItems(ProductId);

-- Purchase Indexes
CREATE INDEX IX_PurchaseEntries_PurchaseNo ON dbo.PurchaseEntries(PurchaseNo);
CREATE INDEX IX_PurchaseEntries_PurchaseDate ON dbo.PurchaseEntries(PurchaseDate);
CREATE INDEX IX_PurchaseEntries_SupplierId ON dbo.PurchaseEntries(SupplierId);
CREATE INDEX IX_PurchaseEntries_IsActive ON dbo.PurchaseEntries(IsActive);

-- Stock Ledger Indexes
CREATE INDEX IX_StockLedgers_ProductId ON dbo.StockLedgers(ProductId);
CREATE INDEX IX_StockLedgers_Date ON dbo.StockLedgers(Date);
CREATE INDEX IX_StockLedgers_SupplierId ON dbo.StockLedgers(SupplierId);

-- Staff Indexes
CREATE INDEX IX_Staff_StaffID ON dbo.Staff(StaffID);
CREATE INDEX IX_Staff_Name ON dbo.Staff(Name);
CREATE INDEX IX_Staff_IsActive ON dbo.Staff(IsActive);

-- Attendance Indexes
CREATE INDEX IX_StaffAttendances_StaffId ON dbo.StaffAttendances(StaffId);
CREATE INDEX IX_StaffAttendances_AttendanceDate ON dbo.StaffAttendances(AttendanceDate);

-- Salary Indexes
CREATE INDEX IX_StaffSalaries_StaffId ON dbo.StaffSalaries(StaffId);
CREATE INDEX IX_StaffSalaries_SalaryMonth ON dbo.StaffSalaries(SalaryMonth);
CREATE INDEX IX_StaffSalaries_IsPaid ON dbo.StaffSalaries(IsPaid);

-- Payment Indexes
CREATE INDEX IX_CustomerPayments_CustomerId ON dbo.CustomerPayments(CustomerId);
CREATE INDEX IX_CustomerPayments_PaymentDate ON dbo.CustomerPayments(PaymentDate);
CREATE INDEX IX_SupplierPayments_SupplierId ON dbo.SupplierPayments(SupplierId);
CREATE INDEX IX_SupplierPayments_PaymentDate ON dbo.SupplierPayments(PaymentDate);

-- Cash Entry Indexes
CREATE INDEX IX_CashEntries_Date ON dbo.CashEntries(Date);
CREATE INDEX IX_CashEntries_CEType ON dbo.CashEntries(CEType);

-- Notification Indexes
CREATE INDEX IX_Notifications_UserId ON dbo.Notifications(UserId);
CREATE INDEX IX_Notifications_IsRead ON dbo.Notifications(IsRead);
CREATE INDEX IX_Notifications_CreatedDate ON dbo.Notifications(CreatedDate);

PRINT 'Indexes created successfully.'
GO

-- =============================================
-- SECTION 4: INSERT SEED DATA
-- =============================================
PRINT 'Inserting seed data...'

-- Insert Default Roles
INSERT INTO dbo.UserRoles (RoleName, Description, CreatedDate) VALUES
('SuperAdmin', 'Full system access with all permissions', GETDATE()),
('Admin', 'Administrative access with most permissions', GETDATE()),
('Manager', 'Manager level access', GETDATE()),
('Staff', 'Limited staff access', GETDATE()),
('Cashier', 'Cashier/Billing access only', GETDATE());
GO

-- Insert Default Admin User (Password: Admin@123)
-- Note: In production, use proper password hashing
INSERT INTO dbo.Users (Username, Email, PasswordHash, PasswordSalt, RoleId, IsActive, CreatedDate) VALUES
('admin', 'admin@modernbilling.com',
'AQAAAAIAAYagAAAAEJxK8qF0V7qVJ8K8K8qF0V7qVJ8K8K8qF0V7qVJ8K8K8qF0V7qVJ8K8K8qF0V7qV',
'SaltValue123',
1,
1,
GETDATE());
GO

-- Insert Default Categories
INSERT INTO dbo.Categories (CatName) VALUES
('Electronics'),
('Groceries'),
('Stationery'),
('Clothing'),
('Cosmetics'),
('Hardware'),
('Medicines'),
('Food Items'),
('Beverages'),
('Others');
GO

-- Insert Default Shop Settings
INSERT INTO dbo.ShopSettings (
    ShopName, Address, City, State, PinCode, Phone, Email,
    InvoicePrefix, ReceiptPrefix, PurchasePrefix,
    TaxRate, CurrencySymbol, FinancialYearStart, LowStockThreshold,
    EnableSMS, EnableWhatsApp, CreatedDate
) VALUES (
    'Modern Billing Store',
    '123 Main Street',
    'Mumbai',
    'Maharashtra',
    '400001',
    '+91 9876543210',
    'info@modernbilling.com',
    'INV',
    'REC',
    'PUR',
    18.00,
    '₹',
    4,
    10,
    0,
    0,
    GETDATE()
);
GO

-- Insert Sample Permissions for SuperAdmin
INSERT INTO dbo.RolePermissions (RoleId, PermissionKey, CanView, CanCreate, CanEdit, CanDelete) VALUES
(1, 'Dashboard', 1, 1, 1, 1),
(1, 'Billing', 1, 1, 1, 1),
(1, 'Customers', 1, 1, 1, 1),
(1, 'Products', 1, 1, 1, 1),
(1, 'Inventory', 1, 1, 1, 1),
(1, 'Purchase', 1, 1, 1, 1),
(1, 'Suppliers', 1, 1, 1, 1),
(1, 'Reports', 1, 1, 1, 1),
(1, 'Staff', 1, 1, 1, 1),
(1, 'Expenses', 1, 1, 1, 1),
(1, 'Settings', 1, 1, 1, 1),
(1, 'Users', 1, 1, 1, 1);
GO

-- Insert Sample Permissions for Admin
INSERT INTO dbo.RolePermissions (RoleId, PermissionKey, CanView, CanCreate, CanEdit, CanDelete) VALUES
(2, 'Dashboard', 1, 1, 1, 1),
(2, 'Billing', 1, 1, 1, 1),
(2, 'Customers', 1, 1, 1, 1),
(2, 'Products', 1, 1, 1, 1),
(2, 'Inventory', 1, 1, 1, 1),
(2, 'Purchase', 1, 1, 1, 1),
(2, 'Suppliers', 1, 1, 1, 1),
(2, 'Reports', 1, 1, 1, 1),
(2, 'Staff', 1, 1, 1, 0),
(2, 'Expenses', 1, 1, 1, 1),
(2, 'Settings', 1, 0, 1, 0),
(2, 'Users', 1, 1, 1, 0);
GO

-- Insert Sample Permissions for Cashier
INSERT INTO dbo.RolePermissions (RoleId, PermissionKey, CanView, CanCreate, CanEdit, CanDelete) VALUES
(5, 'Dashboard', 1, 0, 0, 0),
(5, 'Billing', 1, 1, 1, 0),
(5, 'Customers', 1, 1, 0, 0),
(5, 'Products', 1, 0, 0, 0),
(5, 'Reports', 1, 0, 0, 0);
GO

PRINT 'Seed data inserted successfully.'
GO

-- =============================================
-- SECTION 5: CREATE VIEWS FOR REPORTING
-- =============================================
PRINT 'Creating views...'

-- Sales Summary View
CREATE VIEW vw_SalesSummary AS
SELECT
    b.Id AS BillId,
    b.BillNo,
    b.BDate,
    b.CustomerId,
    c.Name AS CustomerName,
    c.Phone AS CustomerPhone,
    b.SubTotal,
    b.DiscountAmount,
    b.GstAmount,
    b.TotalAmount,
    b.PaidAmount,
    b.BalanceAmount,
    b.PaymentMode,
    b.PaymentStatus,
    u.Username AS CreatedBy,
    COUNT(bi.Id) AS TotalItems,
    SUM(bi.Quantity) AS TotalQuantity
FROM dbo.Bills b
LEFT JOIN dbo.Customers c ON b.CustomerId = c.Id
LEFT JOIN dbo.BillItems bi ON b.Id = bi.BillId
LEFT JOIN dbo.Users u ON b.CreatedByUserId = u.Id
WHERE b.IsActive = 1
GROUP BY b.Id, b.BillNo, b.BDate, b.CustomerId, c.Name, c.Phone,
         b.SubTotal, b.DiscountAmount, b.GstAmount, b.TotalAmount,
         b.PaidAmount, b.BalanceAmount, b.PaymentMode, b.PaymentStatus,
         u.Username;
GO

-- Stock Summary View
CREATE VIEW vw_StockSummary AS
SELECT
    p.Id AS ProductId,
    p.PID,
    p.PName,
    p.Barcode,
    c.CatName AS Category,
    p.CurrentStock,
    p.SPrice,
    p.MRP,
    p.Gst,
    (p.CurrentStock * p.SPrice) AS StockValue,
    p.ExpiryDate,
    CASE
        WHEN p.CurrentStock <= 10 THEN 'Low Stock'
        WHEN p.CurrentStock <= 0 THEN 'Out of Stock'
        ELSE 'In Stock'
    END AS StockStatus
FROM dbo.Products p
LEFT JOIN dbo.Categories c ON p.CategoryId = c.Id;
GO

-- Customer Outstanding View
CREATE VIEW vw_CustomerOutstanding AS
SELECT
    c.Id AS CustomerId,
    c.Name,
    c.Phone,
    c.Email,
    c.OutstandingBalance,
    c.CreditLimit,
    COUNT(DISTINCT b.Id) AS TotalBills,
    SUM(b.TotalAmount) AS TotalPurchase,
    SUM(b.PaidAmount) AS TotalPaid,
    SUM(b.BalanceAmount) AS PendingAmount,
    MAX(b.BDate) AS LastPurchaseDate
FROM dbo.Customers c
LEFT JOIN dbo.Bills b ON c.Id = b.CustomerId AND b.IsActive = 1
GROUP BY c.Id, c.Name, c.Phone, c.Email, c.OutstandingBalance, c.CreditLimit;
GO

-- Supplier Outstanding View
CREATE VIEW vw_SupplierOutstanding AS
SELECT
    s.Id AS SupplierId,
    s.Name,
    s.Phone,
    s.Email,
    s.OutstandingBalance,
    COUNT(DISTINCT pe.Id) AS TotalPurchases,
    SUM(pe.TotalAmount) AS TotalPurchaseAmount,
    SUM(pe.PaidAmount) AS TotalPaid,
    SUM(pe.BalanceAmount) AS PendingAmount,
    MAX(pe.PurchaseDate) AS LastPurchaseDate
FROM dbo.Suppliers s
LEFT JOIN dbo.PurchaseEntries pe ON s.Id = pe.SupplierId AND pe.IsActive = 1
GROUP BY s.Id, s.Name, s.Phone, s.Email, s.OutstandingBalance;
GO

-- Daily Sales Report View
CREATE VIEW vw_DailySalesReport AS
SELECT
    CAST(b.BDate AS DATE) AS SaleDate,
    COUNT(DISTINCT b.Id) AS TotalBills,
    SUM(b.SubTotal) AS TotalSubTotal,
    SUM(b.DiscountAmount) AS TotalDiscount,
    SUM(b.GstAmount) AS TotalGST,
    SUM(b.TotalAmount) AS TotalSales,
    SUM(b.PaidAmount) AS TotalCashReceived,
    SUM(CASE WHEN b.PaymentMode = 'Cash' THEN b.PaidAmount ELSE 0 END) AS CashSales,
    SUM(CASE WHEN b.PaymentMode = 'Card' THEN b.PaidAmount ELSE 0 END) AS CardSales,
    SUM(CASE WHEN b.PaymentMode = 'UPI' THEN b.PaidAmount ELSE 0 END) AS UPISales,
    SUM(CASE WHEN b.PaymentMode = 'Credit' THEN b.TotalAmount ELSE 0 END) AS CreditSales
FROM dbo.Bills b
WHERE b.IsActive = 1
GROUP BY CAST(b.BDate AS DATE);
GO

PRINT 'Views created successfully.'
GO

-- =============================================
-- SECTION 6: CREATE STORED PROCEDURES
-- =============================================
PRINT 'Creating stored procedures...'

-- Procedure to Get Low Stock Products
CREATE PROCEDURE sp_GetLowStockProducts
    @Threshold INT = 10
AS
BEGIN
    SELECT
        p.Id,
        p.PID,
        p.PName,
        p.CurrentStock,
        p.SPrice,
        p.MRP,
        c.CatName AS Category,
        (p.CurrentStock * p.SPrice) AS StockValue
    FROM dbo.Products p
    LEFT JOIN dbo.Categories c ON p.CategoryId = c.Id
    WHERE p.CurrentStock <= @Threshold
    ORDER BY p.CurrentStock ASC;
END;
GO

-- Procedure to Get Sales Report by Date Range
CREATE PROCEDURE sp_GetSalesReport
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SELECT
        CAST(b.BDate AS DATE) AS SaleDate,
        COUNT(b.Id) AS TotalBills,
        SUM(b.SubTotal) AS SubTotal,
        SUM(b.DiscountAmount) AS TotalDiscount,
        SUM(b.GstAmount) AS TotalGST,
        SUM(b.TotalAmount) AS TotalSales,
        SUM(b.PaidAmount) AS CashReceived,
        SUM(bi.Quantity) AS TotalItemsSold
    FROM dbo.Bills b
    LEFT JOIN dbo.BillItems bi ON b.Id = bi.BillId
    WHERE b.IsActive = 1
        AND CAST(b.BDate AS DATE) BETWEEN @StartDate AND @EndDate
    GROUP BY CAST(b.BDate AS DATE)
    ORDER BY SaleDate DESC;
END;
GO

-- Procedure to Update Stock After Sale
CREATE PROCEDURE sp_UpdateStockAfterSale
    @ProductId INT,
    @Quantity DECIMAL(10,2)
AS
BEGIN
    UPDATE dbo.Products
    SET CurrentStock = CurrentStock - @Quantity
    WHERE Id = @ProductId;
END;
GO

-- Procedure to Get Customer Purchase History
CREATE PROCEDURE sp_GetCustomerPurchaseHistory
    @CustomerId INT,
    @StartDate DATE = NULL,
    @EndDate DATE = NULL
AS
BEGIN
    SELECT
        b.BillNo,
        b.BDate,
        b.SubTotal,
        b.DiscountAmount,
        b.TotalAmount,
        b.PaidAmount,
        b.BalanceAmount,
        b.PaymentMode,
        b.PaymentStatus,
        COUNT(bi.Id) AS TotalItems
    FROM dbo.Bills b
    LEFT JOIN dbo.BillItems bi ON b.Id = bi.BillId
    WHERE b.CustomerId = @CustomerId
        AND b.IsActive = 1
        AND (@StartDate IS NULL OR CAST(b.BDate AS DATE) >= @StartDate)
        AND (@EndDate IS NULL OR CAST(b.BDate AS DATE) <= @EndDate)
    GROUP BY b.BillNo, b.BDate, b.SubTotal, b.DiscountAmount,
             b.TotalAmount, b.PaidAmount, b.BalanceAmount,
             b.PaymentMode, b.PaymentStatus
    ORDER BY b.BDate DESC;
END;
GO

PRINT 'Stored procedures created successfully.'
GO

-- =============================================
-- SECTION 7: COMPLETION MESSAGE
-- =============================================
PRINT '============================================='
PRINT 'Database Migration Completed Successfully!'
PRINT '============================================='
PRINT ''
PRINT 'Summary:'
PRINT '- All tables created with proper relationships'
PRINT '- Indexes created for optimal performance'
PRINT '- Default seed data inserted'
PRINT '- Views created for reporting'
PRINT '- Stored procedures created'
PRINT ''
PRINT 'Default Login Credentials:'
PRINT 'Username: admin'
PRINT 'Password: Admin@123'
PRINT ''
PRINT 'IMPORTANT: Change the default password immediately!'
PRINT '============================================='
GO
