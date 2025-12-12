using Gymify.Application.DTOs.Notification;
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
    public bool UkranianVer { get; set; }
    public IEnumerable<NotificationDto> Notifications { get; set; }
}
