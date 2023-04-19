using ProtoBuf.Grpc.Configuration;

namespace Shared
{
    [Service("GrpcApi.Shared.Todo")]
    public interface ITodo
    {
        Task<List<TodoState>> GetAll();
        Task Create(TodoState todo);
        IAsyncEnumerable<TodoState> SubscribeAsync();
    }
}
