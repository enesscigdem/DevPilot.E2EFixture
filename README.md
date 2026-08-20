# DevPilot.E2EFixture

Deterministic, lightweight benchmark fixture repository for DevPilot E2E evaluation.

## Solution Structure

```text
DevPilot.E2EFixture/
├── DevPilot.E2EFixture.sln
├── BENCHMARKS.md
├── benchmarks.json
├── README.md
├── src/
│   └── TodoApi/
│       ├── TodoApi.csproj
│       ├── Program.cs
│       ├── appsettings.json
│       ├── Models/
│       │   ├── TodoItem.cs
│       │   ├── CreateTodoRequest.cs
│       │   └── UpdateTodoRequest.cs
│       ├── Services/
│       │   ├── ITodoService.cs
│       │   ├── TodoService.cs
│       │   ├── ITodoAuditLogger.cs
│       │   └── TodoAuditLogger.cs
│       └── Controllers/
│           └── TodosController.cs
└── tests/
    └── TodoApi.Tests/
        ├── TodoApi.Tests.csproj
        ├── TodoServiceTests.cs
        └── TodosControllerTests.cs
```

## Running Builds and Tests

```bash
dotnet build DevPilot.E2EFixture.sln
dotnet test DevPilot.E2EFixture.sln
```
