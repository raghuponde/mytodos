using Microsoft.AspNetCore.Mvc;
using Moq;
using MyTodos.Api.Controllers;
using MyTodos.Api.Models;
using MyTodos.Api.Services;

namespace MyTodos.Api.Tests;

public class TodosControllerTests
{
    private static Todo MakeTodo(string title = "Sample", bool isComplete = false) => new()
    {
        Id = Guid.NewGuid(),
        Title = title,
        IsComplete = isComplete,
        CreatedAt = DateTime.UtcNow,
    };

    [Fact]
    public async Task GetAll_ReturnsOk_WithTodosFromService()
    {
        var todos = new List<Todo> { MakeTodo("A"), MakeTodo("B") };
        var serviceMock = new Mock<ITodoService>();
        serviceMock.Setup(s => s.GetAllAsync(TodoStatusFilter.All)).ReturnsAsync(todos);
        var controller = new TodosController(serviceMock.Object);

        var result = await controller.GetAll("all");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(todos, okResult.Value);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenStatusIsInvalid()
    {
        var serviceMock = new Mock<ITodoService>();
        var controller = new TodosController(serviceMock.Object);

        var result = await controller.GetAll("bogus");

        Assert.IsType<BadRequestObjectResult>(result);
        serviceMock.Verify(s => s.GetAllAsync(It.IsAny<TodoStatusFilter>()), Times.Never);
    }

    [Fact]
    public async Task GetAll_PassesPendingFilter_ToService()
    {
        var todos = new List<Todo> { MakeTodo("Pending one") };
        var serviceMock = new Mock<ITodoService>();
        serviceMock.Setup(s => s.GetAllAsync(TodoStatusFilter.Pending)).ReturnsAsync(todos);
        var controller = new TodosController(serviceMock.Object);

        var result = await controller.GetAll("pending");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(todos, okResult.Value);
        serviceMock.Verify(s => s.GetAllAsync(TodoStatusFilter.Pending), Times.Once);
    }

    [Fact]
    public async Task GetAll_PassesCompletedFilter_ToService()
    {
        var todos = new List<Todo> { MakeTodo("Done one", isComplete: true) };
        var serviceMock = new Mock<ITodoService>();
        serviceMock.Setup(s => s.GetAllAsync(TodoStatusFilter.Completed)).ReturnsAsync(todos);
        var controller = new TodosController(serviceMock.Object);

        var result = await controller.GetAll("completed");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(todos, okResult.Value);
        serviceMock.Verify(s => s.GetAllAsync(TodoStatusFilter.Completed), Times.Once);
    }

    [Fact]
    public async Task GetAll_ParsesStatus_CaseInsensitively()
    {
        var todos = new List<Todo>();
        var serviceMock = new Mock<ITodoService>();
        serviceMock.Setup(s => s.GetAllAsync(TodoStatusFilter.Completed)).ReturnsAsync(todos);
        var controller = new TodosController(serviceMock.Object);

        var result = await controller.GetAll("Completed");

        Assert.IsType<OkObjectResult>(result);
        serviceMock.Verify(s => s.GetAllAsync(TodoStatusFilter.Completed), Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenTodoExists()
    {
        var todo = MakeTodo();
        var serviceMock = new Mock<ITodoService>();
        serviceMock.Setup(s => s.GetByIdAsync(todo.Id)).ReturnsAsync(todo);
        var controller = new TodosController(serviceMock.Object);

        var result = await controller.GetById(todo.Id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(todo, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenTodoMissing()
    {
        var serviceMock = new Mock<ITodoService>();
        serviceMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Todo?)null);
        var controller = new TodosController(serviceMock.Object);

        var result = await controller.GetById(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_ReturnsBadRequest_WhenTitleIsEmptyOrWhitespace(string title)
    {
        var serviceMock = new Mock<ITodoService>();
        var controller = new TodosController(serviceMock.Object);
        var request = new CreateTodoRequest { Title = title };

        var result = await controller.Create(request);

        Assert.IsType<BadRequestObjectResult>(result);
        serviceMock.Verify(s => s.CreateAsync(It.IsAny<string>(), It.IsAny<DateOnly?>()), Times.Never);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WhenTitleIsValid()
    {
        var created = MakeTodo("New todo");
        var serviceMock = new Mock<ITodoService>();
        serviceMock.Setup(s => s.CreateAsync("New todo", null)).ReturnsAsync(created);
        var controller = new TodosController(serviceMock.Object);
        var request = new CreateTodoRequest { Title = "New todo" };

        var result = await controller.Create(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(TodosController.GetById), createdResult.ActionName);
        Assert.Equal(created.Id, createdResult.RouteValues!["id"]);
        Assert.Same(created, createdResult.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Update_ReturnsBadRequest_WhenTitleIsEmptyOrWhitespace(string title)
    {
        var serviceMock = new Mock<ITodoService>();
        var controller = new TodosController(serviceMock.Object);
        var request = new UpdateTodoRequest { Title = title };

        var result = await controller.Update(Guid.NewGuid(), request);

        Assert.IsType<BadRequestObjectResult>(result);
        serviceMock.Verify(
            s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateOnly?>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenServiceUpdatesSuccessfully()
    {
        var updated = MakeTodo("Updated", isComplete: true);
        var serviceMock = new Mock<ITodoService>();
        serviceMock
            .Setup(s => s.UpdateAsync(updated.Id, "Updated", null, true))
            .ReturnsAsync(updated);
        var controller = new TodosController(serviceMock.Object);
        var request = new UpdateTodoRequest { Title = "Updated", IsComplete = true };

        var result = await controller.Update(updated.Id, request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Same(updated, okResult.Value);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenIdDoesNotExist()
    {
        var serviceMock = new Mock<ITodoService>();
        serviceMock
            .Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateOnly?>(), It.IsAny<bool>()))
            .ReturnsAsync((Todo?)null);
        var controller = new TodosController(serviceMock.Object);
        var request = new UpdateTodoRequest { Title = "Doesn't matter" };

        var result = await controller.Update(Guid.NewGuid(), request);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenServiceSucceeds()
    {
        var serviceMock = new Mock<ITodoService>();
        serviceMock.Setup(s => s.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);
        var controller = new TodosController(serviceMock.Object);

        var result = await controller.Delete(Guid.NewGuid());

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenServiceFails()
    {
        var serviceMock = new Mock<ITodoService>();
        serviceMock.Setup(s => s.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);
        var controller = new TodosController(serviceMock.Object);

        var result = await controller.Delete(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }
}
