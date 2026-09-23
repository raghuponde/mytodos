using Microsoft.EntityFrameworkCore;
using MyTodos.Api.Data;
using MyTodos.Api.Models;

namespace MyTodos.Api.Services;

public class TodoService : ITodoService
{
    private readonly TodosDbContext _db;

    public TodoService(TodosDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Todo>> GetAllAsync(TodoStatusFilter filter = TodoStatusFilter.All)
    {
        var query = _db.Todos.OrderBy(t => t.CreatedAt).AsQueryable();
        query = filter switch
        {
            TodoStatusFilter.Pending => query.Where(t => !t.IsComplete),
            TodoStatusFilter.Completed => query.Where(t => t.IsComplete),
            _ => query,
        };
        return await query.ToListAsync();
    }

    public async Task<Todo?> GetByIdAsync(Guid id)
    {
        return await _db.Todos.FindAsync(id);
    }

    public async Task<Todo> CreateAsync(string title, DateOnly? dueDate)
    {
        var todo = new Todo
        {
            Id = Guid.NewGuid(),
            Title = title,
            DueDate = dueDate,
            IsComplete = false,
            CreatedAt = DateTime.UtcNow,
        };
        _db.Todos.Add(todo);
        await _db.SaveChangesAsync();
        return todo;
    }

    public async Task<Todo?> UpdateAsync(Guid id, string title, DateOnly? dueDate, bool isComplete)
    {
        var todo = await _db.Todos.FindAsync(id);
        if (todo is null)
        {
            return null;
        }

        todo.Title = title;
        todo.DueDate = dueDate;
        todo.IsComplete = isComplete;
        await _db.SaveChangesAsync();
        return todo;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var todo = await _db.Todos.FindAsync(id);
        if (todo is null)
        {
            return false;
        }

        _db.Todos.Remove(todo);
        await _db.SaveChangesAsync();
        return true;
    }
}
