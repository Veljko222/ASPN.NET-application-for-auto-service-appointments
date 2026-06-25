using AutoService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            AutoServiceDbContext context)
        {
            await context.Database.MigrateAsync();

            if (await context.Korisnici.AnyAsync())
            {
                return;
            }

            var korisnik1 = new Korisnik
            {
                Ime = "Marko",
                Prezime = "Marković",
                Email = "marko@gmail.com",
                Telefon = "061111111"
            };

            var korisnik2 = new Korisnik
            {
                Ime = "Jovan",
                Prezime = "Jovanović",
                Email = "jovan@gmail.com",
                Telefon = "062222222"
            };

            await context.Korisnici.AddRangeAsync(
                korisnik1,
                korisnik2);

            await context.SaveChangesAsync();

            var vozila = new List<Vozilo>
            {
                new Vozilo
                {
                    Marka = "Volkswagen",
                    Model = "Golf 7",
                    GodinaProizvodnje = 2017,
                    Registracija = "PA-123-AA",
                    KorisnikId = korisnik1.KorisnikId
                },
                new Vozilo
                {
                    Marka = "BMW",
                    Model = "320d",
                    GodinaProizvodnje = 2019,
                    Registracija = "BG-456-BB",
                    KorisnikId = korisnik2.KorisnikId
                }
            };

            var serviseri = new List<Serviser>
            {
                new Serviser
                {
                    Ime = "Nikola",
                    Prezime = "Nikolić",
                    Specijalizacija = "Mehanika",
                    Aktivan = true
                },
                new Serviser
                {
                    Ime = "Milan",
                    Prezime = "Milić",
                    Specijalizacija = "Auto-elektrika",
                    Aktivan = true
                }
            };

            var usluge = new List<ServisnaUsluga>
            {
                new ServisnaUsluga
                {
                    Naziv = "Mali servis",
                    Opis = "Zamena ulja i filtera.",
                    Cena = 12000,
                    TrajanjeUMinutima = 60,
                    Aktivna = true
                },
                new ServisnaUsluga
                {
                    Naziv = "Veliki servis",
                    Opis = "Zamena zupčastog kaiša i pratećih delova.",
                    Cena = 45000,
                    TrajanjeUMinutima = 180,
                    Aktivna = true
                },
                new ServisnaUsluga
                {
                    Naziv = "Dijagnostika",
                    Opis = "Računarska dijagnostika vozila.",
                    Cena = 3000,
                    TrajanjeUMinutima = 30,
                    Aktivna = true
                },
                new ServisnaUsluga
                {
                    Naziv = "Zamena kočionih pločica",
                    Opis = "Zamena prednjih ili zadnjih pločica.",
                    Cena = 8000,
                    TrajanjeUMinutima = 60,
                    Aktivna = true
                }
            };

            await context.Vozila.AddRangeAsync(vozila);
            await context.Serviseri.AddRangeAsync(serviseri);
            await context.ServisneUsluge.AddRangeAsync(usluge);

            await context.SaveChangesAsync();
        }
    }
}
