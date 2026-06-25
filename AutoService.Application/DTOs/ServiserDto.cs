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
    }
}
