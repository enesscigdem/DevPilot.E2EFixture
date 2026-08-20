# DevPilot E2E Benchmark Definitions

This document defines 5 deterministic, unambiguous benchmark levels for evaluating DevPilot autonomous development runs against the `DevPilot.E2EFixture` repository.

---

## LEVEL 1 — ONE TEST FILE

- **Task Title**: Add Regression Test for Non-Existent Todo Lookup
- **Exact Intended Scope**: Add a single unit test method to `tests/TodoApi.Tests/TodoServiceTests.cs` verifying that calling `TodoService.GetById` with a non-existent `Guid` returns `null`.
- **Expected Files**:
  - `tests/TodoApi.Tests/TodoServiceTests.cs` (MODIFIED)
- **Forbidden Changes**:
  - Any production code modifications under `src/TodoApi/**`.
  - Modifying `TodosControllerTests.cs` or any `.csproj` file.
  - Adding new dependencies, mocks, or abstractions.
- **Acceptance Criteria**:
  1. `TodoServiceTests.cs` contains a unit test (e.g. `GetById_NonExistentItem_ReturnsNull`) invoking `_sut.GetById(Guid.NewGuid())`.
  2. The test asserts that the returned value is `null`.
  3. `dotnet test tests/TodoApi.Tests/TodoApi.Tests.csproj` passes with 100% success rate (14 passing tests).
  4. Exactly 1 file modified (`tests/TodoApi.Tests/TodoServiceTests.cs`).

---

## LEVEL 2 — ONE PRODUCTION FILE

- **Task Title**: Add DueDate Property to TodoItem Model
- **Exact Intended Scope**: Add an optional `DueDateUtc` nullable `DateTime?` property to the `TodoItem` model with getter and setter.
- **Expected Files**:
  - `src/TodoApi/Models/TodoItem.cs` (MODIFIED)
- **Forbidden Changes**:
  - Modifying any test files, services, controllers, or `.csproj` files.
  - Introducing new abstractions, database attributes, or package dependencies.
- **Acceptance Criteria**:
  1. `TodoItem.cs` contains `public DateTime? DueDateUtc { get; set; }`.
  2. `dotnet build DevPilot.E2EFixture.sln` compiles with 0 errors and 0 warnings.
  3. `dotnet test tests/TodoApi.Tests/TodoApi.Tests.csproj` passes with 0 failures.
  4. Exactly 1 file modified (`src/TodoApi/Models/TodoItem.cs`).

---

## LEVEL 3 — PRODUCTION + TEST

- **Task Title**: Add GetCompletedCount Method and Unit Test
- **Exact Intended Scope**: Add a `GetCompletedCount()` method to `TodoService` that returns the count of completed todos (`IsCompleted == true`), and add corresponding unit test coverage in `TodoServiceTests`.
- **Expected Files**:
  - `src/TodoApi/Services/TodoService.cs` (MODIFIED)
  - `tests/TodoApi.Tests/TodoServiceTests.cs` (MODIFIED)
- **Forbidden Changes**:
  - Modifying `ITodoService.cs`, `TodosController.cs`, models, or `.csproj` files.
  - Modifying any other files in the repository.
- **Acceptance Criteria**:
  1. `TodoService.cs` contains `public int GetCompletedCount() => _items.Values.Count(x => x.IsCompleted);`.
  2. `TodoServiceTests.cs` contains test(s) verifying `GetCompletedCount()` returns `0` when empty or without completed items, and returns the expected count when completed items exist.
  3. `dotnet test tests/TodoApi.Tests/TodoApi.Tests.csproj` passes with 100% success rate.
  4. Exactly 2 files modified (`src/TodoApi/Services/TodoService.cs` and `tests/TodoApi.Tests/TodoServiceTests.cs`).

---

## LEVEL 4 — SMALL MULTI-FILE FEATURE

- **Task Title**: Filter Todos by Completion Status
- **Exact Intended Scope**: Implement filtering todos by completion status across interface, service implementation, controller endpoint, and test suite.
- **Expected Files**:
  - `src/TodoApi/Services/ITodoService.cs` (MODIFIED)
  - `src/TodoApi/Services/TodoService.cs` (MODIFIED)
  - `src/TodoApi/Controllers/TodosController.cs` (MODIFIED)
  - `tests/TodoApi.Tests/TodoServiceTests.cs` (MODIFIED)
- **Forbidden Changes**:
  - Adding external packages, third-party libraries, or database dependencies.
  - Modifying `TodoItem.cs`, `ITodoAuditLogger.cs`, or `.csproj` files.
  - Changing existing route behavior for existing endpoints.
- **Acceptance Criteria**:
  1. `ITodoService` declares `IReadOnlyList<TodoItem> GetByStatus(bool isCompleted);`.
  2. `TodoService` implements `GetByStatus` returning items where `IsCompleted == isCompleted`, ordered by `CreatedAtUtc`.
  3. `TodosController` exposes a GET endpoint (e.g. `[HttpGet("status/{isCompleted:bool}")]` or query parameter) returning the filtered list with HTTP 200 OK.
  4. `TodoServiceTests.cs` includes unit test assertions verifying `GetByStatus(true)` and `GetByStatus(false)`.
  5. `dotnet test tests/TodoApi.Tests/TodoApi.Tests.csproj` passes with 100% success rate.
  6. Exactly 4 files modified.

---

## LEVEL 5 — EXISTING DEPENDENCY

- **Task Title**: Clear Completed Todos with Audit Logging
- **Exact Intended Scope**: Add a method `ClearCompleted()` to `TodoService` that removes all completed todos and records an audit log entry for each deleted item using the existing `ITodoAuditLogger` dependency (`LogAction("ClearedCompleted", id)`), with accompanying unit test in `TodoServiceTests`.
- **Expected Files**:
  - `src/TodoApi/Services/ITodoService.cs` (MODIFIED)
  - `src/TodoApi/Services/TodoService.cs` (MODIFIED)
  - `tests/TodoApi.Tests/TodoServiceTests.cs` (MODIFIED)
- **Forbidden Changes**:
  - Modifying `ITodoAuditLogger.cs` or `TodoAuditLogger.cs`.
  - Creating new logging abstractions or bypassing `_auditLogger.LogAction`.
  - Modifying controllers, models, or `.csproj` files.
- **Acceptance Criteria**:
  1. `ITodoService` declares `int ClearCompleted();`.
  2. `TodoService.ClearCompleted()` removes all completed items from internal storage, calls `_auditLogger.LogAction("ClearedCompleted", item.Id)` for each removed item, and returns the count of cleared items.
  3. `TodoServiceTests.cs` includes a test verifying that:
     - Completed items are removed from storage.
     - Uncompleted items remain intact.
     - `ClearCompleted()` returns the correct count.
     - `_auditLogger.Logs` contains an entry with `"Action: ClearedCompleted"` for each removed item.
  4. `dotnet test tests/TodoApi.Tests/TodoApi.Tests.csproj` passes with 100% success rate.
  5. Exactly 3 files modified.
