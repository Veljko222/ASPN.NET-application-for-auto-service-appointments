using AutoService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Infrastructure.Configuration
{
    internal class VlasnikConfiguration : IEntityTypeConfiguration<Vlasnik>
    {
        public void Configure(EntityTypeBuilder<Vlasnik> builder)
        {
            builder.ToTable("Vlasnici");

            builder.HasKey(k => k.VlasnikId);

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

            builder.Property(k => k.UserName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(k => k.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(k => k.Email)
                .IsUnique();

            builder.HasIndex(k => k.UserName)
                .IsUnique();
        }
    }
}

