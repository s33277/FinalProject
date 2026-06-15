USE [RevenueRecognitionDB];
GO

DELETE FROM ContractPayments;
DELETE FROM SubscriptionPayments;
DELETE FROM Contracts;
DELETE FROM Subscriptions;
DELETE FROM Discounts;
DELETE FROM Customers;
DELETE FROM Software;
DELETE FROM Users;
GO

DBCC CHECKIDENT ('ContractPayments', RESEED, 0);
DBCC CHECKIDENT ('SubscriptionPayments', RESEED, 0);
DBCC CHECKIDENT ('Contracts', RESEED, 0);
DBCC CHECKIDENT ('Subscriptions', RESEED, 0);
DBCC CHECKIDENT ('Discounts', RESEED, 0);
DBCC CHECKIDENT ('Customers', RESEED, 0);
DBCC CHECKIDENT ('Software', RESEED, 0);
DBCC CHECKIDENT ('Users', RESEED, 0);
GO

INSERT INTO Users (Login, PasswordHash, Role)
VALUES
('superadmin', 'securepass123', 'Admin'),
('manager_john', 'mgrpassword!', 'User'),
('sales_agent', 'agentpass2026', 'User');

INSERT INTO Software (Name, Description, CurrentVersion, Category, BasePriceOneYear, BasePriceSubscription)
VALUES
('Finance Pro', 'Software for accounting and company finance.', '1.0', 'Finance', 12000.00, 1200.00),
('Edu Cloud', 'Online learning platform for schools and training companies.', '2.1', 'Education', 8000.00, 850.00),
('CRM Lite', 'Basic CRM system for small sales teams.', '3.0', 'Business', 6000.00, 600.00);

INSERT INTO Discounts (Name, OfferType, ValuePercentage, ValidFrom, ValidTo)
VALUES
('Winter Software Discount', 'Software', 10.00, '2026-01-01', '2026-12-31'),
('Subscription Start Promo', 'Subscription', 15.00, '2026-01-01', '2026-12-31'),
('Black Friday Mega Deal', 'Software', 30.00, '2026-11-20', '2026-11-30');

INSERT INTO Customers (Address, Email, PhoneNumber, IsDeleted, CustomerType, FirstName, LastName, Pesel)
VALUES
('Example Street 1, Warsaw', 'jan.kowalski@example.com', '500100200', 0, 'Individual', 'Jan', 'Kowalski', '90010112345'),
('Grodzka 14, Krakow', 'anna.nowak@example.com', '601202303', 0, 'Individual', 'Anna', 'Nowak', '85051298765'),
('Piotrkowska 99, Lodz', 'michal.wisniewski@example.com', '732456123', 0, 'Individual', 'Michal', 'Wisniewski', '95082344556');

INSERT INTO Customers (Address, Email, PhoneNumber, IsDeleted, CustomerType, CompanyName, KrsNumber)
VALUES
('Company Avenue 10, Warsaw', 'contact@abc-client.pl', '222333444', 0, 'Company', 'ABC Client Sp. z o.o.', '0000123456'),
('Tech Park 4, Wroclaw', 'info@globaltech.io', '713004005', 0, 'Company', 'GlobalTech Solutions S.A.', '0000987654'),
('Silesia Tower, Katowice', 'procurement@nordiclogistics.pl', '322001122', 0, 'Company', 'Nordic Logistics Sp. z o.o.', '0000555666');

DECLARE @JanId int = (SELECT Id FROM Customers WHERE Pesel = '90010112345');
DECLARE @AnnaId int = (SELECT Id FROM Customers WHERE Pesel = '85051298765');
DECLARE @MichalId int = (SELECT Id FROM Customers WHERE Pesel = '95082344556');
DECLARE @AbcId int = (SELECT Id FROM Customers WHERE KrsNumber = '0000123456');
DECLARE @GlobalTechId int = (SELECT Id FROM Customers WHERE KrsNumber = '0000987654');
DECLARE @NordicId int = (SELECT Id FROM Customers WHERE KrsNumber = '0000555666');

DECLARE @FinanceId int = (SELECT Id FROM Software WHERE Name = 'Finance Pro');
DECLARE @EduId int = (SELECT Id FROM Software WHERE Name = 'Edu Cloud');
DECLARE @CrmId int = (SELECT Id FROM Software WHERE Name = 'CRM Lite');

INSERT INTO Contracts
(CustomerId, SoftwareId, SoftwareVersion, StartDate, EndDate, AdditionalSupportYears, OriginalPrice, FinalPrice, IsSigned)
VALUES
(@JanId, @FinanceId, '1.0', '2026-06-10', '2026-06-25', 1, 13000.00, 11700.00, 1),
(@AnnaId, @EduId, '2.1', '2026-06-12', '2026-06-27', 0, 8000.00, 7200.00, 0),
(@AbcId, @CrmId, '3.0', '2026-06-14', '2026-06-29', 2, 8000.00, 7200.00, 1);

DECLARE @Contract1 int = (SELECT Id FROM Contracts WHERE CustomerId = @JanId AND SoftwareId = @FinanceId);
DECLARE @Contract2 int = (SELECT Id FROM Contracts WHERE CustomerId = @AnnaId AND SoftwareId = @EduId);
DECLARE @Contract3 int = (SELECT Id FROM Contracts WHERE CustomerId = @AbcId AND SoftwareId = @CrmId);

INSERT INTO ContractPayments (ContractId, Amount, PaymentDate, IsRefunded)
VALUES
(@Contract1, 11700.00, '2026-06-12', 0),
(@Contract2, 2000.00, '2026-06-14', 0),
(@Contract3, 7200.00, '2026-06-15', 0);

INSERT INTO Subscriptions
(CustomerId, SoftwareId, Name, PricePerPeriod, RenewalPeriodMonths, ValidUntil, Status)
VALUES
(@MichalId, @FinanceId, 'Finance Pro Monthly', 1200.00, 1, '2026-07-15', 'Active'),
(@GlobalTechId, @EduId, 'Edu Cloud Yearly', 8500.00, 12, '2027-06-15', 'Active'),
(@NordicId, @CrmId, 'CRM Lite Monthly', 600.00, 1, '2026-07-15', 'Active');

DECLARE @Sub1 int = (SELECT Id FROM Subscriptions WHERE CustomerId = @MichalId AND SoftwareId = @FinanceId);
DECLARE @Sub2 int = (SELECT Id FROM Subscriptions WHERE CustomerId = @GlobalTechId AND SoftwareId = @EduId);
DECLARE @Sub3 int = (SELECT Id FROM Subscriptions WHERE CustomerId = @NordicId AND SoftwareId = @CrmId);

INSERT INTO SubscriptionPayments
(SubscriptionId, Amount, PaymentDate, CoveredPeriodStart, CoveredPeriodEnd)
VALUES
(@Sub1, 1020.00, '2026-06-15', '2026-06-15', '2026-07-15'),
(@Sub2, 7225.00, '2026-06-15', '2026-06-15', '2027-06-15'),
(@Sub3, 510.00, '2026-06-15', '2026-06-15', '2026-07-15');
GO
