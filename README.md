
A REST API for managing company information with full CRUD operations.
Features

Company management (Create, Read, Update, Delete)
API Key authentication
Input validation
Swagger documentation
Database integration

Quick Start

 bash# Clone repository
	
 git clone https://github.com/danatontea/CompanyApi.git
	
 cd CompanyApi
	

# Install dependencies
dotnet restore

Create Database on mysql server and set the connectionstring in the appsettings.json file.

CREATE DATABASE `companies`

# Update database
dotnet ef database update

# Run application
dotnet run
API Endpoints

GET /api/Companies - Get all companies

GET /api/Companies/{id} - Get company by ID

GET /api/Companies/by-isin/{isin} - Get company by Isin

POST /api/Companies - Create new company

PUT /api/Companies/{id} - Update company


#Authentication

Include API key in request header:

X-API-Key: your-api-key-here

Documentation

Swagger UI available at: http://localhost:5000/swagger
