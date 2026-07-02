using AutoService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Infrastructure.Configuration
{
    internal class ServisnaUslugaConfiguration : IEntityTypeConfiguration<ServisnaUsluga>
    {
        public void Configure(EntityTypeBuilder<ServisnaUsluga> builder)
        {
            builder.ToTable("ServisneUsluge");

            builder.HasKey(su => su.ServisnaUslugaId);

            builder.Property(su => su.Naziv)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(su => su.Opis)
                .HasMaxLength(500);

            builder.Property(su => su.Cena)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(su => su.TrajanjeUMinutima)
                .IsRequired();

            builder.Property(su => su.Aktivna)
                .HasDefaultValue(true);
        }
    }
}

