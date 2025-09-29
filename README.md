# SimpleTrack - .NET Parcel Tracking System

A modern, full-stack parcel tracking application built with .NET 9, Blazor Server, Entity Framework Core, PostgreSQL, Redis, and SignalR for real-time updates.

## 🏗️ Architecture

### Projects Structure
- **ParcelTracking.Core** - Shared library containing DTOs, models, and interfaces
- **ParcelTracking.Api** - REST API backend with JWT authentication, SignalR, and caching
- **ParcelTracking.Web** - Blazor Server frontend with real-time updates

### Technology Stack
- **.NET 9.0** - Latest .NET framework
- **Blazor Server** - Interactive web UI framework
- **Entity Framework Core** - Database ORM with PostgreSQL
- **Redis** - Distributed caching layer
- **SignalR** - Real-time communication
- **JWT Authentication** - Secure token-based auth
- **Docker & Docker Compose** - Containerization
- **GitHub Actions** - CI/CD pipeline

## 🚀 Features

### API Features
- **JWT Authentication** - Secure login with token-based auth
- **RESTful Endpoints** - Complete CRUD operations for parcels
- **Real-time Updates** - SignalR hub for live status changes
- **Caching Layer** - Redis caching for improved performance
- **Database Migrations** - Automated EF Core migrations
- **Swagger/OpenAPI** - Interactive API documentation
- **Health Checks** - Application health monitoring

### Web Features
- **Responsive UI** - Bootstrap-based responsive design
- **Real-time Tracking** - Live parcel status updates
- **User Authentication** - Secure login interface
- **Interactive Dashboard** - Track multiple parcels simultaneously
- **Search Functionality** - Quick parcel lookup by tracking number

## 📦 Getting Started

### Prerequisites
- .NET 9.0 SDK
- Docker and Docker Compose
- PostgreSQL (for local development)
- Redis (for local development)

### Quick Start with Docker Compose

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd SimpleTrack
   ```

2. **Start all services**
   ```bash
   docker compose up --build
   ```

3. **Access the applications**
   - Web Frontend: http://localhost:5001
   - API Backend: http://localhost:5000
   - API Documentation: http://localhost:5000/swagger

### Local Development Setup

1. **Start Infrastructure Services**
   ```bash
   # Start PostgreSQL
   docker run --name postgres -e POSTGRES_DB=parceltracking -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16-alpine

   # Start Redis
   docker run --name redis -p 6379:6379 -d redis:7-alpine
   ```

2. **Run Database Migrations**
   ```bash
   cd ParcelTracking.Api
   dotnet ef database update
   ```

3. **Start API Backend**
   ```bash
   cd ParcelTracking.Api
   dotnet run
   ```

4. **Start Web Frontend** (in a new terminal)
   ```bash
   cd ParcelTracking.Web
   dotnet run
   ```

## 🔐 Authentication

### Demo Accounts
The system includes demo user accounts for testing:

| Username | Password     | Description     |
|----------|--------------|-----------------|
| admin    | password123  | Administrator   |
| user     | demo123      | Regular user    |
| demo     | demo         | Demo account    |

### JWT Configuration
The application uses JWT tokens for authentication. Configure the JWT settings in `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "your-super-secure-jwt-key-256-bits-minimum",
    "Issuer": "ParcelTracking.Api",
    "Audience": "ParcelTracking.Client"
  }
}
```

## 🛠️ Configuration

### API Configuration (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=parceltracking;Username=postgres;Password=postgres;Port=5432",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Key": "your-jwt-secret-key",
    "Issuer": "ParcelTracking.Api",
    "Audience": "ParcelTracking.Client"
  },
  "AllowedOrigins": [
    "https://localhost:7001",
    "http://localhost:5001"
  ]
}
```

### Web Configuration (appsettings.json)
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:5000"
  }
}
```

## 📡 API Endpoints

### Authentication
- `POST /api/auth/login` - User login

### Parcels
- `GET /api/parcel/{trackingNumber}` - Get parcel by tracking number
- `GET /api/parcel` - Get all parcels
- `POST /api/parcel` - Create new parcel
- `PUT /api/parcel/{id}/status` - Update parcel status

### SignalR Hub
- `/hub/parcels` - Real-time parcel updates

## 🎯 Key Components

### Core Models
```csharp
public class Parcel
{
    public Guid Id { get; set; }
    public string TrackingNumber { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### SignalR Hub
```csharp
[Authorize]
public class ParcelHub : Hub
{
    // Join/leave tracking groups for specific parcels
    // Broadcast real-time status updates
}
```

### Authentication Service
```csharp
public class AuthService
{
    // JWT token management
    // Login/logout functionality
    // Token validation
}
```

## 🐳 Docker Deployment

### Docker Compose Services
- **postgres** - PostgreSQL database
- **redis** - Redis cache
- **api** - .NET API backend
- **web** - Blazor web frontend

### Environment Variables
```bash
POSTGRES_PASSWORD=your-postgres-password
JWT_SECRET_KEY=your-jwt-secret-key
ASPNETCORE_ENVIRONMENT=Production
```

## 🔄 CI/CD Pipeline

The project includes a comprehensive GitHub Actions workflow:

- **Build & Test** - Automated building and testing
- **Docker Images** - Multi-platform container builds
- **Security Scanning** - Vulnerability scanning with Trivy
- **Package Registry** - Automated publishing to GHCR

### Workflow Triggers
- Push to main branch
- Pull requests
- Manual dispatch
- Nightly scheduled builds

## 🗂️ Project Structure

```
SimpleTrack/
├── ParcelTracking.Core/          # Shared library
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Models/                   # Domain models
│   └── Interfaces/               # Service contracts
├── ParcelTracking.Api/           # REST API
│   ├── Controllers/              # API controllers
│   ├── Data/                     # Database context & migrations
│   ├── Hubs/                     # SignalR hubs
│   ├── Services/                 # Business logic
│   └── Dockerfile               # API container
├── ParcelTracking.Web/           # Blazor frontend
│   ├── Components/               # Razor components
│   ├── Services/                 # Client services
│   └── Dockerfile               # Web container
├── docker-compose.yml            # Service orchestration
└── .github/workflows/            # CI/CD pipelines
```

## 🧪 Testing

### API Testing
Use the built-in Swagger UI at `http://localhost:5000/swagger` to test API endpoints.

### Integration Testing
The solution supports both unit and integration testing patterns.

## 🔧 Development Notes

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName --project ParcelTracking.Api

# Update database
dotnet ef database update --project ParcelTracking.Api
```

### Real-time Features
The application uses SignalR for real-time updates:
- Parcel status changes are broadcast automatically
- Users can join specific tracking groups
- Live dashboard updates without page refresh

## 📝 License

This project is licensed under the MIT License.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📞 Support

For questions or support, please open an issue in the GitHub repository.