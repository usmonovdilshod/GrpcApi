using ProtoBuf;

namespace Shared
{
    [ProtoContract]
    public class CounterState
    {
        [ProtoMember(1)]
        public int Count { get; set; }
    }
}
