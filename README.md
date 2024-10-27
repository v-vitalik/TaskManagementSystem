# Task Management System API

## Description
Asp.NET Core Web API for simple task objects manipulations. Uses SQL Server as data storage.
Project created using Clean Architecture structure.
Also contains class for sending/receiving messages to RabbitMQ using MassTransit package.

## Getting Started

### Prerequisites
- .NET 8
- SQL Server.
- RabbitMQ instance or docker

### Installation
1. Clone the repository.
   ```git clone https://github.com/v-vitalik/TaskManagementSystem.git```  
2. Open repo folder
   ```cd TaskManagementSystem/src```

3. Update `~src\TaskManagementSystem.API\appsettings.json` with valid connection string and your RabbtMQ instance values.

4. Run migrations to init DB
   ```dotnet ef database update --project TaskManagementSystem.DataAccess/TaskManagementSystem.DataAccess.csproj --startup-project TaskManagementSystem.API/TaskManagementSystem.API.csproj```

5. Build project ```dotnet build```

6. Run project ```dotnet run --project TaskManagementSystem.API/TaskManagementSystem.API.csproj```


### Project Structure
```
├── src
│   ├── TaskManagementSystem.API
│   ├── TaskManagementSystem.Application
│   ├── TaskManagementSystem.DataAccess
│   └── TaskManagementSystem.Domain
└── tests
    └── TaskManagementSystem.Application.Tests```


#### How to create RabbitMQ instance
Note: make sure you have Docker Desktop installed!

1. Pull RabbitMQ image ```docker pull rabbitmq:management```
2. Start container from image ```docker run -d --name rabbitmq -p 15672:15672 rabbitmq:management```