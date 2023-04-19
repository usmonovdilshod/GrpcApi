using Grpc.Core;
using Shared;

namespace GrpcApi
{
    public class CounterService : ICounter
    {
        protected readonly CounterStateStorageService storage;

        public CounterService(CounterStateStorageService storage)
        {
            this.storage = storage;
        }

        public Task Decrement()
        {
            var currentState = storage.GetCounterState();

            currentState.Value.Count--;

            return storage.SetCounterState(currentState.Value);
        }

        public Task Increment()
        {
            var currentState = storage.GetCounterState();

            currentState.Value.Count++;

            return storage.SetCounterState(currentState.Value);
        }

        public IAsyncEnumerable<CounterState> SubscribeAsync()
        {
            var subj = storage.GetCounterState();
            return subj.ToAsyncEnumerable();
        }   

        
        public Task ThrowException()
        {
            var rng = new Random();
            object _ = rng.Next(0, 7) switch
            {
                0 => throw new Exception("Exception"),
                1 => throw new ArgumentException("Argument exception"),
                2 => throw new ArithmeticException("Arithmetic exception"),
                3 => throw new IndexOutOfRangeException("IndexOutOfRange exception"),
                4 => throw new RpcException(new Status(StatusCode.AlreadyExists, "Example AlreadyExists RpcException")),
                5 => throw new RpcException(new Status(StatusCode.InvalidArgument, "Example InvalidArgument RpcException")),
                _ => throw new RpcException(new Status(StatusCode.FailedPrecondition, "Example FailedPrecondition RpcException")),
            };

            return Task.CompletedTask;
        }
    }
}
