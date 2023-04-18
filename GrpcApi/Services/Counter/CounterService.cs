using Shared;
using System.Collections.Concurrent;
using System.Reactive.Subjects;

namespace GrpcApi
{
    public class CounterService : ICounter
    {
        public Task Decrement()
        {
            throw new NotImplementedException();
        }

        public Task Increment()
        {
            throw new NotImplementedException();
        }

        public ConcurrentDictionary<string, BehaviorSubject<CounterState>> States { get; set; } = new();

        public IAsyncEnumerable<CounterState> SubscribeAsync()
        {
            var subj = GetCounterState();
            return subj.ToAsyncEnumerable();
        }

        public BehaviorSubject<CounterState> GetCounterState()
        {
            var rng = new Random();
            BehaviorSubject<CounterState> counter = new BehaviorSubject<CounterState>(
                        new CounterState
                        {
                            Count = rng.Next()
                        });

            lock (this)
            {
                if (!States.ContainsKey("test"))
                {
                    States["test"] = new BehaviorSubject<CounterState>(
                        new CounterState
                        {
                            Count = counter.Value.Count,
                        });
                }
            }

            return States["test"];
        }


        public Task ThrowException()
        {
            throw new NotImplementedException();
        }
    }
}
