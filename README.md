StackBuldAssessment

StackBuldAssessment is an e-commerce/order management API built with .NET 8, MediatR, and Entity Framework Core, featuring JWT-based authentication, role-based authorization, Paystack payment integration, and a modular architecture for scalability.

Table of Contents
Project Overview
Tech Stack
Setup & Configuration
Database Schema
API Endpoints
DTOs & Models
Enums
Validation
Authentication & Authorization
Payment Gateway Integration
Example Usage
Error Handling

Project Overview
StackBuldAssessment is a RESTful API for managing users, products, orders, and transactions. Key features include:

User Management – Register, login, role-based access (Customer, Admin)

Products – CRUD operations for products (Admin only for create/update/delete)

Orders – Customers can place orders, view their orders, and track status

Transactions & Payments – Payment initiation and verification via Paystack

Pagination & Filtering – For orders, transactions, and products

Database Seeding – Default admin user seeded on startup

The project follows CQRS pattern using MediatR, keeping commands, queries, and handlers separate for better maintainability.

Tech Stack

.NET 8
Entity Framework Core
MediatR
FluentValidation
JWT Authentication
PostgreSQL
Paystack Payment Integration
ASP.NET Core API Versioning

Setup & Configuration
1. Clone the repository
git clone https://github.com/Adeyemiadigun/StackBuldAssessment.git

cd StackBuldAssessment

3. Install dependencies
dotnet restore

4. Configure appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Paystack": {
    "SecretKey": "sk_test_xxx"
  },
  "JwtSettings": {
    "Secret": "YOUR_SECRET_KEY",
    "Issuer": "OrderApi",
    "Audience": "OrderApiUsers"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=store_db;Username=postgres;Password=password;Port=5432;"
  },
  "DefaultAdmin": {
    "Email": "admin email",
    "Password": "admin password"
  }
}

5. Run migrations & seed database
dotnet ef database update


Default admin user will be created automatically using credentials from DefaultAdmin.

Database Schema
Tables
Table	Description
Users	Stores user info (Email, PasswordHash, Role, CreatedAt)
Products	Product info (Name, Description, Price, StockQuantity, CreatedAt)
Orders	Orders placed by users (Status, TotalAmount, CreatedAt)
OrderItems	Items in each order (ProductId, Quantity, UnitPrice)
PaymentTransactions	Payment info (Amount, Status, Reference, Provider)

Relationships:

User → Orders (1:N)

Order → OrderItems (1:N)

Order → PaymentTransactions (1:N)

User → PaymentTransactions (1:N)

API Endpoints
Auth
Method	Endpoint	Roles	Request	Response
POST	/api/v1/auth/register	Anonymous	{ "Email": "...", "Password": "..." }	{ "UserId": "..." }
POST	/api/v1/auth/login	Anonymous	{ "Email": "...", "Password": "..." }	{ "Data": { "AccessToken": "...", "ExpiresAt": "..." }, "Success": true, "Message": "Login Successfully" }

Products
Method	Endpoint	Roles	Request	Response
POST	/api/v1/products	Admin	{ "Name":"...", "Description":"...", "Price":100, "StockQuantity":10 }	201 Created
PUT	/api/v1/products/{id}	Admin	{ "ProductName":"...", "Description":"...", "Price":..., "StockQuantity":... }	204 No Content
DELETE	/api/v1/products/{id}	Admin	-	204 No Content
GET	/api/v1/products	Public	?page=1&pageSize=10	Paginated list of products
GET	/api/v1/products/{id}	Public	-	Product details

Orders
Method	Endpoint	Roles	Request	Response
GET	/api/v1/orders	Customer	?status=Paid&fromDate=...&toDate=...&page=1&pageSize=10	Paginated user orders
GET	/api/v1/orders/{id}	Customer/Admin	-	Order details
GET	/api/v1/orders/admin/orders	Admin	?status=Paid&fromDate=...&toDate=...&page=1&pageSize=10	Paginated all orders
GET	/api/v1/orders/{orderId}/transactions	Customer/Admin	-	Transaction history for an order

User Transactions
Method	Endpoint	Roles	Request	Response
GET	/api/v1/users/transactions	Customer	?status=Success&fromDate=...&toDate=...&page=1&pageSize=10	Paginated list of user transactions

Payments
Method	Endpoint	Roles	Request	Response
POST	/api/v1/payments/initiate	Customer	{ "OrderId":"...", "Email":"..." }	{ "Success": true, "AuthorizationUrl": "...", "Reference": "..." }
GET	/api/v1/payments/verify	Customer	?reference=...	{ "Success": true, "Status": "Success", "Amount": 100, "Reference": "..." }

DTOs & Models
Auth
public record CreateUserDto(string Email, string Password);
public record LoginDto(string Email, string Password);
public record AuthResponse(string AccessToken, DateTime ExpiresAt);

Products
public record CreateProductDto(string Name, string Description, decimal Price, int StockQuantity);
public record UpdateProductRequestDto
{
    public string ProductName { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}

Orders
public record OrderListDto(Guid Id, OrderStatus Status, decimal TotalAmount, int ItemCount, DateTime CreatedAt);
public record OrderItemDto(Guid ProductId, int Quantity, decimal UnitPrice, decimal LineTotal);
public record OrderDetailsDto(Guid Id, OrderStatus Status, decimal TotalAmount, DateTime CreatedAt, IReadOnlyList<OrderItemDto> Items, IReadOnlyList<OrderTransactionDto> Transactions);
public record OrderTransactionDto(Guid Id, decimal Amount, PaymentStatus Status, string Provider, string Reference, DateTime CreatedAt);

Payments
public record PaymentInitResult(bool Success, string AuthorizationUrl, string Reference);
public record PaymentVerifyResult(bool Success, string Status, decimal Amount, string Reference);
public record VerificationData(Guid OrderId, decimal AmountPaid, string Status, string TransactionReference, DateTime? PaidAt);

Enums
public enum UserRole { Customer = 1, Admin }
public enum OrderStatus { Pending = 1, Failed, Paid, Cancelled }
public enum PaymentStatus { Pending = 1, Success, Failed }

Validation

FluentValidation is used for commands:

PlaceOrderCommandValidator – ensures items exist and quantity > 0

InitiatePaymentCommandValidator – validates OrderId and email

Product and user commands also have validation rules

Authentication & Authorization

JWT tokens with 1-hour expiry

Role-based access (Customer, Admin)

Protected endpoints require Authorization: Bearer <token> header

Payment Gateway Integration

Integrated with Paystack

Initiates payment and provides authorization URL

Verifies transaction status after payment completion

Amounts in Paystack are multiplied by 100 (kobo)

Example Usage
Register User
curl -X POST https://localhost:5001/api/v1/auth/register \
-H "Content-Type: application/json" \
-d '{"Email":"user@example.com","Password":"Password123"}'

Login
curl -X POST https://localhost:5001/api/v1/auth/login \
-H "Content-Type: application/json" \
-d '{"Email":"user@example.com","Password":"Password123"}'

Create Product (Admin)
curl -X POST https://localhost:5001/api/v1/products \
-H "Authorization: Bearer <JWT>" \
-H "Content-Type: application/json" \
-d '{"Name":"Laptop","Description":"Gaming Laptop","Price":1500,"StockQuantity":10}'

Initiate Payment
curl -X POST https://localhost:5001/api/v1/payments/initiate \
-H "Authorization: Bearer <JWT>" \
-H "Content-Type: application/json" \
-d '{"OrderId":"<ORDER_ID>","Email":"user@example.com"}'

Error Handling

ApiException for API-level errors (e.g., invalid credentials)

ConcurrencyException for 409 conflicts

Responses follow:

{
  "Success": false,
  "Message": "Invalid credentials",
  "Error": {...}
}
