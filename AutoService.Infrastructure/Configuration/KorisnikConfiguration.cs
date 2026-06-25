using AutoService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Infrastructure.Configuration
{
    internal class KorisnikConfiguration : IEntityTypeConfiguration<Korisnik>
    {
        public void Configure(EntityTypeBuilder<Korisnik> builder)
        {
            builder.ToTable("Korisnici");

            builder.HasKey(k => k.KorisnikId);

            builder.Property(k => k.Ime)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(k => k.Prezime)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(k => k.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(k => k.Telefon)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(k => k.Email)
                .IsUnique();
        }
    }
}
