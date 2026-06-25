using AutoService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Infrastructure.Configuration
{
    internal class ServiserConfiguration : IEntityTypeConfiguration<Serviser>
    {
        public void Configure(EntityTypeBuilder<Serviser> builder)
        {
            builder.ToTable("Serviseri");

            builder.HasKey(s => s.ServiserId);

            builder.Property(s => s.Ime)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Prezime)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Specijalizacija)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Aktivan)
                .HasDefaultValue(true);
        }
    }
}
