using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AutoService.Application.DTOs
{
    public class VoziloDto
    {
        public int VoziloId { get; set; }

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

        [Required(ErrorMessage = "Izaberite vlasnika vozila.")]
        [Display(Name = "Vlasnik")]
        public int VlasnikId { get; set; }
    }
}

