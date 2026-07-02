using AutoService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Domain.Models
{
    public class Termin
    {
        public int TerminId { get; set; }

        public DateTime DatumIVreme { get; set; }

        public string Napomena { get; set; } = string.Empty;

        public StatusTermina Status { get; set; } = StatusTermina.Zakazan;

        public int VoziloId { get; set; }

        public Vozilo Vozilo { get; set; } = null!;

        public int ServiserId { get; set; }

        public Serviser Serviser { get; set; } = null!;

        public int ServisnaUslugaId { get; set; }

        public ServisnaUsluga ServisnaUsluga { get; set; } = null!;
    }
}

