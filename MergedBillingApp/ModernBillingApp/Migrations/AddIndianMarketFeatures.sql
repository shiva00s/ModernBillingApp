-- Migration Script for Indian Market Features
-- Run this script to add new fields for Indian supermarket features

-- Add barcode and unit fields to Products table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Products]') AND name = 'Barcode')
BEGIN
    ALTER TABLE [Products] ADD [Barcode] NVARCHAR(50) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Products]') AND name = 'Unit')
BEGIN
    ALTER TABLE [Products] ADD [Unit] NVARCHAR(20) NULL DEFAULT 'PCS';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Products]') AND name = 'ExpiryDate')
BEGIN
    ALTER TABLE [Products] ADD [ExpiryDate] DATETIME2 NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Products]') AND name = 'BatchNo')
BEGIN
    ALTER TABLE [Products] ADD [BatchNo] NVARCHAR(50) NULL;
END

-- Add Indian GST fields to Bills table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Bills]') AND name = 'CGSTAmount')
BEGIN
    ALTER TABLE [Bills] ADD [CGSTAmount] DECIMAL(18, 2) NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Bills]') AND name = 'SGSTAmount')
BEGIN
    ALTER TABLE [Bills] ADD [SGSTAmount] DECIMAL(18, 2) NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Bills]') AND name = 'IGSTAmount')
BEGIN
    ALTER TABLE [Bills] ADD [IGSTAmount] DECIMAL(18, 2) NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Bills]') AND name = 'IsInterState')
BEGIN
    ALTER TABLE [Bills] ADD [IsInterState] BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Bills]') AND name = 'UPIReference')
BEGIN
    ALTER TABLE [Bills] ADD [UPIReference] NVARCHAR(50) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Bills]') AND name = 'PaymentDetails')
BEGIN
    ALTER TABLE [Bills] ADD [PaymentDetails] NVARCHAR(MAX) NULL;
END

-- Add Indian GST fields to BillItems table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[BillItems]') AND name = 'CGSTAmount')
BEGIN
    ALTER TABLE [BillItems] ADD [CGSTAmount] DECIMAL(18, 2) NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[BillItems]') AND name = 'SGSTAmount')
BEGIN
    ALTER TABLE [BillItems] ADD [SGSTAmount] DECIMAL(18, 2) NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[BillItems]') AND name = 'IGSTAmount')
BEGIN
    ALTER TABLE [BillItems] ADD [IGSTAmount] DECIMAL(18, 2) NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[BillItems]') AND name = 'Unit')
BEGIN
    ALTER TABLE [BillItems] ADD [Unit] NVARCHAR(20) NULL;
END

-- Create index on Barcode for faster lookups
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_Barcode' AND object_id = OBJECT_ID(N'[Products]'))
BEGIN
    CREATE INDEX [IX_Products_Barcode] ON [Products] ([Barcode]);
END

PRINT 'Migration completed successfully!';

