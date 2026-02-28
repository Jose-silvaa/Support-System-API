### Support System API

📌 Description

Support System API is a RESTful backend application built with ASP.NET Core.
It allows users to create, update, and manage support tickets based on their roles.

The project follows clean architecture principles and uses Entity Framework Core for data persistence.

🚀 Tech Stack

ASP.NET Core

Entity Framework Core

SQL Server

JWT Authentication

📂 Project Structure

Domain → Entities and Enums

Data → DbContext and database configuration

Services → Business logic

DTOs → Data Transfer Objects

Controllers → API endpoints

✨ Features

Create tickets

Update tickets

Delete tickets

Role-based ticket filtering

User authentication (JWT)

Status management (Open, Closed, etc.)

🔐 Authentication

This API uses JWT (JSON Web Token) authentication.
Users must provide a valid token in the Authorization header:

Authorization: Bearer your_token_here
