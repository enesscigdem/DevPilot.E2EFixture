using FluentAssertions;
using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace TodoApi.Tests;

public class TodoServiceTests
{
    private readonly TodoAuditLogger _auditLogger;
    private readonly TodoService _sut;

    public TodoServiceTests()
    {
        _auditLogger = new TodoAuditLogger();
        _sut = new TodoService(_auditLogger);
    }

    [Fact]
    public void Create_ValidTitle_ReturnsCreatedTodoAndLogsAudit()
    {
        // Act
        var result = _sut.Create(new CreateTodoRequest { Title = "Write benchmark tests" });

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Title.Should().Be("Write benchmark tests");
        result.IsCompleted.Should().BeFalse();
        _auditLogger.Logs.Should().ContainSingle(log => log.Contains("Action: Created") && log.Contains(result.Id.ToString()));
    }

    [Fact]
    public void Create_EmptyTitle_ThrowsArgumentException()
    {
        // Act
        var act = () => _sut.Create(new CreateTodoRequest { Title = "   " });

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetById_ExistingItem_ReturnsTodo()
    {
        // Arrange
        var created = _sut.Create(new CreateTodoRequest { Title = "Existing item" });

        // Act
        var result = _sut.GetById(created.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.Title.Should().Be("Existing item");
    }

    [Fact]
    public void GetAll_MultipleItems_ReturnsAllOrderedByCreatedAt()
    {
        // Arrange
        var item1 = _sut.Create(new CreateTodoRequest { Title = "Item 1" });
        var item2 = _sut.Create(new CreateTodoRequest { Title = "Item 2" });

        // Act
        var results = _sut.GetAll();

        // Assert
        results.Should().HaveCount(2);
        results.Select(x => x.Id).Should().ContainInOrder(item1.Id, item2.Id);
    }

    [Fact]
    public void Update_ExistingItem_UpdatesPropertiesAndReturnsTrue()
    {
        // Arrange
        var created = _sut.Create(new CreateTodoRequest { Title = "Old Title" });

        // Act
        var updated = _sut.Update(created.Id, new UpdateTodoRequest { Title = "New Title", IsCompleted = true });
        var fetched = _sut.GetById(created.Id);

        // Assert
        updated.Should().BeTrue();
        fetched.Should().NotBeNull();
        fetched!.Title.Should().Be("New Title");
        fetched.IsCompleted.Should().BeTrue();
        _auditLogger.Logs.Should().Contain(log => log.Contains("Action: Updated") && log.Contains(created.Id.ToString()));
    }

    [Fact]
    public void Update_NonExistentItem_ReturnsFalse()
    {
        // Act
        var updated = _sut.Update(Guid.NewGuid(), new UpdateTodoRequest { Title = "Non existent", IsCompleted = true });

        // Assert
        updated.Should().BeFalse();
    }

    [Fact]
    public void Delete_ExistingItem_RemovesItemAndReturnsTrue()
    {
        // Arrange
        var created = _sut.Create(new CreateTodoRequest { Title = "Item to delete" });

        // Act
        var deleted = _sut.Delete(created.Id);
        var fetched = _sut.GetById(created.Id);

        // Assert
        deleted.Should().BeTrue();
        fetched.Should().BeNull();
        _auditLogger.Logs.Should().Contain(log => log.Contains("Action: Deleted") && log.Contains(created.Id.ToString()));
    }

    [Fact]
    public void Delete_NonExistentItem_ReturnsFalse()
    {
        // Act
        var deleted = _sut.Delete(Guid.NewGuid());

        // Assert
        deleted.Should().BeFalse();
    }
}
