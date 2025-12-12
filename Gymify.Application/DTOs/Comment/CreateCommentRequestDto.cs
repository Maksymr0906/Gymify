using Gymify.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.DTOs.Comment
{
    public class CreateCommentRequestDto
    {
        public Guid TargetId { get; set; }
        public CommentTargetType TargetType { get; set; }

        [Required(ErrorMessage = "Required")]
        [MaxLength(30, ErrorMessage = "CommentMaxLength")]
        public string Content { get; set; }
    }
}
