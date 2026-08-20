using System.Collections.Concurrent;
using TodoApi.Models;

namespace TodoApi.Services;

public class TodoService : ITodoService
{
    private readonly ConcurrentDictionary<Guid, TodoItem> _items = new();
    private readonly ITodoAuditLogger _auditLogger;

    public TodoService(ITodoAuditLogger auditLogger)
    {
        _auditLogger = auditLogger;
    }

    public IReadOnlyList<TodoItem> GetAll()
    {
        return _items.Values.OrderBy(x => x.CreatedAtUtc).ToList();
    }

    public TodoItem? GetById(Guid id)
    {
        return _items.TryGetValue(id, out var item) ? item : null;
    }

    public TodoItem Create(CreateTodoRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Title cannot be empty.", nameof(request));
        }

        var item = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            IsCompleted = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        _items[item.Id] = item;
        _auditLogger.LogAction("Created", item.Id);
        return item;
    }

    public bool Update(Guid id, UpdateTodoRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Title cannot be empty.", nameof(request));
        }

        if (!_items.TryGetValue(id, out var existing))
        {
            return false;
        }

        existing.Title = request.Title.Trim();
        existing.IsCompleted = request.IsCompleted;
        _auditLogger.LogAction("Updated", id);
        return true;
    }

    public bool Delete(Guid id)
    {
        var removed = _items.TryRemove(id, out _);
        if (removed)
        {
            _auditLogger.LogAction("Deleted", id);
        }
        return removed;
    }
}
