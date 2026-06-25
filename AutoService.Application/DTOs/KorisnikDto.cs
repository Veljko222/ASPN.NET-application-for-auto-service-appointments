using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AutoService.Application.DTOs
{
    public class KorisnikDto
    {
        public int KorisnikId { get; set; }

        [Required(ErrorMessage = "Ime je obavezno.")]
        [MaxLength(50)]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno.")]
        [MaxLength(50)]
        public string Prezime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Email nije u ispravnom formatu.")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon je obavezan.")]
        [MaxLength(30)]
        public string Telefon { get; set; } = string.Empty;
    }
}
