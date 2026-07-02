using AutoService.Application.Auth;
using AutoService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoService.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            AutoServiceDbContext context,
            IPasswordHasher passwordHasher)
        {
            await context.Database.MigrateAsync();

            await EnsureServiserAdminColumnsAsync(context);
            await EnsureAdminServiserAsync(context, passwordHasher);
            await EnsureVlasnikLoginsAsync(context, passwordHasher);

            if (await context.Vlasnici.AnyAsync())
            {
                return;
            }

            var vlasnik1 = new Vlasnik
            {
                Ime = "Marko",
                Prezime = "MarkoviÄ‡",
                Email = "marko@gmail.com",
                Telefon = "061111111",
                UserName = "marko",
                PasswordHash = passwordHasher.Hash("Marko123!")
            };

            var vlasnik2 = new Vlasnik
            {
                Ime = "Jovan",
                Prezime = "JovanoviÄ‡",
                Email = "jovan@gmail.com",
                Telefon = "062222222",
                UserName = "jovan",
                PasswordHash = passwordHasher.Hash("Jovan123!")
            };

            await context.Vlasnici.AddRangeAsync(
                vlasnik1,
                vlasnik2);

            await context.SaveChangesAsync();

            var vozila = new List<Vozilo>
            {
                new Vozilo
                {
                    Marka = "Volkswagen",
                    Model = "Golf 7",
                    GodinaProizvodnje = 2017,
                    Registracija = "PA-123-AA",
                    VlasnikId = vlasnik1.VlasnikId
                },
                new Vozilo
                {
                    Marka = "BMW",
                    Model = "320d",
                    GodinaProizvodnje = 2019,
                    Registracija = "BG-456-BB",
                    VlasnikId = vlasnik2.VlasnikId
                }
            };

            var serviseri = new List<Serviser>
            {
                new Serviser
                {
                    Ime = "Milan",
                    Prezime = "MiliÄ‡",
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
                    Opis = "Zamena zupÄastog kaiÅ¡a i prateÄ‡ih delova.",
                    Cena = 45000,
                    TrajanjeUMinutima = 180,
                    Aktivna = true
                },
                new ServisnaUsluga
                {
                    Naziv = "Dijagnostika",
                    Opis = "RaÄunarska dijagnostika vozila.",
                    Cena = 3000,
                    TrajanjeUMinutima = 30,
                    Aktivna = true
                },
                new ServisnaUsluga
                {
                    Naziv = "Zamena koÄionih ploÄica",
                    Opis = "Zamena prednjih ili zadnjih ploÄica.",
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

        private static async Task EnsureServiserAdminColumnsAsync(
            AutoServiceDbContext context)
        {
            await context.Database.ExecuteSqlRawAsync(@"
IF COL_LENGTH('Serviseri', 'IsAdmin') IS NULL
BEGIN
    ALTER TABLE [Serviseri] ADD [IsAdmin] bit NOT NULL CONSTRAINT [DF_Serviseri_IsAdmin] DEFAULT CAST(0 AS bit);
END");

            await context.Database.ExecuteSqlRawAsync(@"
IF COL_LENGTH('Serviseri', 'UserName') IS NULL
BEGIN
    ALTER TABLE [Serviseri] ADD [UserName] nvarchar(50) NULL;
END");

            await context.Database.ExecuteSqlRawAsync(@"
IF COL_LENGTH('Serviseri', 'Email') IS NULL
BEGIN
    ALTER TABLE [Serviseri] ADD [Email] nvarchar(100) NULL;
END");

            await context.Database.ExecuteSqlRawAsync(@"
IF COL_LENGTH('Serviseri', 'PasswordHash') IS NULL
BEGIN
    ALTER TABLE [Serviseri] ADD [PasswordHash] nvarchar(500) NULL;
END");

            await context.Database.ExecuteSqlRawAsync(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Serviseri_UserName'
      AND object_id = OBJECT_ID('Serviseri')
)
BEGIN
    CREATE UNIQUE INDEX [IX_Serviseri_UserName]
    ON [Serviseri] ([UserName])
    WHERE [UserName] IS NOT NULL;
END");

            await context.Database.ExecuteSqlRawAsync(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Serviseri_Email'
      AND object_id = OBJECT_ID('Serviseri')
)
BEGIN
    CREATE UNIQUE INDEX [IX_Serviseri_Email]
    ON [Serviseri] ([Email])
    WHERE [Email] IS NOT NULL;
END");
        }

        private static async Task EnsureVlasnikLoginsAsync(
            AutoServiceDbContext context,
            IPasswordHasher passwordHasher)
        {
            string defaultPasswordHash = passwordHasher.Hash("Vlasnik123!");
            string markoPasswordHash = passwordHasher.Hash("Marko123!");
            string jovanPasswordHash = passwordHasher.Hash("Jovan123!");

            await context.Database.ExecuteSqlInterpolatedAsync($@"
UPDATE Vlasnici
SET UserName = LOWER(LEFT(Email, CHARINDEX('@', Email + '@') - 1))
WHERE UserName IS NULL OR UserName = ''");

            await context.Database.ExecuteSqlInterpolatedAsync($@"
UPDATE Vlasnici
SET PasswordHash =
    CASE
        WHEN Email = 'marko@gmail.com' THEN {markoPasswordHash}
        WHEN Email = 'jovan@gmail.com' THEN {jovanPasswordHash}
        ELSE {defaultPasswordHash}
    END
WHERE PasswordHash IS NULL OR PasswordHash = ''");
        }

        private static async Task EnsureAdminServiserAsync(
            AutoServiceDbContext context,
            IPasswordHasher passwordHasher)
        {
            string adminEmail = "admin@autoservice.com";
            string adminUserName = "admin";

            var adminServiser = await context.Serviseri
                .FirstOrDefaultAsync(s => s.Email == adminEmail);

            if (adminServiser == null)
            {
                adminServiser = new Serviser
                {
                    Ime = "Nikola",
                    Prezime = "Nikolic",
                    Specijalizacija = "Mehanika",
                    Aktivan = true
                };

                await context.Serviseri.AddAsync(adminServiser);
            }

            adminServiser.IsAdmin = true;
            adminServiser.UserName = adminUserName;
            adminServiser.Email = adminEmail;
            adminServiser.PasswordHash ??= passwordHasher.Hash("Admin123!");
            adminServiser.Aktivan = true;

            await context.SaveChangesAsync();
        }
    }
}

