using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AutoService.Application.DTOs
{
    public class VlasnikDto
    {
        public int VlasnikId { get; set; }

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

        [Required(ErrorMessage = "Korisničko ime je obavezno.")]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}

