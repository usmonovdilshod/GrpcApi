using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace Shared
{
    [ProtoContract]
    public class TodoState
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public string? Name { get; set; }
    }

    public class TodoEntity
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    
}
