using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPISistemaRifas.Migrations
{
    public partial class EliminarGanadores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ganadores");

            migrationBuilder.DropColumn(
                name: "IdGanadores",
                table: "Premios");

            migrationBuilder.CreateIndex(
                name: "IX_Premios_RifaId",
                table: "Premios",
                column: "RifaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Premios_Rifas_RifaId",
                table: "Premios",
                column: "RifaId",
                principalTable: "Rifas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Premios_Rifas_RifaId",
                table: "Premios");

            migrationBuilder.DropIndex(
                name: "IX_Premios_RifaId",
                table: "Premios");

            migrationBuilder.AddColumn<int>(
                name: "IdGanadores",
                table: "Premios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Ganadores",
                columns: table => new
                {
                    RifaId = table.Column<int>(type: "int", nullable: false),
                    PremioId = table.Column<int>(type: "int", nullable: false),
                    ParticipanteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ganadores", x => new { x.RifaId, x.PremioId });
                    table.ForeignKey(
                        name: "FK_Ganadores_Participantes_ParticipanteId",
                        column: x => x.ParticipanteId,
                        principalTable: "Participantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ganadores_Premios_PremioId",
                        column: x => x.PremioId,
                        principalTable: "Premios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ganadores_Rifas_RifaId",
                        column: x => x.RifaId,
                        principalTable: "Rifas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ganadores_ParticipanteId",
                table: "Ganadores",
                column: "ParticipanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ganadores_PremioId",
                table: "Ganadores",
                column: "PremioId",
                unique: true);
        }
    }
}
