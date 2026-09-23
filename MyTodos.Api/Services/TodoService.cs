using System.Collections.Concurrent;
using MyTodos.Api.Models;

namespace MyTodos.Api.Services;

public class TodoService : ITodoService
{
    private readonly ConcurrentDictionary<Guid, Todo> _todos = new();

    public TodoService()
    {
        var seed1 = new Todo
        {
            Id = Guid.NewGuid(),
            Title = "Set up backend skeleton",
            IsComplete = true,
            CreatedAt = DateTime.UtcNow,
        };
        var seed2 = new Todo
        {
            Id = Guid.NewGuid(),
            Title = "Set up frontend skeleton",
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            IsComplete = false,
            CreatedAt = DateTime.UtcNow,
        };
        _todos[seed1.Id] = seed1;
        _todos[seed2.Id] = seed2;
    }

    public Task<IEnumerable<Todo>> GetAllAsync()
    {
        var todos = _todos.Values.OrderBy(t => t.CreatedAt).AsEnumerable();
        return Task.FromResult(todos);
    }

    public Task<Todo?> GetByIdAsync(Guid id)
    {
        _todos.TryGetValue(id, out var todo);
        return Task.FromResult(todo);
    }

    public Task<Todo> CreateAsync(string title, DateOnly? dueDate)
    {
        var todo = new Todo
        {
            Id = Guid.NewGuid(),
            Title = title,
            DueDate = dueDate,
            IsComplete = false,
            CreatedAt = DateTime.UtcNow,
        };
        _todos[todo.Id] = todo;
        return Task.FromResult(todo);
    }

    public Task<Todo?> UpdateAsync(Guid id, string title, DateOnly? dueDate, bool isComplete)
    {
        if (!_todos.TryGetValue(id, out var todo))
        {
            return Task.FromResult<Todo?>(null);
        }

        todo.Title = title;
        todo.DueDate = dueDate;
        todo.IsComplete = isComplete;
        return Task.FromResult<Todo?>(todo);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(_todos.TryRemove(id, out _));
    }
}
