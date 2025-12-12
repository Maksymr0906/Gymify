using Gymify.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.DTOs.Comment
{
    public class CreateMessageRequestDto
    {
        [Required]
        public Guid ChatId { get; set; }

        [Required(ErrorMessage = "Message cannot be empty.")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 1000 characters.")]
        public string Content { get; set; }
    }
}
