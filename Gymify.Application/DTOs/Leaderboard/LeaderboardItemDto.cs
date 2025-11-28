using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.DTOs.Leaderboard;

public class LeaderboardItemDto
{
    public int Rank { get; set; } // Порядковий номер (1, 2, 3...)
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string AvatarUrl { get; set; }
    public int Level { get; set; }
    public long TotalXP { get; set; }
    public bool IsMe { get; set; }
    public bool IsFriend { get; set; }
}
