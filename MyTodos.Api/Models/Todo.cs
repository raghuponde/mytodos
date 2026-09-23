namespace MyTodos.Api.Models;

public class Todo
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly? DueDate { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CreatedAt { get; set; }
}
