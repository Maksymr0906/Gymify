using Gymify.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Data.Interfaces.Repositories;

public interface IMessageReadStatusRepository
{
    Task CreateAsync(MessageReadStatus status);
    Task<bool> IsReadAsync(Guid messageId, Guid userId);
}
