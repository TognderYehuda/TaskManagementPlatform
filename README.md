# Task Management Platform

A comprehensive task management system built with ASP.NET Core MVC backend and React frontend, designed to handle multiple task types with extensible architecture.

## 🌟 Features

- **Extensible Task System**: Support for multiple task types (Procurement, Development) with custom fields
- **Sequential Workflow**: Enforced sequential status progression with backward moves allowed
- **Type-Specific Validation**: Custom validation rules for each task type
- **Dual Frontend**: Both MVC (server-side) and React (client-side) implementations
- **User Management**: Task assignment and user switching
- **Status History**: Complete audit trail of task changes
- **Real-time Updates**: Dynamic form fields based on task type and status

## 🏗️ Architecture

The system follows clean architecture principles with clear separation of concerns:

```
TaskManagementPlatform/
├── 📁 TaskManagementPlatform/          # ASP.NET Core MVC Backend
│   ├── Controllers/                    # MVC and API Controllers
│   ├── Models/                        # Data Models
│   ├── Services/                      # Business Logic
│   ├── Data/                          # Entity Framework DbContext
│   ├── Views/                         # Razor Views (MVC UI)
│   └── ViewModels/                    # View Models
├── 📁 react-task-management/          # React Frontend
│   ├── src/components/                # React Components
│   ├── src/services/                  # API Services
│   └── public/                        # Static Assets
└── 📄 README.md                       # This file
```

## 🔧 Technologies Used

### Backend (.NET 8)
- **ASP.NET Core MVC** - Web framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database
- **RESTful APIs** - For React frontend communication

### Frontend (React 18)
- **React** - UI library
- **React Router** - Client-side routing
- **Axios** - HTTP client
- **Bootstrap 5** - CSS framework

## 📋 Task Types & Workflows

### Procurement Tasks
1. **Created** → 2. **Supplier offers received** → 3. **Purchase completed** → **Closed**
   - Status 2 requires: 2 price quotes
   - Status 3 requires: Receipt

### Development Tasks  
1. **Created** → 2. **Specification completed** → 3. **Development completed** → 4. **Distribution completed** → **Closed**
   - Status 2 requires: Specification text
   - Status 3 requires: Branch name
   - Status 4 requires: Version number

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- Node.js (v16+)
- SQL Server (LocalDB for development)
- Git

### Backend Setup (ASP.NET Core MVC)

1. **Clone the repository**
   ```bash
   git clone https://github.com/TognderYehuda/TaskManagementPlatform.git
   cd TaskManagementPlatform
   ```

2. **Setup the MVC Backend**
   ```bash
   cd TaskManagementPlatform
   dotnet restore
   dotnet run
   ```
   
3. **Access the application**
   - MVC UI: `https://localhost:5001`
   - API: `https://localhost:5001/api`

### Frontend Setup (React)

1. **Setup the React Frontend**
   ```bash
   cd react-task-management
   npm install
   npm start
   ```

2. **Access the React app**
   - React UI: `http://localhost:3000`

### Database

The application uses **Entity Framework Code First** with automatic database creation. The database will be created automatically on first run with seed data (4 demo users).

**Connection String** (in `appsettings.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManagementDb;Trusted_Conne
