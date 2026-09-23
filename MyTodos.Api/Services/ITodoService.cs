using MyTodos.Api.Models;

namespace MyTodos.Api.Services;

public interface ITodoService
{
    Task<IEnumerable<Todo>> GetAllAsync();
    Task<Todo?> GetByIdAsync(Guid id);
    Task<Todo> CreateAsync(string title, DateOnly? dueDate);
    Task<Todo?> UpdateAsync(Guid id, string title, DateOnly? dueDate, bool isComplete);
    Task<bool> DeleteAsync(Guid id);
}
