using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

//Add .well-known folder to serve the .well-known folder
app.UseStaticFiles();

// Initialize Todos
var todos = new List<TodoItem>();

// Get all todos
app.MapGet("/todos", () => todos);

// Get a single todo
app.MapGet("/todos/{id}", (int id) => todos.FirstOrDefault(t => t.id == id));

// Add todo
app.MapPost("/todos", (TodoItem todo) =>
{
    todos.Add(todo);
    return Results.Created($"/todos/{todo.id}", todo);
});

// Delete todo
app.MapDelete("/todos/{id}", (int id) =>
{
    var todoItem = todos.FirstOrDefault(t => t.id == id);
    if (todoItem is null)
    {
        return Results.NotFound();
    }
    todos.Remove(todoItem);
    return Results.Ok();
}); 

app.Run();


internal record TodoItem(int id, string todo);