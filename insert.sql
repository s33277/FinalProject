USE [RevenueRecognitionDB];
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Login = 'admin')
BEGIN
    INSERT INTO Users (Login, PasswordHash, Role)
    VALUES ('admin', 'admin123', 'Admin');
END

IF NOT EXISTS (SELECT 1 FROM Users WHERE Login = 'user')
BEGIN
    INSERT INTO Users (Login, PasswordHash, Role)
    VALUES ('user', 'user123', 'User');
END

IF NOT EXISTS (SELECT 1 FROM Software WHERE Name = 'Finance Pro')
BEGIN
    INSERT INTO Software (Name, Description, CurrentVersion, Category, BasePriceOneYear, BasePriceSubscription)
    VALUES ('Finance Pro', 'Software for accounting and company finance.', '1.0', 'Finance', 12000.00, 1200.00);
END

IF NOT EXISTS (SELECT 1 FROM Software WHERE Name = 'Edu Cloud')
BEGIN
    INSERT INTO Software (Name, Description, CurrentVersion, Category, BasePriceOneYear, BasePriceSubscription)
    VALUES ('Edu Cloud', 'Online learning platform for schools and training companies.', '2.1', 'Education', 8000.00, 850.00);
END

IF NOT EXISTS (SELECT 1 FROM Software WHERE Name = 'CRM Lite')
BEGIN
    INSERT INTO Software (Name, Description, CurrentVersion, Category, BasePriceOneYear, BasePriceSubscription)
    VALUES ('CRM Lite', 'Basic CRM system for small sales teams.', '3.0', 'Business', 6000.00, 600.00);
END

IF NOT EXISTS (SELECT 1 FROM Discounts WHERE Name = 'Winter Software Discount')
BEGIN
    INSERT INTO Discounts (Name, OfferType, ValuePercentage, ValidFrom, ValidTo)
    VALUES ('Winter Software Discount', 'Software', 10.00, '2026-01-01', '2026-12-31');
END

IF NOT EXISTS (SELECT 1 FROM Discounts WHERE Name = 'Subscription Start Promo')
BEGIN
    INSERT INTO Discounts (Name, OfferType, ValuePercentage, ValidFrom, ValidTo)
    VALUES ('Subscription Start Promo', 'Subscription', 15.00, '2026-01-01', '2026-12-31');
END

IF NOT EXISTS (SELECT 1 FROM Customers WHERE Pesel = '90010112345')
BEGIN
    INSERT INTO Customers
        (Address, Email, PhoneNumber, IsDeleted, CustomerType, FirstName, LastName, Pesel)
    VALUES
        ('Example Street 1, Warsaw', 'jan.kowalski@example.com', '500100200', 0, 'Individual', 'Jan', 'Kowalski', '90010112345');
END

IF NOT EXISTS (SELECT 1 FROM Customers WHERE KrsNumber = '0000123456')
BEGIN
    INSERT INTO Customers
        (Address, Email, PhoneNumber, IsDeleted, CustomerType, CompanyName, KrsNumber)
    VALUES
        ('Company Avenue 10, Warsaw', 'contact@abc-client.pl', '222333444', 0, 'Company', 'ABC Client Sp. z o.o.', '0000123456');
END
GO
