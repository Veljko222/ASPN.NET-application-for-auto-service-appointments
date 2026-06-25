using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AutoService.Application.DTOs
{
    public class ZakaziTerminDto
    {
        [Required(ErrorMessage = "Izaberite vozilo.")]
        [Display(Name = "Vozilo")]
        public int VoziloId { get; set; }

        [Required(ErrorMessage = "Izaberite servisera.")]
        [Display(Name = "Serviser")]
        public int ServiserId { get; set; }

        [Required(ErrorMessage = "Izaberite servisnu uslugu.")]
        [Display(Name = "Servisna usluga")]
        public int ServisnaUslugaId { get; set; }

        [Required(ErrorMessage = "Unesite datum i vreme.")]
        [Display(Name = "Datum i vreme")]
        public DateTime DatumIVreme { get; set; }

        [MaxLength(500)]
        public string Napomena { get; set; } = string.Empty;
    }
}
