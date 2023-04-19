using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace Shared
{
    [ProtoContract]
    public class TodoState
    {
        [Key]
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public string? Name { get; set; }
    }
}
