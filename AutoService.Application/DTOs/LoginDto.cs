using System.ComponentModel.DataAnnotations;

namespace AutoService.Application.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}

