using AutoService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Infrastructure.Configuration
{
    internal class VoziloConfiguration : IEntityTypeConfiguration<Vozilo>
    {
        public void Configure(EntityTypeBuilder<Vozilo> builder)
        {
            builder.ToTable("Vozila");

            builder.HasKey(v => v.VoziloId);

            builder.Property(v => v.Marka)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Registracija)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(v => v.Registracija)
                .IsUnique();

            builder.HasOne(v => v.Korisnik)
                .WithMany(k => k.Vozila)
                .HasForeignKey(v => v.KorisnikId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
