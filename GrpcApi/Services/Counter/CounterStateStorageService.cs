using Shared;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace GrpcApi
{
    public class CounterStateStorageService
    {
        protected readonly IServiceProvider serviceProvider;
        public ConcurrentDictionary<string, BehaviorSubject<CounterState>> States { get; set; } = new();

        string userId = "test";

        public CounterStateStorageService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public BehaviorSubject<CounterState> GetCounterState()
        {
            var counter = GetSetPersistedCounterState();

            lock (this)
            {
                if (!States.ContainsKey(userId))
                {
                    States[userId] = new BehaviorSubject<CounterState>(
                        new CounterState
                        {
                            Count = counter.Count,
                        });
                }
            }

            return States[userId];
        }

        public Task SetCounterState(CounterState state)
        {

            lock (this)
            {
                if (States.ContainsKey(userId))
                {
                    States[userId].OnNext(state);
                }
                else
                {
                    States[userId] = new BehaviorSubject<CounterState>(state);
                }
            }

            GetSetPersistedCounterState(state.Count);

            return Task.CompletedTask;
        }

        private Counter GetSetPersistedCounterState(int? newState = null)
        {
            lock (this)
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetService<MyDbContext>();
                var counter = context?.Counters.FirstOrDefault(x => x.Id.Equals(1));
                if (counter == null)
                {
                    counter = new Counter { Id = 1, Count = 1 };
                    context.Add(counter);
                    context.SaveChanges();
                }

                if (newState.HasValue)
                {
                    counter.Count = newState.Value;
                    context.SaveChanges();
                }

                return counter;
            }
        }
    }
}
