# Revenue Recognition System

This is my APBD final project: a REST API application for a large corporation named ABC.
This app stores customers, software products, contracts, subscriptions, payments, discounts and calculates revenue.

**Project assumption:** in this project, taxes, public fees, and detailed accounting regulations are ignored. 
The focus is on the business logic needed to handle contracts, subscriptions, payments, discounts, and revenue calculations.


## What is inside

The solution is split into **4 projects**:

- `RevenueRecognitionSystem.API` - controllers, services, authentication, Swagger
- `RevenueRecognitionSystem.Core` - DTOs, interfaces, validators
- `RevenueRecognitionSystem.Data` - Entity Framework context, entities, configurations, migrations
- `RevenueRecognitionSystem.UnitTests` - simple business logic tests


## Main features

Customer management:
- add individual customers
- add company customers
- update customer contact/name data
- soft delete individual customers 
- company customers cannot be deleted
- PESEL and KRS are kept unique and are not changed after creation

Software:
- software products have name, description, version, category, one-year price and subscription price

Discounts:
- software discounts
- subscription discounts
- highest active discount is selected
- returning/loyal customer 5 percent discount is included

Contracts:
- create a contract offer for a customer and software
- contract has start/end date and software version
- contract duration is validated between 3 and 30 days
- extra support years cost 1000 PLN each
- contract is not revenue until fully paid
- full payment signs the contract
- late payment is rejected and old payments are marked as refunded
- customer cannot have an active contract/subscription for the same product

Subscriptions:
- create a subscription
- first payment is created immediately
- renewal period is between 1 and 24 months
- renewal payment must match expected price
- unpaid expired subscription is cancelled when renewal payment is attempted too late

Revenue:
- current revenue
- predicted revenue
- revenue for all products or one selected product
- currency conversion using simple fixed rates:
  - PLN = 1
  - EUR = 0.23
  - USD = 0.25

Authentication and roles:
- JWT login
- `User` role can use normal business endpoints
- `Admin` role can also update and delete customers


## Database

Database name used in the project:
```text
RevenueRecognitionDB
```

Connection string in `appsettings.json` and `appsettings.Development.json`:
```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=RevenueRecognitionDB;Trusted_Connection=True;TrustServerCertificate=True"
```


## How to run
From the solution folder:
```powershell
dotnet restore
dotnet build
dotnet ef database update --project RevenueRecognitionSystem.Data --startup-project RevenueRecognitionSystem.API
```

Then run the API:
```powershell
dotnet run --project RevenueRecognitionSystem.API
```

Swagger:
```text
https://localhost:5028/swagger
```

## Seed data
There is an `insert.sql` file with sample data.
Run it in SQL Server Management Studio after the migration creates the database tables.
Example login users from the seed file:
```text
superadmin / securepass123 / Admin
manager_john / mgrpassword! / User
sales_agent / agentpass2026 / User
```


## Main endpoints

Auth:
- `POST /api/auth/login`

Customers:
- `GET /api/customers`
- `GET /api/customers/{id}`
- `POST /api/customers/individuals`
- `POST /api/customers/companies`
- `PUT /api/customers/{id}` - Admin only
- `DELETE /api/customers/individuals/{id}` - Admin only

Contracts:
- `GET /api/contracts`
- `GET /api/contracts/{id}`
- `POST /api/contracts`
- `POST /api/contracts/{id}/payments`
- `DELETE /api/contracts/{id}`

Subscriptions:
- `GET /api/subscriptions`
- `GET /api/subscriptions/{id}`
- `POST /api/subscriptions`
- `POST /api/subscriptions/{id}/payments`

Revenue:
- `GET /api/revenue/current`
- `GET /api/revenue/predicted`
- `GET /api/revenue/summary`

Revenue endpoints accept optional query parameters:
```text
softwareId
currency
```

Example:
```text
/api/revenue/current?softwareId=1&currency=USD
```


## Tests

The project has simple unit tests for the most important business rules:
- individual customer soft delete
- contract full payment signs the contract
- late contract payment is rejected
- late contract old payments are refunded
- current revenue counts only signed contracts
- revenue conversion works for USD

Run tests:
```powershell
dotnet test
```


## Simplifications
- Currency conversion uses fixed rates, not a live public exchange API.
- Predicted subscription revenue is simplified and assumes one next subscription period.
- Contract/subscription payments store the relation to contract/subscription; customer is checked in the request/service instead of duplicated in the payment table.
- Password hashing exists in service, but seed users use simple demo passwords because this is easier to test quickly.