using Gymify.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Data.Entities;
public class Chat : BaseEntity
{
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
    public ChatType Type { get; set; }
    public Guid? LastMessageId { get; set; }
    public Message? LastMessage { get; set; }
    public ICollection<UserChat> Members { get; set; } = new List<UserChat>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
