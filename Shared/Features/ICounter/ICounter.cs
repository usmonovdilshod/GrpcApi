using ProtoBuf.Grpc.Configuration;

namespace Shared
{
    [Service("GrpcApi.Shared.Counter")]
    public interface ICounter
    {
        Task Increment();
        Task Decrement();
        Task ThrowException();

        IAsyncEnumerable<CounterState> SubscribeAsync();
    }
}
