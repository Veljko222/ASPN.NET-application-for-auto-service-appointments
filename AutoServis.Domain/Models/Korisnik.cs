using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Domain.Models
{
    public class Korisnik
    {
        public int KorisnikId { get; set; }

        public string Ime { get; set; } = string.Empty;

        public string Prezime { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Telefon { get; set; } = string.Empty;

        public ICollection<Vozilo> Vozila { get; set; } = new List<Vozilo>();
    }
}
