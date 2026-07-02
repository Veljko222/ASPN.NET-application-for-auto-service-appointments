using AutoService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Infrastructure.Configuration
{
    internal class TerminConfiguration : IEntityTypeConfiguration<Termin>
    {
        public void Configure(EntityTypeBuilder<Termin> builder)
        {
            builder.ToTable("Termini");

            builder.HasKey(t => t.TerminId);

            builder.Property(t => t.DatumIVreme)
                .IsRequired();

            builder.Property(t => t.Napomena)
                .HasMaxLength(500);

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.HasOne(t => t.Vozilo)
                .WithMany(v => v.Termini)
                .HasForeignKey(t => t.VoziloId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Serviser)
                .WithMany(s => s.Termini)
                .HasForeignKey(t => t.ServiserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.ServisnaUsluga)
                .WithMany(su => su.Termini)
                .HasForeignKey(t => t.ServisnaUslugaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => new
            {
                t.ServiserId,
                t.DatumIVreme
            });
        }
    }
}

