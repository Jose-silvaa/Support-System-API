## Support System API

A RESTful API built with ASP.NET Core for managing support tickets.

The system allows users to create, update, and track tickets based on role-based access control (RBAC), ensuring proper authorization and workflow management.

## Frontend

Repository : https://github.com/Jose-silvaa/Support-System-Frontend

Live : https://support-system-frontend-delta.vercel.app

## API Documentation (Swagger)

Live : https://system-ticket-1-0.onrender.com/index.html

## Tech Stack

**Backend**
- ASP.NET Core
- Entity Framework Core

**Database**
- SQL Server

**Authentication**
- JWT (JSON Web Tokens)

**DevOps**
- Docker
  
## How to Run the Project

### Configuration

Update the `appsettings.json` file with your database connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "your_connection_string_here"
}
````

### Steps

1. Clone the repository
```bash
git clone https://github.com/Jose-silvaa/Support-System-API.git
````

2. Navigate to the project folder
```bash
cd Support-System-API      
````

3. Restore dependencies
```bash
dotnet restore
````

4. Run the application
```bash
dotnet run
````

## Features

- User authentication with JWT
- Role-based authorization
- Ticket creation and management
- Status updates (Open, In Progress, Closed)
- Logging and validation

## Running with Docker

### Pull the image

```bash
docker pull vitor590/system-ticket:1.3.0
````

