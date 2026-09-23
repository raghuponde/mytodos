using Microsoft.EntityFrameworkCore;
using MyTodos.Api.Models;

namespace MyTodos.Api.Data;

public class TodosDbContext : DbContext
{
    public TodosDbContext(DbContextOptions<TodosDbContext> options) : base(options)
    {
    }

    public DbSet<Todo> Todos => Set<Todo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>().HasData(
            new Todo
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Title = "Set up backend skeleton",
                DueDate = null,
                IsComplete = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new Todo
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Title = "Set up frontend skeleton",
                DueDate = new DateOnly(2026, 1, 2),
                IsComplete = false,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 1, DateTimeKind.Utc),
            });
    }
}
