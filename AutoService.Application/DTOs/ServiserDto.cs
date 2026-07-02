using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AutoService.Application.DTOs
{
    public class ServiserDto
    {
        public int ServiserId { get; set; }

        [Required(ErrorMessage = "Ime je obavezno.")]
        [MaxLength(50)]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno.")]
        [MaxLength(50)]
        public string Prezime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Specijalizacija je obavezna.")]
        [MaxLength(100)]
        public string Specijalizacija { get; set; } = string.Empty;

        public bool Aktivan { get; set; } = true;

        public bool IsAdmin { get; set; }

        [MaxLength(50)]
        public string? UserName { get; set; }

        [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu.")]
        [MaxLength(100)]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Lozinka mora imati najmanje 6 karaktera.")]
        public string? Password { get; set; }
    }
}

