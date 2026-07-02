using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PrviMigrationSaBazom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vlasnici",
                columns: table => new
                {
                    VlasnikId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vlasnici", x => x.VlasnikId);
                });

            migrationBuilder.CreateTable(
                name: "Serviseri",
                columns: table => new
                {
                    ServiserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Specijalizacija = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aktivan = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Serviseri", x => x.ServiserId);
                });

            migrationBuilder.CreateTable(
                name: "ServisneUsluge",
                columns: table => new
                {
                    ServisnaUslugaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Cena = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TrajanjeUMinutima = table.Column<int>(type: "int", nullable: false),
                    Aktivna = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServisneUsluge", x => x.ServisnaUslugaId);
                });

            migrationBuilder.CreateTable(
                name: "Vozila",
                columns: table => new
                {
                    VoziloId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marka = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GodinaProizvodnje = table.Column<int>(type: "int", nullable: false),
                    Registracija = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VlasnikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vozila", x => x.VoziloId);
                    table.ForeignKey(
                        name: "FK_Vozila_Vlasnici_VlasnikId",
                        column: x => x.VlasnikId,
                        principalTable: "Vlasnici",
                        principalColumn: "VlasnikId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Termini",
                columns: table => new
                {
                    TerminId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumIVreme = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Napomena = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    VoziloId = table.Column<int>(type: "int", nullable: false),
                    ServiserId = table.Column<int>(type: "int", nullable: false),
                    ServisnaUslugaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Termini", x => x.TerminId);
                    table.ForeignKey(
                        name: "FK_Termini_Serviseri_ServiserId",
                        column: x => x.ServiserId,
                        principalTable: "Serviseri",
                        principalColumn: "ServiserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Termini_ServisneUsluge_ServisnaUslugaId",
                        column: x => x.ServisnaUslugaId,
                        principalTable: "ServisneUsluge",
                        principalColumn: "ServisnaUslugaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Termini_Vozila_VoziloId",
                        column: x => x.VoziloId,
                        principalTable: "Vozila",
                        principalColumn: "VoziloId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vlasnici_Email",
                table: "Vlasnici",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Termini_ServiserId_DatumIVreme",
                table: "Termini",
                columns: new[] { "ServiserId", "DatumIVreme" });

            migrationBuilder.CreateIndex(
                name: "IX_Termini_ServisnaUslugaId",
                table: "Termini",
                column: "ServisnaUslugaId");

            migrationBuilder.CreateIndex(
                name: "IX_Termini_VoziloId",
                table: "Termini",
                column: "VoziloId");

            migrationBuilder.CreateIndex(
                name: "IX_Vozila_VlasnikId",
                table: "Vozila",
                column: "VlasnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Vozila_Registracija",
                table: "Vozila",
                column: "Registracija",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Termini");

            migrationBuilder.DropTable(
                name: "Serviseri");

            migrationBuilder.DropTable(
                name: "ServisneUsluge");

            migrationBuilder.DropTable(
                name: "Vozila");

            migrationBuilder.DropTable(
                name: "Vlasnici");
        }
    }
}

