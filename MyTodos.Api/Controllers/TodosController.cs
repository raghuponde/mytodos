using Microsoft.AspNetCore.Mvc;
using MyTodos.Api.Models;
using MyTodos.Api.Services;

namespace MyTodos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string status = "all")
    {
        TodoStatusFilter? filter = status.Trim().ToLowerInvariant() switch
        {
            "all" => TodoStatusFilter.All,
            "pending" => TodoStatusFilter.Pending,
            "completed" => TodoStatusFilter.Completed,
            _ => null,
        };

        if (filter is null)
        {
            return BadRequest(new { message = "Invalid status. Valid values: 'pending', 'completed', 'all'." });
        }

        var todos = await _todoService.GetAllAsync(filter.Value);
        return Ok(todos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var todo = await _todoService.GetByIdAsync(id);
        return todo is null ? NotFound() : Ok(todo);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTodoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new { message = "Title is required." });
        }

        var todo = await _todoService.CreateAsync(request.Title.Trim(), request.DueDate);
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTodoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new { message = "Title is required." });
        }

        var todo = await _todoService.UpdateAsync(id, request.Title.Trim(), request.DueDate, request.IsComplete);
        return todo is null ? NotFound() : Ok(todo);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _todoService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
