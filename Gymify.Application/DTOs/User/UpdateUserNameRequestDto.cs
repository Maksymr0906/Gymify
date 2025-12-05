using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gymify.Application.DTOs.User
{
    public class UpdateUserNameRequest
    {
        [Required(ErrorMessage = "Required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "UserNameLength")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "UsernameLettersNumbersUnderscores")]
        public string UpdatedUserName { get; set; }
    }
}
