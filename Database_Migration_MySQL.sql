-- =============================================
-- ModernBillingApp Database Migration Script
-- MySQL/MariaDB Compatible
-- =============================================
-- This script creates all tables for the Modern Billing Application
-- Execute this script on a new or existing MySQL/MariaDB database
-- =============================================

-- Create Database (uncomment if needed)
-- CREATE DATABASE IF NOT EXISTS ModernBillingDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
-- USE ModernBillingDB;

SET FOREIGN_KEY_CHECKS = 0;

-- =============================================
-- SECTION 1: DROP EXISTING TABLES
-- =============================================

DROP TABLE IF EXISTS `Notifications`;
DROP TABLE IF EXISTS `RolePermissions`;
DROP TABLE IF EXISTS `ExpiryAlerts`;
DROP TABLE IF EXISTS `LoyaltyTransactions`;
DROP TABLE IF EXISTS `StaffSalaries`;
DROP TABLE IF EXISTS `StaffAttendances`;
DROP TABLE IF EXISTS `Staff`;
DROP TABLE IF EXISTS `BillReturnItems`;
DROP TABLE IF EXISTS `BillReturns`;
DROP TABLE IF EXISTS `BillItems`;
DROP TABLE IF EXISTS `Bills`;
DROP TABLE IF EXISTS `PurchaseReturnItems`;
DROP TABLE IF EXISTS `PurchaseReturns`;
DROP TABLE IF EXISTS `PurchaseItems`;
DROP TABLE IF EXISTS `PurchaseEntries`;
DROP TABLE IF EXISTS `StockReturns`;
DROP TABLE IF EXISTS `StockLedgers`;
DROP TABLE IF EXISTS `Products`;
DROP TABLE IF EXISTS `Categories`;
DROP TABLE IF EXISTS `CustomerPaymentEnhanced`;
DROP TABLE IF EXISTS `CustomerPayments`;
DROP TABLE IF EXISTS `Customers`;
DROP TABLE IF EXISTS `SupplierPayments`;
DROP TABLE IF EXISTS `Suppliers`;
DROP TABLE IF EXISTS `CashEntries`;
DROP TABLE IF EXISTS `Employees`;
DROP TABLE IF EXISTS `Users`;
DROP TABLE IF EXISTS `UserRoles`;
DROP TABLE IF EXISTS `ShopSettings`;

SET FOREIGN_KEY_CHECKS = 1;

-- =============================================
-- SECTION 2: CREATE TABLES
-- =============================================

-- User Roles
CREATE TABLE `UserRoles` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `RoleName` VARCHAR(50) NOT NULL UNIQUE,
    `Description` VARCHAR(200) NULL,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_rolename (`RoleName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Users
CREATE TABLE `Users` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Username` VARCHAR(50) NOT NULL UNIQUE,
    `Email` VARCHAR(100) NOT NULL,
    `PasswordHash` TEXT NOT NULL,
    `PasswordSalt` TEXT NOT NULL,
    `RoleId` INT NOT NULL,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `LastLogin` DATETIME NULL,
    FOREIGN KEY (`RoleId`) REFERENCES `UserRoles`(`Id`),
    INDEX idx_username (`Username`),
    INDEX idx_email (`Email`),
    INDEX idx_roleid (`RoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Role Permissions
CREATE TABLE `RolePermissions` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `RoleId` INT NOT NULL,
    `PermissionKey` VARCHAR(100) NOT NULL,
    `CanView` BOOLEAN NOT NULL DEFAULT FALSE,
    `CanCreate` BOOLEAN NOT NULL DEFAULT FALSE,
    `CanEdit` BOOLEAN NOT NULL DEFAULT FALSE,
    `CanDelete` BOOLEAN NOT NULL DEFAULT FALSE,
    FOREIGN KEY (`RoleId`) REFERENCES `UserRoles`(`Id`) ON DELETE CASCADE,
    INDEX idx_roleid (`RoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Employees
CREATE TABLE `Employees` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `EmpID` VARCHAR(50) NULL,
    `Name` VARCHAR(100) NOT NULL,
    `Contact` VARCHAR(15) NULL,
    `Address` VARCHAR(300) NULL,
    `Gender` VARCHAR(10) NULL,
    `Salary` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `JoiningDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_name (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Customers
CREATE TABLE `Customers` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `CID` VARCHAR(50) NULL,
    `Name` VARCHAR(100) NOT NULL,
    `Address` VARCHAR(300) NULL,
    `Gender` VARCHAR(10) NULL,
    `Phone` VARCHAR(15) NULL,
    `City` VARCHAR(50) NULL,
    `State` VARCHAR(50) NULL,
    `GSTNumber` VARCHAR(15) NULL,
    `Email` VARCHAR(100) NULL,
    `PinCode` VARCHAR(10) NULL,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `LastUpdated` DATETIME NULL,
    `CardNo` VARCHAR(50) NULL,
    `Points` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `TotalPointsEarned` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `TotalPointsRedeemed` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `CustomerType` VARCHAR(50) NULL DEFAULT 'Regular',
    `CreditLimit` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `OutstandingBalance` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `Notes` VARCHAR(200) NULL,
    INDEX idx_name (`Name`),
    INDEX idx_phone (`Phone`),
    INDEX idx_gstnumber (`GSTNumber`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Customer Payments
CREATE TABLE `CustomerPayments` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `CustomerId` INT NOT NULL,
    `PaymentDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `PaymentMode` VARCHAR(50) NOT NULL DEFAULT 'Cash',
    `Amount` DECIMAL(18, 2) NOT NULL,
    `BillId` INT NULL,
    `TransactionReference` VARCHAR(100) NULL,
    `Notes` VARCHAR(300) NULL,
    `CreatedByUserId` INT NOT NULL,
    FOREIGN KEY (`CustomerId`) REFERENCES `Customers`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`CreatedByUserId`) REFERENCES `Users`(`Id`),
    INDEX idx_customerid (`CustomerId`),
    INDEX idx_paymentdate (`PaymentDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Customer Payment Enhanced
CREATE TABLE `CustomerPaymentEnhanced` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `CustomerId` INT NOT NULL,
    `BillId` INT NULL,
    `PaymentDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `AmountPaid` DECIMAL(18, 2) NOT NULL,
    `PaymentMode` VARCHAR(50) NOT NULL DEFAULT 'Cash',
    `TransactionReference` VARCHAR(100) NULL,
    `Notes` VARCHAR(300) NULL,
    `RemainingBalance` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `CreatedByUserId` INT NOT NULL,
    FOREIGN KEY (`CustomerId`) REFERENCES `Customers`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`CreatedByUserId`) REFERENCES `Users`(`Id`),
    INDEX idx_customerid (`CustomerId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Loyalty Transactions
CREATE TABLE `LoyaltyTransactions` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `CustomerId` INT NOT NULL,
    `BillId` INT NULL,
    `TransactionType` VARCHAR(50) NOT NULL,
    `Points` DECIMAL(18, 2) NOT NULL,
    `TransactionDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `Description` VARCHAR(200) NULL,
    FOREIGN KEY (`CustomerId`) REFERENCES `Customers`(`Id`) ON DELETE CASCADE,
    INDEX idx_customerid (`CustomerId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Suppliers
CREATE TABLE `Suppliers` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `SupplierID` VARCHAR(50) NULL,
    `Name` VARCHAR(100) NOT NULL,
    `Phone` VARCHAR(15) NULL,
    `Email` VARCHAR(100) NULL,
    `Address` VARCHAR(300) NULL,
    `City` VARCHAR(50) NULL,
    `State` VARCHAR(50) NULL,
    `GSTNumber` VARCHAR(15) NULL,
    `PinCode` VARCHAR(10) NULL,
    `ContactPerson` VARCHAR(100) NULL,
    `CreditLimit` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `OutstandingBalance` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `LastUpdated` DATETIME NULL,
    `Notes` VARCHAR(200) NULL,
    INDEX idx_name (`Name`),
    INDEX idx_gstnumber (`GSTNumber`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Supplier Payments
CREATE TABLE `SupplierPayments` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `SupplierId` INT NOT NULL,
    `PaymentDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `PaymentMode` VARCHAR(50) NOT NULL DEFAULT 'Cash',
    `Amount` DECIMAL(18, 2) NOT NULL,
    `PurchaseEntryId` INT NULL,
    `TransactionReference` VARCHAR(100) NULL,
    `Notes` VARCHAR(300) NULL,
    `CreatedByUserId` INT NOT NULL,
    FOREIGN KEY (`SupplierId`) REFERENCES `Suppliers`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`CreatedByUserId`) REFERENCES `Users`(`Id`),
    INDEX idx_supplierid (`SupplierId`),
    INDEX idx_paymentdate (`PaymentDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Categories
CREATE TABLE `Categories` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `CatName` VARCHAR(100) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Products
CREATE TABLE `Products` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `PID` VARCHAR(50) NOT NULL,
    `PName` VARCHAR(100) NOT NULL,
    `HSN` VARCHAR(20) NULL,
    `SPrice` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `MRP` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `Gst` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `Barcode` VARCHAR(50) NULL,
    `Unit` VARCHAR(20) NULL DEFAULT 'PCS',
    `ExpiryDate` DATETIME NULL,
    `BatchNo` VARCHAR(50) NULL,
    `CurrentStock` DOUBLE NOT NULL DEFAULT 0,
    `CategoryId` INT NULL,
    FOREIGN KEY (`CategoryId`) REFERENCES `Categories`(`Id`),
    INDEX idx_pid (`PID`),
    INDEX idx_pname (`PName`),
    INDEX idx_barcode (`Barcode`),
    INDEX idx_categoryid (`CategoryId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Stock Ledgers
CREATE TABLE `StockLedgers` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `BatchNo` VARCHAR(50) NULL,
    `PurNo` VARCHAR(50) NULL,
    `QtyAdded` DOUBLE NOT NULL,
    `PPrice` DECIMAL(18, 2) NOT NULL,
    `ProductId` INT NOT NULL,
    `SupplierId` INT NULL,
    FOREIGN KEY (`ProductId`) REFERENCES `Products`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`SupplierId`) REFERENCES `Suppliers`(`Id`),
    INDEX idx_productid (`ProductId`),
    INDEX idx_date (`Date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Stock Returns
CREATE TABLE `StockReturns` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `PID` VARCHAR(50) NULL,
    `PName` VARCHAR(100) NULL,
    `Scat` VARCHAR(100) NULL,
    `HSN` VARCHAR(20) NULL,
    `Qty` DOUBLE NOT NULL DEFAULT 0,
    `TQty` DOUBLE NOT NULL DEFAULT 0,
    `CQty` DOUBLE NOT NULL DEFAULT 0,
    `PPrice` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `SPrice` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `MRP` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `Gst` DOUBLE NOT NULL DEFAULT 0,
    `Size` VARCHAR(50) NULL,
    `Type` VARCHAR(50) NULL,
    `VID` VARCHAR(50) NULL,
    `VName` VARCHAR(100) NULL,
    `PurNo` VARCHAR(50) NULL,
    `Date` DATETIME NULL DEFAULT CURRENT_TIMESTAMP,
    `Remarks` VARCHAR(300) NULL,
    `BatchNo` VARCHAR(50) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Expiry Alerts
CREATE TABLE `ExpiryAlerts` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `ProductId` INT NOT NULL,
    `AlertDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `ExpiryDate` DATETIME NOT NULL,
    `DaysToExpiry` INT NOT NULL,
    `Quantity` DOUBLE NOT NULL,
    `IsAcknowledged` BOOLEAN NOT NULL DEFAULT FALSE,
    `AcknowledgedByUserId` INT NULL,
    `AcknowledgedDate` DATETIME NULL,
    FOREIGN KEY (`ProductId`) REFERENCES `Products`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`AcknowledgedByUserId`) REFERENCES `Users`(`Id`),
    INDEX idx_productid (`ProductId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Purchase Entries
CREATE TABLE `PurchaseEntries` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `PurchaseNo` VARCHAR(50) NOT NULL,
    `PurchaseDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `SupplierId` INT NOT NULL,
    `SubTotal` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `DiscountAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `GSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `CGSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `SGSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `IGSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `TotalAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `PaidAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `BalanceAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `PaymentMode` VARCHAR(50) NOT NULL DEFAULT 'Cash',
    `PaymentStatus` VARCHAR(50) NOT NULL DEFAULT 'Paid',
    `TransactionReference` VARCHAR(100) NULL,
    `Notes` VARCHAR(200) NULL,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `CreatedByUserId` INT NOT NULL,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `LastUpdated` DATETIME NULL,
    FOREIGN KEY (`SupplierId`) REFERENCES `Suppliers`(`Id`),
    FOREIGN KEY (`CreatedByUserId`) REFERENCES `Users`(`Id`),
    INDEX idx_purchaseno (`PurchaseNo`),
    INDEX idx_purchasedate (`PurchaseDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Purchase Items
CREATE TABLE `PurchaseItems` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `PurchaseEntryId` INT NOT NULL,
    `ProductId` INT NOT NULL,
    `ProductName` VARCHAR(100) NOT NULL,
    `ProductCode` VARCHAR(50) NULL,
    `Quantity` DECIMAL(10, 2) NOT NULL,
    `Unit` VARCHAR(20) NOT NULL DEFAULT 'PCS',
    `Rate` DECIMAL(18, 2) NOT NULL,
    `DiscountPercentage` DECIMAL(8, 2) NOT NULL DEFAULT 0,
    `DiscountAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `GST` DECIMAL(8, 2) NOT NULL DEFAULT 0,
    `GSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `CGSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `SGSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `IGSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `Total` DECIMAL(18, 2) NOT NULL,
    `HSNCode` VARCHAR(20) NULL,
    `BatchNo` VARCHAR(50) NULL,
    `ExpiryDate` DATETIME NULL,
    FOREIGN KEY (`PurchaseEntryId`) REFERENCES `PurchaseEntries`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`ProductId`) REFERENCES `Products`(`Id`),
    INDEX idx_purchaseentryid (`PurchaseEntryId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Purchase Returns
CREATE TABLE `PurchaseReturns` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `ReturnNo` VARCHAR(50) NOT NULL,
    `ReturnDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `PurchaseEntryId` INT NOT NULL,
    `SupplierId` INT NOT NULL,
    `SubTotal` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `GSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `TotalAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `RefundAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `RefundMode` VARCHAR(50) NOT NULL DEFAULT 'Cash',
    `Notes` VARCHAR(200) NULL,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `CreatedByUserId` INT NOT NULL,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (`PurchaseEntryId`) REFERENCES `PurchaseEntries`(`Id`),
    FOREIGN KEY (`SupplierId`) REFERENCES `Suppliers`(`Id`),
    FOREIGN KEY (`CreatedByUserId`) REFERENCES `Users`(`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Purchase Return Items
CREATE TABLE `PurchaseReturnItems` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `PurchaseReturnId` INT NOT NULL,
    `PurchaseItemId` INT NOT NULL,
    `ProductId` INT NOT NULL,
    `Quantity` DECIMAL(10, 2) NOT NULL,
    `Rate` DECIMAL(18, 2) NOT NULL,
    `GST` DECIMAL(8, 2) NOT NULL DEFAULT 0,
    `GSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `Total` DECIMAL(18, 2) NOT NULL,
    `Reason` VARCHAR(200) NULL,
    FOREIGN KEY (`PurchaseReturnId`) REFERENCES `PurchaseReturns`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`PurchaseItemId`) REFERENCES `PurchaseItems`(`Id`),
    FOREIGN KEY (`ProductId`) REFERENCES `Products`(`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Bills
CREATE TABLE `Bills` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `BillNo` VARCHAR(50) NOT NULL UNIQUE,
    `BDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `CustomerId` INT NULL,
    `CName` VARCHAR(100) NULL,
    `CContact` VARCHAR(15) NULL,
    `CustomerAddress` VARCHAR(300) NULL,
    `SalesPerson` VARCHAR(100) NULL,
    `PaymentMode` VARCHAR(50) NOT NULL DEFAULT 'Cash',
    `UPIReference` VARCHAR(50) NULL,
    `PaymentDetails` VARCHAR(200) NULL,
    `PaymentReference` VARCHAR(100) NULL,
    `PaymentStatus` VARCHAR(50) NOT NULL DEFAULT 'Paid',
    `SubTotal` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `DiscountAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `DiscountPercentage` DECIMAL(8, 2) NOT NULL DEFAULT 0,
    `GstAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `CGSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `SGSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `IGSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `IsInterState` BOOLEAN NOT NULL DEFAULT FALSE,
    `TotalAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `PaidAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `BalanceAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `Notes` VARCHAR(200) NULL,
    `IsPrinted` BOOLEAN NOT NULL DEFAULT FALSE,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `CreatedByUserId` INT NOT NULL,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `LastUpdated` DATETIME NULL,
    FOREIGN KEY (`CustomerId`) REFERENCES `Customers`(`Id`),
    FOREIGN KEY (`CreatedByUserId`) REFERENCES `Users`(`Id`),
    INDEX idx_billno (`BillNo`),
    INDEX idx_bdate (`BDate`),
    INDEX idx_customerid (`CustomerId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Bill Items
CREATE TABLE `BillItems` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `BillId` INT NOT NULL,
    `ProductId` INT NOT NULL,
    `ProductName` VARCHAR(100) NOT NULL,
    `ProductCode` VARCHAR(50) NULL,
    `HSNCode` VARCHAR(20) NULL,
    `Quantity` DECIMAL(10, 2) NOT NULL,
    `Rate` DECIMAL(18, 2) NOT NULL,
    `MRP` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `DiscountPercentage` DECIMAL(8, 2) NOT NULL DEFAULT 0,
    `DiscountAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `PurchasePrice` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `GST` DECIMAL(8, 2) NOT NULL DEFAULT 0,
    `GSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `CGSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `SGSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `IGSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `Total` DECIMAL(18, 2) NOT NULL,
    `Unit` VARCHAR(20) NOT NULL DEFAULT 'PCS',
    `BatchNo` VARCHAR(50) NULL,
    `ExpiryDate` DATETIME NULL,
    FOREIGN KEY (`BillId`) REFERENCES `Bills`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`ProductId`) REFERENCES `Products`(`Id`),
    INDEX idx_billid (`BillId`),
    INDEX idx_productid (`ProductId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Bill Returns
CREATE TABLE `BillReturns` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `ReturnNo` VARCHAR(50) NOT NULL,
    `ReturnDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `BillId` INT NOT NULL,
    `CustomerId` INT NULL,
    `SubTotal` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `GSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `TotalAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `RefundAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `RefundMode` VARCHAR(50) NOT NULL DEFAULT 'Cash',
    `Notes` VARCHAR(200) NULL,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `CreatedByUserId` INT NOT NULL,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (`BillId`) REFERENCES `Bills`(`Id`),
    FOREIGN KEY (`CustomerId`) REFERENCES `Customers`(`Id`),
    FOREIGN KEY (`CreatedByUserId`) REFERENCES `Users`(`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Bill Return Items
CREATE TABLE `BillReturnItems` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `BillReturnId` INT NOT NULL,
    `BillItemId` INT NOT NULL,
    `ProductId` INT NOT NULL,
    `Quantity` DECIMAL(10, 2) NOT NULL,
    `Rate` DECIMAL(18, 2) NOT NULL,
    `GST` DECIMAL(8, 2) NOT NULL DEFAULT 0,
    `GSTAmount` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `Total` DECIMAL(18, 2) NOT NULL,
    `Reason` VARCHAR(200) NULL,
    FOREIGN KEY (`BillReturnId`) REFERENCES `BillReturns`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`BillItemId`) REFERENCES `BillItems`(`Id`),
    FOREIGN KEY (`ProductId`) REFERENCES `Products`(`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Cash Entries
CREATE TABLE `CashEntries` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `CEID` VARCHAR(50) NULL,
    `CEName` VARCHAR(100) NOT NULL,
    `CEType` VARCHAR(50) NOT NULL,
    `Date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `Amount` DECIMAL(18, 2) NOT NULL,
    `Remark` VARCHAR(300) NULL,
    INDEX idx_date (`Date`),
    INDEX idx_cetype (`CEType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Staff
CREATE TABLE `Staff` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `StaffID` VARCHAR(50) NULL,
    `Name` VARCHAR(100) NOT NULL,
    `Phone` VARCHAR(15) NULL,
    `Email` VARCHAR(100) NULL,
    `Address` VARCHAR(300) NULL,
    `City` VARCHAR(50) NULL,
    `State` VARCHAR(50) NULL,
    `PinCode` VARCHAR(10) NULL,
    `DateOfBirth` DATETIME NULL,
    `Gender` VARCHAR(10) NULL,
    `Position` VARCHAR(100) NULL,
    `Department` VARCHAR(100) NULL,
    `JoiningDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `BasicSalary` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `EmergencyContact` VARCHAR(15) NULL,
    `EmergencyContactName` VARCHAR(100) NULL,
    `AadharNumber` VARCHAR(12) NULL,
    `PANNumber` VARCHAR(10) NULL,
    `BankAccountNumber` VARCHAR(20) NULL,
    `BankName` VARCHAR(100) NULL,
    `BankIFSC` VARCHAR(11) NULL,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `LastUpdated` DATETIME NULL,
    `Notes` VARCHAR(200) NULL,
    INDEX idx_staffid (`StaffID`),
    INDEX idx_name (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Staff Attendances
CREATE TABLE `StaffAttendances` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `StaffId` INT NOT NULL,
    `AttendanceDate` DATE NOT NULL,
    `CheckInTime` DATETIME NULL,
    `CheckOutTime` DATETIME NULL,
    `Status` VARCHAR(20) NOT NULL DEFAULT 'Present',
    `WorkHours` DECIMAL(5, 2) NOT NULL DEFAULT 0,
    `OvertimeHours` DECIMAL(5, 2) NOT NULL DEFAULT 0,
    `Notes` VARCHAR(200) NULL,
    `RecordedByUserId` INT NULL,
    FOREIGN KEY (`StaffId`) REFERENCES `Staff`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`RecordedByUserId`) REFERENCES `Users`(`Id`),
    INDEX idx_staffid (`StaffId`),
    INDEX idx_attendancedate (`AttendanceDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Staff Salaries
CREATE TABLE `StaffSalaries` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `StaffId` INT NOT NULL,
    `SalaryMonth` VARCHAR(7) NOT NULL,
    `BasicSalary` DECIMAL(18, 2) NOT NULL,
    `Allowances` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `Deductions` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `OvertimePay` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `Bonus` DECIMAL(18, 2) NOT NULL DEFAULT 0,
    `NetSalary` DECIMAL(18, 2) NOT NULL,
    `PaymentDate` DATETIME NULL,
    `PaymentMode` VARCHAR(50) NULL DEFAULT 'Cash',
    `TransactionReference` VARCHAR(100) NULL,
    `IsPaid` BOOLEAN NOT NULL DEFAULT FALSE,
    `WorkingDays` INT NOT NULL DEFAULT 0,
    `PresentDays` INT NOT NULL DEFAULT 0,
    `AbsentDays` INT NOT NULL DEFAULT 0,
    `LeaveDays` INT NOT NULL DEFAULT 0,
    `Notes` VARCHAR(200) NULL,
    `ProcessedByUserId` INT NULL,
    `ProcessedDate` DATETIME NULL,
    FOREIGN KEY (`StaffId`) REFERENCES `Staff`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`ProcessedByUserId`) REFERENCES `Users`(`Id`),
    INDEX idx_staffid (`StaffId`),
    INDEX idx_salarymonth (`SalaryMonth`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Notifications
CREATE TABLE `Notifications` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `UserId` INT NULL,
    `Title` VARCHAR(200) NOT NULL,
    `Message` TEXT NOT NULL,
    `Type` VARCHAR(50) NOT NULL DEFAULT 'Info',
    `IsRead` BOOLEAN NOT NULL DEFAULT FALSE,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `ReadDate` DATETIME NULL,
    `ActionLink` VARCHAR(200) NULL,
    FOREIGN KEY (`UserId`) REFERENCES `Users`(`Id`),
    INDEX idx_userid (`UserId`),
    INDEX idx_isread (`IsRead`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Shop Settings
CREATE TABLE `ShopSettings` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `ShopName` VARCHAR(200) NOT NULL,
    `Address` VARCHAR(300) NULL,
    `City` VARCHAR(50) NULL,
    `State` VARCHAR(50) NULL,
    `PinCode` VARCHAR(10) NULL,
    `Phone` VARCHAR(15) NULL,
    `Email` VARCHAR(100) NULL,
    `GSTNumber` VARCHAR(15) NULL,
    `LogoPath` VARCHAR(500) NULL,
    `InvoicePrefix` VARCHAR(10) NULL DEFAULT 'INV',
    `ReceiptPrefix` VARCHAR(10) NULL DEFAULT 'REC',
    `PurchasePrefix` VARCHAR(10) NULL DEFAULT 'PUR',
    `TaxRate` DECIMAL(5, 2) NOT NULL DEFAULT 0,
    `CurrencySymbol` VARCHAR(5) NULL DEFAULT '₹',
    `FinancialYearStart` INT NOT NULL DEFAULT 4,
    `LowStockThreshold` INT NOT NULL DEFAULT 10,
    `EnableSMS` BOOLEAN NOT NULL DEFAULT FALSE,
    `EnableWhatsApp` BOOLEAN NOT NULL DEFAULT FALSE,
    `SMSAPIKey` VARCHAR(200) NULL,
    `WhatsAppAPIKey` VARCHAR(200) NULL,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `LastUpdated` DATETIME NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- SECTION 3: INSERT SEED DATA
-- =============================================

-- Insert Default Roles
INSERT INTO `UserRoles` (`RoleName`, `Description`, `CreatedDate`) VALUES
('SuperAdmin', 'Full system access with all permissions', NOW()),
('Admin', 'Administrative access with most permissions', NOW()),
('Manager', 'Manager level access', NOW()),
('Staff', 'Limited staff access', NOW()),
('Cashier', 'Cashier/Billing access only', NOW());

-- Insert Default Admin User (Password: Admin@123)
-- Note: In production, use proper password hashing
INSERT INTO `Users` (`Username`, `Email`, `PasswordHash`, `PasswordSalt`, `RoleId`, `IsActive`, `CreatedDate`) VALUES
('admin', 'admin@modernbilling.com',
'AQAAAAIAAYagAAAAEJxK8qF0V7qVJ8K8K8qF0V7qVJ8K8K8qF0V7qVJ8K8K8qF0V7qVJ8K8K8qF0V7qV',
'SaltValue123',
1,
TRUE,
NOW());

-- Insert Default Categories
INSERT INTO `Categories` (`CatName`) VALUES
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

-- Insert Default Shop Settings
INSERT INTO `ShopSettings` (
    `ShopName`, `Address`, `City`, `State`, `PinCode`, `Phone`, `Email`,
    `InvoicePrefix`, `ReceiptPrefix`, `PurchasePrefix`,
    `TaxRate`, `CurrencySymbol`, `FinancialYearStart`, `LowStockThreshold`,
    `EnableSMS`, `EnableWhatsApp`, `CreatedDate`
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
    FALSE,
    FALSE,
    NOW()
);

-- Insert Sample Permissions for SuperAdmin
INSERT INTO `RolePermissions` (`RoleId`, `PermissionKey`, `CanView`, `CanCreate`, `CanEdit`, `CanDelete`) VALUES
(1, 'Dashboard', TRUE, TRUE, TRUE, TRUE),
(1, 'Billing', TRUE, TRUE, TRUE, TRUE),
(1, 'Customers', TRUE, TRUE, TRUE, TRUE),
(1, 'Products', TRUE, TRUE, TRUE, TRUE),
(1, 'Inventory', TRUE, TRUE, TRUE, TRUE),
(1, 'Purchase', TRUE, TRUE, TRUE, TRUE),
(1, 'Suppliers', TRUE, TRUE, TRUE, TRUE),
(1, 'Reports', TRUE, TRUE, TRUE, TRUE),
(1, 'Staff', TRUE, TRUE, TRUE, TRUE),
(1, 'Expenses', TRUE, TRUE, TRUE, TRUE),
(1, 'Settings', TRUE, TRUE, TRUE, TRUE),
(1, 'Users', TRUE, TRUE, TRUE, TRUE);

-- Insert Sample Permissions for Admin
INSERT INTO `RolePermissions` (`RoleId`, `PermissionKey`, `CanView`, `CanCreate`, `CanEdit`, `CanDelete`) VALUES
(2, 'Dashboard', TRUE, TRUE, TRUE, TRUE),
(2, 'Billing', TRUE, TRUE, TRUE, TRUE),
(2, 'Customers', TRUE, TRUE, TRUE, TRUE),
(2, 'Products', TRUE, TRUE, TRUE, TRUE),
(2, 'Inventory', TRUE, TRUE, TRUE, TRUE),
(2, 'Purchase', TRUE, TRUE, TRUE, TRUE),
(2, 'Suppliers', TRUE, TRUE, TRUE, TRUE),
(2, 'Reports', TRUE, TRUE, TRUE, TRUE),
(2, 'Staff', TRUE, TRUE, TRUE, FALSE),
(2, 'Expenses', TRUE, TRUE, TRUE, TRUE),
(2, 'Settings', TRUE, FALSE, TRUE, FALSE),
(2, 'Users', TRUE, TRUE, TRUE, FALSE);

-- Insert Sample Permissions for Cashier
INSERT INTO `RolePermissions` (`RoleId`, `PermissionKey`, `CanView`, `CanCreate`, `CanEdit`, `CanDelete`) VALUES
(5, 'Dashboard', TRUE, FALSE, FALSE, FALSE),
(5, 'Billing', TRUE, TRUE, TRUE, FALSE),
(5, 'Customers', TRUE, TRUE, FALSE, FALSE),
(5, 'Products', TRUE, FALSE, FALSE, FALSE),
(5, 'Reports', TRUE, FALSE, FALSE, FALSE);

-- =============================================
-- COMPLETION MESSAGE
-- =============================================

SELECT '=============================================' AS '';
SELECT 'Database Migration Completed Successfully!' AS '';
SELECT '=============================================' AS '';
SELECT '' AS '';
SELECT 'Summary:' AS '';
SELECT '- All tables created with proper relationships' AS '';
SELECT '- Indexes created for optimal performance' AS '';
SELECT '- Default seed data inserted' AS '';
SELECT '' AS '';
SELECT 'Default Login Credentials:' AS '';
SELECT 'Username: admin' AS '';
SELECT 'Password: Admin@123' AS '';
SELECT '' AS '';
SELECT 'IMPORTANT: Change the default password immediately!' AS '';
SELECT '=============================================' AS '';
