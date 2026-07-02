using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVlasnikLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Vlasnici]', N'U') IS NULL AND OBJECT_ID(N'[Korisnici]', N'U') IS NOT NULL
BEGIN
    EXEC sp_rename N'[Korisnici]', N'Vlasnici';
END");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Vlasnici]', N'U') IS NOT NULL
   AND COL_LENGTH('Vlasnici', 'VlasnikId') IS NULL
   AND COL_LENGTH('Vlasnici', 'KorisnikId') IS NOT NULL
BEGIN
    EXEC sp_rename N'[Vlasnici].[KorisnikId]', N'VlasnikId', N'COLUMN';
END");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Vozila]', N'U') IS NOT NULL
   AND COL_LENGTH('Vozila', 'VlasnikId') IS NULL
   AND COL_LENGTH('Vozila', 'KorisnikId') IS NOT NULL
BEGIN
    IF OBJECT_ID(N'[FK_Vozila_Korisnici_KorisnikId]', N'F') IS NOT NULL
        ALTER TABLE [Vozila] DROP CONSTRAINT [FK_Vozila_Korisnici_KorisnikId];

    IF EXISTS (
        SELECT 1
        FROM sys.indexes
        WHERE name = 'IX_Vozila_KorisnikId'
          AND object_id = OBJECT_ID('Vozila')
    )
        DROP INDEX [IX_Vozila_KorisnikId] ON [Vozila];

    EXEC sp_rename N'[Vozila].[KorisnikId]', N'VlasnikId', N'COLUMN';
END");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Vozila]', N'U') IS NOT NULL
   AND COL_LENGTH('Vozila', 'VlasnikId') IS NOT NULL
   AND NOT EXISTS (
        SELECT 1
        FROM sys.indexes
        WHERE name = 'IX_Vozila_VlasnikId'
          AND object_id = OBJECT_ID('Vozila')
   )
BEGIN
    CREATE INDEX [IX_Vozila_VlasnikId] ON [Vozila] ([VlasnikId]);
END");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Vozila]', N'U') IS NOT NULL
   AND OBJECT_ID(N'[Vlasnici]', N'U') IS NOT NULL
   AND OBJECT_ID(N'[FK_Vozila_Vlasnici_VlasnikId]', N'F') IS NULL
BEGIN
    ALTER TABLE [Vozila] WITH CHECK ADD CONSTRAINT [FK_Vozila_Vlasnici_VlasnikId]
        FOREIGN KEY([VlasnikId]) REFERENCES [Vlasnici] ([VlasnikId])
        ON DELETE NO ACTION;
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Vlasnici', 'PasswordHash') IS NULL
BEGIN
    ALTER TABLE [Vlasnici] ADD [PasswordHash] nvarchar(500) NULL;
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Vlasnici', 'UserName') IS NULL
BEGIN
    ALTER TABLE [Vlasnici] ADD [UserName] nvarchar(50) NULL;
END");

            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Vlasnici_UserName'
      AND object_id = OBJECT_ID('Vlasnici')
)
BEGIN
    CREATE UNIQUE INDEX [IX_Vlasnici_UserName]
    ON [Vlasnici] ([UserName])
    WHERE [UserName] IS NOT NULL;
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Vlasnici_UserName'
      AND object_id = OBJECT_ID('Vlasnici')
)
BEGIN
    DROP INDEX [IX_Vlasnici_UserName] ON [Vlasnici];
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Vlasnici', 'PasswordHash') IS NOT NULL
BEGIN
    ALTER TABLE [Vlasnici] DROP COLUMN [PasswordHash];
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Vlasnici', 'UserName') IS NOT NULL
BEGIN
    ALTER TABLE [Vlasnici] DROP COLUMN [UserName];
END");
        }
    }
}

