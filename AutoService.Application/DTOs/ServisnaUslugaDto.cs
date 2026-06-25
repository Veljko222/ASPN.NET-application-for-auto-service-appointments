using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AutoService.Application.DTOs
{
    public class ServisnaUslugaDto
    {
        public int ServisnaUslugaId { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan.")]
        [MaxLength(100)]
        public string Naziv { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Opis { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cena je obavezna.")]
        [Range(1, 1000000, ErrorMessage = "Cena mora biti veća od nule.")]
        public decimal Cena { get; set; }

        [Required(ErrorMessage = "Trajanje je obavezno.")]
        [Range(15, 1440, ErrorMessage = "Trajanje mora biti između 15 i 1440 minuta.")]
        [Display(Name = "Trajanje u minutima")]
        public int TrajanjeUMinutima { get; set; }

        public bool Aktivna { get; set; } = true;
    }
}
