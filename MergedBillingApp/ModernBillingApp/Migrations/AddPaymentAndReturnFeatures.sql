-- Migration Script: Add Payment, Return, and Loyalty Features
-- Execute this script to add all new tables and columns

-- ============================================
-- 1. SUPPLIER PAYMENT TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SupplierPayments')
BEGIN
    CREATE TABLE SupplierPayments (
        Id INT PRIMARY KEY IDENTITY(1,1),
        PaymentDate DATETIME2 NOT NULL,
        PaymentMode NVARCHAR(50) NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        ReferenceNo NVARCHAR(100) NULL,
        PurchaseNo NVARCHAR(50) NULL,
        Remarks NVARCHAR(500) NULL,
        SupplierId INT NOT NULL,
        PurchaseEntryId INT NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        CreatedBy NVARCHAR(100) NULL,
        FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_SupplierPayments_SupplierId ON SupplierPayments(SupplierId);
    CREATE INDEX IX_SupplierPayments_PaymentDate ON SupplierPayments(PaymentDate);
END

-- ============================================
-- 2. ENHANCED CUSTOMER PAYMENT TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CustomerPaymentEnhanced')
BEGIN
    CREATE TABLE CustomerPaymentEnhanced (
        Id INT PRIMARY KEY IDENTITY(1,1),
        PaymentDate DATETIME2 NOT NULL,
        PaymentMode NVARCHAR(50) NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        BillNo NVARCHAR(50) NULL,
        BillId INT NULL,
        BillAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        PreviousPaid DECIMAL(18,2) NOT NULL DEFAULT 0,
        CurrentPayment DECIMAL(18,2) NOT NULL DEFAULT 0,
        RemainingBalance DECIMAL(18,2) NOT NULL DEFAULT 0,
        IsFullPayment BIT NOT NULL DEFAULT 0,
        ReferenceNo NVARCHAR(100) NULL,
        Remarks NVARCHAR(500) NULL,
        CustomerId INT NOT NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        CreatedBy NVARCHAR(100) NULL,
        FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE CASCADE,
        FOREIGN KEY (BillId) REFERENCES Bills(Id) ON DELETE NO ACTION
    );
    
    CREATE INDEX IX_CustomerPaymentEnhanced_CustomerId ON CustomerPaymentEnhanced(CustomerId);
    CREATE INDEX IX_CustomerPaymentEnhanced_BillId ON CustomerPaymentEnhanced(BillId);
    CREATE INDEX IX_CustomerPaymentEnhanced_PaymentDate ON CustomerPaymentEnhanced(PaymentDate);
END

-- ============================================
-- 3. PURCHASE ENTRY TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurchaseEntries')
BEGIN
    CREATE TABLE PurchaseEntries (
        Id INT PRIMARY KEY IDENTITY(1,1),
        PurchaseNo NVARCHAR(50) NOT NULL UNIQUE,
        PurchaseDate DATETIME2 NOT NULL,
        SupplierId INT NOT NULL,
        SupplierName NVARCHAR(200) NULL,
        SupplierGSTIN NVARCHAR(50) NULL,
        SubTotal DECIMAL(18,2) NOT NULL DEFAULT 0,
        CGSTAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        SGSTAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        IGSTAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalGST DECIMAL(18,2) NOT NULL DEFAULT 0,
        GrandTotal DECIMAL(18,2) NOT NULL DEFAULT 0,
        PaidAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        BalanceAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        PaymentStatus NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        Remarks NVARCHAR(500) NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        CreatedBy NVARCHAR(100) NULL,
        FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id) ON DELETE NO ACTION
    );
    
    CREATE INDEX IX_PurchaseEntries_SupplierId ON PurchaseEntries(SupplierId);
    CREATE INDEX IX_PurchaseEntries_PurchaseDate ON PurchaseEntries(PurchaseDate);
    CREATE INDEX IX_PurchaseEntries_PurchaseNo ON PurchaseEntries(PurchaseNo);
END

-- ============================================
-- 4. PURCHASE ITEM TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurchaseItems')
BEGIN
    CREATE TABLE PurchaseItems (
        Id INT PRIMARY KEY IDENTITY(1,1),
        PurchaseEntryId INT NOT NULL,
        ProductId INT NOT NULL,
        ProductCode NVARCHAR(50) NULL,
        ProductName NVARCHAR(200) NULL,
        HSNCode NVARCHAR(10) NULL,
        Quantity FLOAT NOT NULL,
        Unit NVARCHAR(20) NULL DEFAULT 'PCS',
        PurchasePrice DECIMAL(18,2) NOT NULL,
        GSTRate DECIMAL(18,2) NOT NULL DEFAULT 0,
        GSTAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        Total DECIMAL(18,2) NOT NULL,
        BatchNo NVARCHAR(50) NULL,
        ExpiryDate DATETIME2 NULL,
        FOREIGN KEY (PurchaseEntryId) REFERENCES PurchaseEntries(Id) ON DELETE CASCADE,
        FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE NO ACTION
    );
    
    CREATE INDEX IX_PurchaseItems_PurchaseEntryId ON PurchaseItems(PurchaseEntryId);
    CREATE INDEX IX_PurchaseItems_ProductId ON PurchaseItems(ProductId);
END

-- ============================================
-- 5. PURCHASE RETURN TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurchaseReturns')
BEGIN
    CREATE TABLE PurchaseReturns (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ReturnNo NVARCHAR(50) NOT NULL UNIQUE,
        ReturnDate DATETIME2 NOT NULL,
        PurchaseEntryId INT NOT NULL,
        SupplierId INT NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        Reason NVARCHAR(500) NULL,
        Remarks NVARCHAR(500) NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        CreatedBy NVARCHAR(100) NULL,
        FOREIGN KEY (PurchaseEntryId) REFERENCES PurchaseEntries(Id) ON DELETE NO ACTION,
        FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id) ON DELETE NO ACTION
    );
    
    CREATE INDEX IX_PurchaseReturns_PurchaseEntryId ON PurchaseReturns(PurchaseEntryId);
    CREATE INDEX IX_PurchaseReturns_SupplierId ON PurchaseReturns(SupplierId);
END

-- ============================================
-- 6. PURCHASE RETURN ITEM TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurchaseReturnItems')
BEGIN
    CREATE TABLE PurchaseReturnItems (
        Id INT PRIMARY KEY IDENTITY(1,1),
        PurchaseReturnId INT NOT NULL,
        PurchaseItemId INT NOT NULL,
        ProductId INT NOT NULL,
        ReturnQuantity FLOAT NOT NULL,
        ReturnAmount DECIMAL(18,2) NOT NULL,
        Reason NVARCHAR(500) NULL,
        FOREIGN KEY (PurchaseReturnId) REFERENCES PurchaseReturns(Id) ON DELETE CASCADE,
        FOREIGN KEY (PurchaseItemId) REFERENCES PurchaseItems(Id) ON DELETE NO ACTION,
        FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE NO ACTION
    );
    
    CREATE INDEX IX_PurchaseReturnItems_PurchaseReturnId ON PurchaseReturnItems(PurchaseReturnId);
END

-- ============================================
-- 7. BILL RETURN TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BillReturns')
BEGIN
    CREATE TABLE BillReturns (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ReturnNo NVARCHAR(50) NOT NULL UNIQUE,
        ReturnDate DATETIME2 NOT NULL,
        OriginalBillId INT NOT NULL,
        OriginalBillNo NVARCHAR(50) NULL,
        CustomerId INT NULL,
        ReturnAmount DECIMAL(18,2) NOT NULL,
        CGSTAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        SGSTAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        IGSTAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        PaymentMode NVARCHAR(50) NULL,
        Reason NVARCHAR(500) NULL,
        Remarks NVARCHAR(500) NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        CreatedBy NVARCHAR(100) NULL,
        FOREIGN KEY (OriginalBillId) REFERENCES Bills(Id) ON DELETE NO ACTION,
        FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE NO ACTION
    );
    
    CREATE INDEX IX_BillReturns_OriginalBillId ON BillReturns(OriginalBillId);
    CREATE INDEX IX_BillReturns_CustomerId ON BillReturns(CustomerId);
END

-- ============================================
-- 8. BILL RETURN ITEM TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BillReturnItems')
BEGIN
    CREATE TABLE BillReturnItems (
        Id INT PRIMARY KEY IDENTITY(1,1),
        BillReturnId INT NOT NULL,
        OriginalBillItemId INT NOT NULL,
        ProductId INT NOT NULL,
        ReturnQuantity FLOAT NOT NULL,
        ReturnPrice DECIMAL(18,2) NOT NULL,
        ReturnAmount DECIMAL(18,2) NOT NULL,
        CGSTAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        SGSTAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        IGSTAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        Reason NVARCHAR(500) NULL,
        FOREIGN KEY (BillReturnId) REFERENCES BillReturns(Id) ON DELETE CASCADE,
        FOREIGN KEY (OriginalBillItemId) REFERENCES BillItems(Id) ON DELETE NO ACTION,
        FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE NO ACTION
    );
    
    CREATE INDEX IX_BillReturnItems_BillReturnId ON BillReturnItems(BillReturnId);
END

-- ============================================
-- 9. LOYALTY TRANSACTION TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LoyaltyTransactions')
BEGIN
    CREATE TABLE LoyaltyTransactions (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CustomerId INT NOT NULL,
        TransactionDate DATETIME2 NOT NULL,
        TransactionType NVARCHAR(50) NOT NULL,
        Points DECIMAL(18,2) NOT NULL,
        BillNo NVARCHAR(50) NULL,
        BillId INT NULL,
        Description NVARCHAR(500) NULL,
        BalanceAfter DECIMAL(18,2) NOT NULL,
        ExpiryDate DATETIME2 NULL,
        FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE CASCADE,
        FOREIGN KEY (BillId) REFERENCES Bills(Id) ON DELETE NO ACTION
    );
    
    CREATE INDEX IX_LoyaltyTransactions_CustomerId ON LoyaltyTransactions(CustomerId);
    CREATE INDEX IX_LoyaltyTransactions_TransactionDate ON LoyaltyTransactions(TransactionDate);
END

-- ============================================
-- 10. EXPIRY ALERT TABLE
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ExpiryAlerts')
BEGIN
    CREATE TABLE ExpiryAlerts (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ProductId INT NOT NULL,
        BatchNo NVARCHAR(50) NULL,
        ExpiryDate DATETIME2 NOT NULL,
        Quantity FLOAT NOT NULL,
        DaysUntilExpiry INT NOT NULL,
        AlertLevel NVARCHAR(50) NOT NULL DEFAULT 'Normal',
        IsNotified BIT NOT NULL DEFAULT 0,
        NotifiedDate DATETIME2 NULL,
        FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_ExpiryAlerts_ProductId ON ExpiryAlerts(ProductId);
    CREATE INDEX IX_ExpiryAlerts_ExpiryDate ON ExpiryAlerts(ExpiryDate);
    CREATE INDEX IX_ExpiryAlerts_AlertLevel ON ExpiryAlerts(AlertLevel);
END

-- ============================================
-- 11. UPDATE EXISTING TABLES
-- ============================================

-- Add columns to Customers table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Customers') AND name = 'TotalPointsEarned')
BEGIN
    ALTER TABLE Customers ADD TotalPointsEarned DECIMAL(18,2) NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Customers') AND name = 'TotalPointsRedeemed')
BEGIN
    ALTER TABLE Customers ADD TotalPointsRedeemed DECIMAL(18,2) NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Customers') AND name = 'CreditLimit')
BEGIN
    ALTER TABLE Customers ADD CreditLimit DECIMAL(18,2) NOT NULL DEFAULT 0;
END

-- Update Points column to DECIMAL if it's FLOAT
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Customers') AND name = 'Points' AND system_type_id = 6)
BEGIN
    ALTER TABLE Customers ALTER COLUMN Points DECIMAL(18,2) NOT NULL DEFAULT 0;
END

-- Add columns to Suppliers table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'OutstandingBalance')
BEGIN
    ALTER TABLE Suppliers ADD OutstandingBalance DECIMAL(18,2) NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'CreditLimit')
BEGIN
    ALTER TABLE Suppliers ADD CreditLimit DECIMAL(18,2) NOT NULL DEFAULT 0;
END

-- Add columns to CustomerPayments table (for backward compatibility)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CustomerPayments') AND name = 'BillId')
BEGIN
    ALTER TABLE CustomerPayments ADD BillId INT NULL;
    ALTER TABLE CustomerPayments ADD CONSTRAINT FK_CustomerPayments_Bills 
        FOREIGN KEY (BillId) REFERENCES Bills(Id) ON DELETE NO ACTION;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CustomerPayments') AND name = 'ReferenceNo')
BEGIN
    ALTER TABLE CustomerPayments ADD ReferenceNo NVARCHAR(100) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CustomerPayments') AND name = 'Remarks')
BEGIN
    ALTER TABLE CustomerPayments ADD Remarks NVARCHAR(500) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CustomerPayments') AND name = 'CreatedDate')
BEGIN
    ALTER TABLE CustomerPayments ADD CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE();
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CustomerPayments') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE CustomerPayments ADD CreatedBy NVARCHAR(100) NULL;
END

-- Add link from SupplierPayments to PurchaseEntries
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'SupplierPayments')
    AND EXISTS (SELECT * FROM sys.tables WHERE name = 'PurchaseEntries')
    AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SupplierPayments_PurchaseEntries')
BEGIN
    ALTER TABLE SupplierPayments 
    ADD CONSTRAINT FK_SupplierPayments_PurchaseEntries 
    FOREIGN KEY (PurchaseEntryId) REFERENCES PurchaseEntries(Id) ON DELETE NO ACTION;
END

PRINT 'Migration completed successfully! All payment, return, and loyalty features are now available.';

