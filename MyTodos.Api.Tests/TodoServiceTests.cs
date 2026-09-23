using Microsoft.EntityFrameworkCore;
using MyTodos.Api.Data;
using MyTodos.Api.Models;
using MyTodos.Api.Services;

namespace MyTodos.Api.Tests;

public class TodoServiceTests
{
    private static readonly Guid CompleteSeedId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid PendingSeedId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static TodoService CreateSeededService()
    {
        var options = new DbContextOptionsBuilder<TodosDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new TodosDbContext(options);

        db.Todos.AddRange(
            new Todo
            {
                Id = CompleteSeedId,
                Title = "Set up backend skeleton",
                IsComplete = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new Todo
            {
                Id = PendingSeedId,
                Title = "Set up frontend skeleton",
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                IsComplete = false,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 1, DateTimeKind.Utc),
            });
        db.SaveChanges();

        return new TodoService(db);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSeededTodos_OrderedByCreatedAt()
    {
        var service = CreateSeededService();

        var todos = (await service.GetAllAsync()).ToList();

        Assert.Equal(2, todos.Count);
        Assert.Equal("Set up backend skeleton", todos[0].Title);
        Assert.True(todos[0].IsComplete);
        Assert.Equal("Set up frontend skeleton", todos[1].Title);
        Assert.False(todos[1].IsComplete);
        Assert.NotNull(todos[1].DueDate);
    }

    [Fact]
    public async Task GetAllAsync_WithAllFilter_ReturnsAllSeededTodos()
    {
        var service = CreateSeededService();

        var todos = (await service.GetAllAsync(TodoStatusFilter.All)).ToList();

        Assert.Equal(2, todos.Count);
    }

    [Fact]
    public async Task GetAllAsync_WithPendingFilter_ReturnsOnlyIncompleteTodos()
    {
        var service = CreateSeededService();

        var todos = (await service.GetAllAsync(TodoStatusFilter.Pending)).ToList();

        Assert.Single(todos);
        Assert.All(todos, t => Assert.False(t.IsComplete));
    }

    [Fact]
    public async Task GetAllAsync_WithCompletedFilter_ReturnsOnlyCompleteTodos()
    {
        var service = CreateSeededService();

        var todos = (await service.GetAllAsync(TodoStatusFilter.Completed)).ToList();

        Assert.Single(todos);
        Assert.All(todos, t => Assert.True(t.IsComplete));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTodo_WhenIdExists()
    {
        var service = CreateSeededService();

        var todo = await service.GetByIdAsync(CompleteSeedId);

        Assert.NotNull(todo);
        Assert.Equal(CompleteSeedId, todo!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenIdDoesNotExist()
    {
        var service = CreateSeededService();

        var todo = await service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(todo);
    }

    [Fact]
    public async Task CreateAsync_AddsRetrievableTodo_WithGeneratedIdAndIncomplete()
    {
        var service = CreateSeededService();
        var dueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3));

        var created = await service.CreateAsync("Write tests", dueDate);

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("Write tests", created.Title);
        Assert.Equal(dueDate, created.DueDate);
        Assert.False(created.IsComplete);

        var fetched = await service.GetByIdAsync(created.Id);
        Assert.NotNull(fetched);
        Assert.Equal(created.Title, fetched!.Title);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFields_WhenIdExists()
    {
        var service = CreateSeededService();
        var newDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));

        var updated = await service.UpdateAsync(CompleteSeedId, "Updated title", newDueDate, true);

        Assert.NotNull(updated);
        Assert.Equal("Updated title", updated!.Title);
        Assert.Equal(newDueDate, updated.DueDate);
        Assert.True(updated.IsComplete);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenIdDoesNotExist()
    {
        var service = CreateSeededService();

        var updated = await service.UpdateAsync(Guid.NewGuid(), "Does not matter", null, false);

        Assert.Null(updated);
    }

    [Fact]
    public async Task DeleteAsync_RemovesTodo_WhenIdExists()
    {
        var service = CreateSeededService();

        var deleted = await service.DeleteAsync(CompleteSeedId);

        Assert.True(deleted);
        Assert.Null(await service.GetByIdAsync(CompleteSeedId));
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenIdDoesNotExist()
    {
        var service = CreateSeededService();

        var deleted = await service.DeleteAsync(Guid.NewGuid());

        Assert.False(deleted);
    }
}
