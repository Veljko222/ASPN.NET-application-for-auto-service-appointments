using System.ComponentModel.DataAnnotations;

namespace AutoService.Application.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Ime je obavezno.")]
        [MaxLength(50)]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno.")]
        [MaxLength(50)]
        public string Prezime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon je obavezan.")]
        [MaxLength(30)]
        public string Telefon { get; set; } = string.Empty;

        [Required(ErrorMessage = "Korisnicko ime je obavezno.")]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu.")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [MinLength(6, ErrorMessage = "Lozinka mora imati najmanje 6 karaktera.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrdite lozinku.")]
        [Compare(nameof(Password), ErrorMessage = "Lozinke se ne poklapaju.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Marka je obavezna.")]
        [MaxLength(50)]
        public string Marka { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model je obavezan.")]
        [MaxLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Godina proizvodnje je obavezna.")]
        [Range(1950, 2100, ErrorMessage = "Godina proizvodnje nije validna.")]
        [Display(Name = "Godina proizvodnje")]
        public int GodinaProizvodnje { get; set; }

        [Required(ErrorMessage = "Registracija je obavezna.")]
        [MaxLength(20)]
        public string Registracija { get; set; } = string.Empty;
    }
}
