# Leave Management System

A comprehensive Leave Management System built with .NET Core 8.0, following Clean Architecture, SOLID principles, and PostgreSQL database.

## Architecture

The solution follows **Clean Architecture** with the following layers:

- **Domain**: Core business entities, enums, and domain models
- **Application**: Business logic, DTOs, services, and interfaces
- **Infrastructure**: Data access (Entity Framework Core), repositories, database context
- **API**: Controllers, middleware, and API endpoints

## Features

- ✅ User Management (Employee, Admin, Super-Admin)
- ✅ Leave Type Configuration (Daily/Hourly, Sick Leave)
- ✅ Leave Request Management with Calendar Support
- ✅ Leave Balance Calculation and Tracking
- ✅ Working Hours Management
- ✅ Company Management
- ✅ Billing and Subscription Management
- ✅ Pricing Tiers Configuration
- ✅ Payment Tracking

## Prerequisites

- .NET 8.0 SDK
- PostgreSQL 12+ database
- Visual Studio 2022 or VS Code (optional)

## Database Setup

1. Create a PostgreSQL database:
```sql
CREATE DATABASE LeaveManagementSystem;
```

2. Update the connection string in `LeaveManagementSystem.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=LeaveManagementSystem;Username=your_username;Password=your_password"
  }
}
```

## Running the Application

1. **Restore packages:**
```bash
dotnet restore
```

2. **Create database migrations:**
```bash
cd LeaveManagementSystem.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../LeaveManagementSystem.API
```

3. **Apply migrations to database:**
```bash
dotnet ef database update --startup-project ../LeaveManagementSystem.API
```

4. **Run the API:**
```bash
cd LeaveManagementSystem.API
dotnet run
```

5. **Access Swagger UI:**
   - Navigate to `https://localhost:5001/swagger` (or the port shown in the console)

## Project Structure

```
LeaveManagementSystem/
├── LeaveManagementSystem.Domain/          # Domain entities and enums
│   ├── Entities/                          # Domain entities
│   ├── Enums/                             # Enumeration types
│   └── Common/                            # Base classes
├── LeaveManagementSystem.Application/     # Application layer
│   ├── DTOs/                              # Data Transfer Objects
│   ├── Interfaces/                        # Service interfaces
│   ├── Services/                          # Business logic services
│   └── DependencyInjection.cs            # DI configuration
├── LeaveManagementSystem.Infrastructure/ # Infrastructure layer
│   ├── Data/                              # DbContext
│   ├── Repositories/                      # Repository implementations
│   └── DependencyInjection.cs            # DI configuration
└── LeaveManagementSystem.API/            # API layer
    ├── Controllers/                       # API controllers
    ├── Program.cs                         # Application entry point
    └── appsettings.json                    # Configuration
```

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login

### Users
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user
- `GET /api/users/company/{companyId}` - Get users by company

### Leave Requests
- `GET /api/leaverequests/user/{userId}` - Get leave requests by user
- `GET /api/leaverequests/{id}` - Get leave request by ID
- `GET /api/leaverequests/company/{companyId}/pending` - Get pending requests
- `POST /api/leaverequests/user/{userId}` - Create leave request
- `POST /api/leaverequests/{id}/approve` - Approve leave request
- `POST /api/leaverequests/{id}/reject` - Reject leave request

### Leave Balances
- `GET /api/leavebalances/user/{userId}` - Get leave balances by user
- `GET /api/leavebalances/user/{userId}/leave-type/{leaveTypeId}` - Get specific balance

## Code Quality

This project follows:
- ✅ **SOLID Principles**
- ✅ **Clean Code** practices
- ✅ **OOP** design patterns
- ✅ Comprehensive **XML comments** throughout
- ✅ **Repository Pattern** for data access
- ✅ **Dependency Injection** for loose coupling

## Notes

- Password hashing uses SHA256 (consider upgrading to bcrypt/Argon2 for production)
- The system supports both hourly and daily leave calculations
- Leave balances are automatically updated when requests are approved
- The system includes validation for overlapping leave requests
- Calendar-based UI features are supported through the API endpoints

## License

This project is created for educational and commercial use.

