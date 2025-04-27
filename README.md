# ProductAPI
This is a simple ASP.NET Core Web API project for managing products.
It includes full CRUD functionality, pagination, input validation, JWT authentication, logging, and unit tests.

# Features
- Create, Read, Update, Delete (CRUD) products

- Pagination support on product listing

- Input validation with Data Annotations

- Entity Framework Core (In-Memory)

- JWT Authentication (Username: admin, Password: pswadmin)

- Swagger UI for API testing

- Logging and centralized error handling

- Unit testing with xUnit and Moq

# Technologies Used
- ASP.NET Core Web API (.NET 9)

- Entity Framework Core

- JWT Bearer Authentication

- xUnit, Moq (for unit tests)

- Swagger (Swashbuckle)

# Project Structure
- Controllers — API endpoints

- Services — Business logic

- Models — Data structures

- Data — EF Core DbContext

- Middleware — Global error handler

# Authentication
- Use the /api/auth/login endpoint to obtain a JWT token.
### Credentials:
- Username: admin

- Password: pswadmin

- Use the token in Authorization: Bearer {token} header for protected endpoints.
