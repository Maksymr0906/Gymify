using System.ComponentModel.DataAnnotations;

namespace Gymify.Application.DTOs.Workout
{
    public class UpdateWorkoutRequestDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Conclusion { get; set; }

        public bool IsPrivate { get; set; }
    }
}