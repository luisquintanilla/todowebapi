// Write a simple TODO app using Minimal ASP.NET, that lets the user add TODOs, list their TODOs, list specific TODOs, and delete TODOs, ensuring that the app stores Id for each todo item. 

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace CopilotAPI
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
    public class Todo
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
    public class TodoController : ControllerBase
    {
        private static ConcurrentDictionary<int, Todo> _todos = new ConcurrentDictionary<int, Todo>();
        [HttpGet]
        [Route("/todos")]
        public IEnumerable<Todo> Get()
        {
            return _todos.Values;
        }
        [HttpGet]
        [Route("/todos/{id}")]
        public Todo Get(int id)
        {
            if (_todos.TryGetValue(id, out Todo todo))
            {
                return todo;
            }
            else
            {
                return null;
            }
        }
        [HttpPost]
        [Route("/todos")]
        public void Post([FromBody]Todo todo)
        {
            todo.Id = _todos.Count + 1;
            _todos[todo.Id] = todo;
        }
        [HttpDelete]
        [Route("/todos/{id}")]
        public void Delete(int id)
        {
            _todos.TryRemove(id, out Todo todo);
        }
    }
}