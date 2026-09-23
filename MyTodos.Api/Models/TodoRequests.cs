using System.ComponentModel.DataAnnotations;

namespace MyTodos.Api.Models;

public class CreateTodoRequest
{
    [Required(AllowEmptyStrings = false)]
    public string Title { get; set; } = string.Empty;

    public DateOnly? DueDate { get; set; }
}

public class UpdateTodoRequest
{
    [Required(AllowEmptyStrings = false)]
    public string Title { get; set; } = string.Empty;

    public DateOnly? DueDate { get; set; }

    public bool IsComplete { get; set; }
}
