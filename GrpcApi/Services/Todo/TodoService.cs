using Microsoft.EntityFrameworkCore;
using Shared;
using System.Collections.Concurrent;
using System.Reactive.Subjects;

namespace GrpcApi.Services.Todo
{
    public class TodoService : ITodo
    {
        protected readonly IServiceProvider serviceProvider;
        protected readonly IServiceScope scope;
        static ConcurrentDictionary<string, BehaviorSubject<TodoState>> States { get; set; } = new();
        string key = "test";

        public TodoService(IServiceProvider serviceProvider)
        {
            this.serviceProvider=serviceProvider;
            this.scope = serviceProvider.CreateScope();
        }
        public Task<List<TodoState>> GetAll()
        {
            var context = scope.ServiceProvider.GetService<MyDbContext>();
            return context.Todos.ToListAsync();
        }
        public Task Create(TodoState todo)
        {
            lock (this)
            {
                if (States.ContainsKey(key))
                {
                    States[key].OnNext(todo);
                }
                else
                {
                    States[key] = new BehaviorSubject<TodoState>(todo);
                }
            }

            GetSetTodoState(todo);
            return Task.CompletedTask;
        }

        public IAsyncEnumerable<TodoState> SubscribeAsync()
        {
            var todo = GetSetTodoState();

            lock (this)
            {
                if (!States.ContainsKey(key))
                {
                    States[key] = new BehaviorSubject<TodoState>(
                        new TodoState
                        {
                            Id = todo.Id,
                            Name = todo.Name,
                        });
                }
            }
             
            return States[key].ToAsyncEnumerable();
        }

        private TodoState GetSetTodoState(TodoState? newState = null)
        {
            lock (this)
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetService<MyDbContext>();
                
                if (newState != null)
                {
                    context.Add(newState);
                    context.SaveChanges();
                }
                var todo = context?.Todos.Last();

                return todo;
            }
        }

    }
}
