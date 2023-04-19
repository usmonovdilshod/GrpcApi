using ProtoBuf;
using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class Counter
    {
        [Key]
        public int Id { get; set; }
        public int Count { get; set; }
    }
}
