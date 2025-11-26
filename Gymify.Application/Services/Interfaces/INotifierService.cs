using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.Services.Interfaces
{
    public interface INotifierService
    {
        Task PushAsync(Guid userId, string method, object data);
    }
}
