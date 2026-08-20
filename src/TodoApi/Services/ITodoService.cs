using TodoApi.Models;

namespace TodoApi.Services;

public interface ITodoService
{
    IReadOnlyList<TodoItem> GetAll();
    TodoItem? GetById(Guid id);
    TodoItem Create(CreateTodoRequest request);
    bool Update(Guid id, UpdateTodoRequest request);
    bool Delete(Guid id);
    int ClearCompleted();
}
