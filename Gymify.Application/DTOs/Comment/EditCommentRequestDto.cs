using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.DTOs.Comment
{
    public class EditCommentRequestDto
    {
        public Guid CommentId { get; set; }

        [Required(ErrorMessage = "Required")]
        [MaxLength(30, ErrorMessage = "CommentMaxLength")]
        public string NewContent { get; set; }
    }
}
