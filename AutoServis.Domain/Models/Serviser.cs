using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Domain.Models
{
    public class Serviser
    {
        public int ServiserId { get; set; }

        public string Ime { get; set; } = string.Empty;

        public string Prezime { get; set; } = string.Empty;

        public string Specijalizacija { get; set; } = string.Empty;

        public bool Aktivan { get; set; } = true;

        public ICollection<Termin> Termini { get; set; } = new List<Termin>();
    }
}
