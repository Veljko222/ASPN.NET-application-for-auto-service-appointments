using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoService.Infrastructure.Migrations
{
    [Migration("20260702000000_AddAdminCredentialsToServiser")]
    public partial class AddAdminCredentialsToServiser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('Serviseri', 'IsAdmin') IS NULL
BEGIN
    ALTER TABLE [Serviseri] ADD [IsAdmin] bit NOT NULL CONSTRAINT [DF_Serviseri_IsAdmin] DEFAULT CAST(0 AS bit);
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Serviseri', 'UserName') IS NULL
BEGIN
    ALTER TABLE [Serviseri] ADD [UserName] nvarchar(50) NULL;
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Serviseri', 'Email') IS NULL
BEGIN
    ALTER TABLE [Serviseri] ADD [Email] nvarchar(100) NULL;
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Serviseri', 'PasswordHash') IS NULL
BEGIN
    ALTER TABLE [Serviseri] ADD [PasswordHash] nvarchar(500) NULL;
END");

            migrationBuilder.Sql(@"
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

            migrationBuilder.Sql(@"
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Serviseri_UserName'
      AND object_id = OBJECT_ID('Serviseri')
)
BEGIN
    DROP INDEX [IX_Serviseri_UserName] ON [Serviseri];
END");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Serviseri_Email'
      AND object_id = OBJECT_ID('Serviseri')
)
BEGIN
    DROP INDEX [IX_Serviseri_Email] ON [Serviseri];
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Serviseri', 'PasswordHash') IS NOT NULL
BEGIN
    ALTER TABLE [Serviseri] DROP COLUMN [PasswordHash];
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Serviseri', 'Email') IS NOT NULL
BEGIN
    ALTER TABLE [Serviseri] DROP COLUMN [Email];
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Serviseri', 'UserName') IS NOT NULL
BEGIN
    ALTER TABLE [Serviseri] DROP COLUMN [UserName];
END");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Serviseri', 'IsAdmin') IS NOT NULL
BEGIN
    ALTER TABLE [Serviseri] DROP CONSTRAINT [DF_Serviseri_IsAdmin];
    ALTER TABLE [Serviseri] DROP COLUMN [IsAdmin];
END");
        }
    }
}
