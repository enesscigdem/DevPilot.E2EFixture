using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace TodoApi.Tests;

public class TodosControllerTests
{
    private readonly TodoService _service;
    private readonly TodosController _controller;

    public TodosControllerTests()
    {
        var auditLogger = new TodoAuditLogger();
        _service = new TodoService(auditLogger);
        _controller = new TodosController(_service);
    }

    [Fact]
    public void GetAll_ReturnsOkWithList()
    {
        // Arrange
        _service.Create(new CreateTodoRequest { Title = "Controller test 1" });

        // Act
        var result = _controller.GetAll();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var items = okResult!.Value as IEnumerable<TodoItem>;
        items.Should().NotBeNull();
        items!.Should().HaveCount(1);
    }

    [Fact]
    public void GetById_ExistingItem_ReturnsOkWithItem()
    {
        // Arrange
        var created = _service.Create(new CreateTodoRequest { Title = "Find me" });

        // Act
        var result = _controller.GetById(created.Id);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var item = okResult!.Value as TodoItem;
        item.Should().NotBeNull();
        item!.Id.Should().Be(created.Id);
    }

    [Fact]
    public void GetById_NonExistentItem_ReturnsNotFound()
    {
        // Act
        var result = _controller.GetById(Guid.NewGuid());

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Create_ValidRequest_ReturnsCreatedAtAction()
    {
        // Act
        var result = _controller.Create(new CreateTodoRequest { Title = "New item" });

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        var item = createdResult!.Value as TodoItem;
        item.Should().NotBeNull();
        item!.Title.Should().Be("New item");
    }

    [Fact]
    public void Create_EmptyTitle_ReturnsBadRequest()
    {
        // Act
        var result = _controller.Create(new CreateTodoRequest { Title = "" });

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}
