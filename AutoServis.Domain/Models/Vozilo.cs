using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Domain.Models
{
    public class Vozilo
    {
        public int VoziloId { get; set; }

        public string Marka { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public int GodinaProizvodnje { get; set; }

        public string Registracija { get; set; } = string.Empty;

        public int VlasnikId { get; set; }

        public Vlasnik Vlasnik { get; set; } = null!;

        public ICollection<Termin> Termini { get; set; } = new List<Termin>();
    }
}

