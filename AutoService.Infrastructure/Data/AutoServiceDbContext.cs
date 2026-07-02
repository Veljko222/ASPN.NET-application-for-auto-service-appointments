using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using AutoService.Domain.Models;
namespace AutoService.Infrastructure.Data
{ 
        public class AutoServiceDbContext : DbContext
        {
            public AutoServiceDbContext(
                DbContextOptions<AutoServiceDbContext> options)
                : base(options)
            {
            }

            public DbSet<Vlasnik> Vlasnici { get; set; }

            public DbSet<Vozilo> Vozila { get; set; }

            public DbSet<Serviser> Serviseri { get; set; }

            public DbSet<ServisnaUsluga> ServisneUsluge { get; set; }

            public DbSet<Termin> Termini { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.ApplyConfigurationsFromAssembly(
                    typeof(AutoServiceDbContext).Assembly);
            }
        }
}

