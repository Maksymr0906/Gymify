using Gymify.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.ViewModels.NotificationBell;

public class NotificationBellViewModel
{
    public int UnreadCount { get; set; }
    public IEnumerable<Notification> Notifications { get; set; }
}
