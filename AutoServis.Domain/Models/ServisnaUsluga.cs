using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Domain.Models
{
    public class ServisnaUsluga
    {
        public int ServisnaUslugaId { get; set; }

        public string Naziv { get; set; } = string.Empty;

        public string Opis { get; set; } = string.Empty;

        public decimal Cena { get; set; }

        public int TrajanjeUMinutima { get; set; }

        public bool Aktivna { get; set; } = true;

        public ICollection<Termin> Termini { get; set; } = new List<Termin>();
    }
}

