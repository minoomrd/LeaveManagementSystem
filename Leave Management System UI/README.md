# Leave Management System UI

A modern, clean React TypeScript application for managing employee leave requests, built with Material-UI and following SOLID principles.

## Features

- 🔐 **Authentication** - Secure login system
- 📊 **Dashboard** - Overview of system statistics
- 📝 **Leave Requests** - Create and manage leave requests
- 💼 **Leave Balances** - View employee leave balances
- 👥 **User Management** - Manage system users

## Architecture

This application follows **Clean Architecture** and **SOLID Principles**:

### SOLID Principles Applied

1. **Single Responsibility Principle (SRP)**
   - Each component/service has a single, well-defined responsibility
   - Example: `AuthService` handles only authentication, `UserService` handles only user operations

2. **Open/Closed Principle (OCP)**
   - Components are open for extension but closed for modification
   - Example: `ApiService` base class can be extended for new services

3. **Liskov Substitution Principle (LSP)**
   - Derived classes can be substituted for their base classes
   - Example: All service classes extend `ApiService` and can be used interchangeably

4. **Interface Segregation Principle (ISP)**
   - Interfaces are specific and focused
   - Example: `AuthContextType` only contains auth-related methods

5. **Dependency Inversion Principle (DIP)**
   - High-level modules depend on abstractions, not concretions
   - Example: Components depend on service interfaces, not implementations

### Project Structure

```
src/
├── components/          # Reusable UI components
│   ├── Layout/         # Main layout with navigation
│   └── ProtectedRoute/ # Route protection component
├── contexts/            # React contexts (Auth, etc.)
├── pages/              # Page components
│   ├── DashboardPage/
│   ├── LoginPage/
│   ├── LeaveRequestsPage/
│   ├── LeaveBalancesPage/
│   └── UsersPage/
├── services/           # API services (following Repository pattern)
├── types/              # TypeScript type definitions
└── main.tsx            # Application entry point
```

## Getting Started

### Prerequisites

- Node.js 18+ and npm
- Backend API running on `http://localhost:5132`

### Installation

1. Install dependencies:
```bash
npm install
```

2. Start the development server:
```bash
npm run dev
```

3. Open your browser to `http://localhost:3000`

### Build for Production

```bash
npm run build
```

## Code Quality

- **TypeScript** - Full type safety
- **ESLint** - Code linting
- **Material-UI** - Modern, accessible UI components
- **Clean Code** - Comprehensive comments and documentation
- **OOP Principles** - Object-oriented design patterns

## API Integration

The application connects to the backend API at `http://localhost:5132/api`. The proxy is configured in `vite.config.ts` to handle CORS.

## Test Credentials

- **Admin**: admin@acme.com / Admin123!
- **Employee**: john.doe@acme.com / Employee123!

## License

MIT

