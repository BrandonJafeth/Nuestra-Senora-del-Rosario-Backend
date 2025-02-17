using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class NuevaMigra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AdministrationRoutes",
                columns: table => new
                {
                    Id_AdministrationRoute = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RouteName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministrationRoutes", x => x.Id_AdministrationRoute);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UnitOfMeasures",
                columns: table => new
                {
                    UnitOfMeasureID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UnitName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOfMeasures", x => x.UnitOfMeasureID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MedicationSpecifics",
                columns: table => new
                {
                    Id_MedicamentSpecific = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name_MedicamentSpecific = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SpecialInstructions = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdministrationSchedule = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    UnitOfMeasureID = table.Column<int>(type: "int", nullable: false),
                    Id_AdministrationRoute = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationSpecifics", x => x.Id_MedicamentSpecific);
                    table.ForeignKey(
                        name: "FK_MedicationSpecifics_AdministrationRoutes_Id_AdministrationRoute",
                        column: x => x.Id_AdministrationRoute,
                        principalTable: "AdministrationRoutes",
                        principalColumn: "Id_AdministrationRoute",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicationSpecifics_UnitOfMeasures_UnitOfMeasureID",
                        column: x => x.UnitOfMeasureID,
                        principalTable: "UnitOfMeasures",
                        principalColumn: "UnitOfMeasureID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ResidentMedications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ResidentId = table.Column<int>(type: "int", nullable: false),
                    MedicationSpecificId = table.Column<int>(type: "int", nullable: false),
                    DosisPrescrita = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Notas = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResidentMedications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResidentMedications_MedicationSpecifics_MedicationSpecificId",
                        column: x => x.MedicationSpecificId,
                        principalTable: "MedicationSpecifics",
                        principalColumn: "Id_MedicamentSpecific",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResidentMedications_Residents_ResidentId",
                        column: x => x.ResidentId,
                        principalTable: "Residents",
                        principalColumn: "Id_Resident",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationSpecifics_Id_AdministrationRoute",
                table: "MedicationSpecifics",
                column: "Id_AdministrationRoute");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationSpecifics_UnitOfMeasureID",
                table: "MedicationSpecifics",
                column: "UnitOfMeasureID");

            migrationBuilder.CreateIndex(
                name: "IX_ResidentMedications_MedicationSpecificId",
                table: "ResidentMedications",
                column: "MedicationSpecificId");

            migrationBuilder.CreateIndex(
                name: "IX_ResidentMedications_ResidentId",
                table: "ResidentMedications",
                column: "ResidentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResidentMedications");

            migrationBuilder.DropTable(
                name: "MedicationSpecifics");

            migrationBuilder.DropTable(
                name: "administrationroutes");

            migrationBuilder.DropTable(
                name: "UnitOfMeasures");
        }
    }
}
