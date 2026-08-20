using System.Collections.Concurrent;

namespace TodoApi.Services;

public class TodoAuditLogger : ITodoAuditLogger
{
    private readonly ConcurrentBag<string> _logs = new();

    public IReadOnlyList<string> Logs => _logs.ToArray();

    public void LogAction(string action, Guid todoId)
    {
        _logs.Add($"[{DateTime.UtcNow:O}] Action: {action}, TodoId: {todoId}");
    }
}
