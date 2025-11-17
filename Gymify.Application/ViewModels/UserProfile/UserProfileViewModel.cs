using Gymify.Application.DTOs.Achievement;
using Gymify.Application.DTOs.UserEquipment;
using Gymify.Application.DTOs.Workout;
using Gymify.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.ViewModels.UserProfile;

public class UserProfileViewModel
{
    public string UserName { get; set; } = "Name";
    public string Title { get; set; } = "Title";
    public List<AchievementDto> Achievements { get; set; }
    public List<WorkoutDto> Workouts { get; set; }
    public UserEquipmentDto UserEquipmentDto { get; set; }
    public UpdateUserEquipmentDto UpdateUserEquipmentDto { get; set; }
}
