namespace TodoApi.Services;

public interface ITodoAuditLogger
{
    IReadOnlyList<string> Logs { get; }
    void LogAction(string action, Guid todoId);
}
