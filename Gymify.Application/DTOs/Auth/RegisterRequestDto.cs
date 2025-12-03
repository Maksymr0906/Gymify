using System.ComponentModel.DataAnnotations;

namespace Gymify.Application.DTOs.Auth
{ 

    public class RegisterRequestDto
    {

        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "EmailInvalid")]
        [Display(Name = "Email")] 
        public string Email { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "UserNameLength")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "PasswordLength")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "PasswordMismatch")]
        [Display(Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}
