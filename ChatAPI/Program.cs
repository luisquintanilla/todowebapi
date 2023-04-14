using Microsoft.Extensions.FileProviders;
using StackExchange.Redis;
using System.Text.Json;

var redis = ConnectionMultiplexer.Connect("redis");
var db = redis.GetDatabase();

public record TodoItem(int Id, string Text);

using Microsoft.AspNetCore.Builder;  
using Microsoft.AspNetCore.Http;  
using Microsoft.AspNetCore.Mvc;  
using Microsoft.Extensions.DependencyInjection;  
using System.Collections.Generic;  
using System.Linq;  
  
var todos = new List<TodoItem>();

var app = WebApplication.Create();

app.MapGet("/todos", () =>
{
    var items = db.ListRange("todos").Select(x => JsonSerializer.Deserialize<TodoItem>(x)).ToList();
    return items;
});

app.MapGet("/todos/{id:int}", (int id) =>
{
    var item = db.ListGetByIndex("todos", id);
    if (item.IsNull)
    {
        return Results.NotFound();
    }
    return JsonSerializer.Deserialize<TodoItem>(item);
});

app.MapPost("/todos", async (HttpContext context) =>
{
    var item = await context.Request.ReadFromJsonAsync<TodoItem>();
    item.Id = todos.Count + 1;
    todos.Add(item);
    db.ListRightPush("todos", JsonSerializer.Serialize(item));
    return Results.Created($"/todos/{item.Id}", item);
});

app.MapDelete("/todos/{id:int}", (int id) =>
{
    var item = db.ListGetByIndex("todos", id);
    if (item.IsNull)
    {
        return Results.NotFound();
    }
    db.ListRemove("todos", item);
    return Results.NoContent();
});

app.Run();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), ".well-known")),
    RequestPath = "/.well-known",
    ServeUnknownFileTypes = true
});
