using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddEquivalencyInstitutionData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalNotes",
                table: "EquivalencyApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "CountryId",
                table: "EquivalencyApplications",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GraduationYear",
                table: "EquivalencyApplications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstitutionId",
                table: "EquivalencyApplications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MajorId",
                table: "EquivalencyApplications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EquivalencyCountries",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquivalencyCountries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquivalencyInstitutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CountryId = table.Column<short>(type: "smallint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquivalencyInstitutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquivalencyInstitutions_EquivalencyCountries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "EquivalencyCountries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquivalencyMajors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    InstitutionId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquivalencyMajors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquivalencyMajors_EquivalencyInstitutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "EquivalencyInstitutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquivalencyApplications_CountryId",
                table: "EquivalencyApplications",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_EquivalencyApplications_InstitutionId",
                table: "EquivalencyApplications",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_EquivalencyApplications_MajorId",
                table: "EquivalencyApplications",
                column: "MajorId");

            migrationBuilder.CreateIndex(
                name: "IX_EquivalencyInstitutions_CountryId",
                table: "EquivalencyInstitutions",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_EquivalencyMajors_InstitutionId",
                table: "EquivalencyMajors",
                column: "InstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquivalencyApplications_EquivalencyCountries_CountryId",
                table: "EquivalencyApplications",
                column: "CountryId",
                principalTable: "EquivalencyCountries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquivalencyApplications_EquivalencyInstitutions_InstitutionId",
                table: "EquivalencyApplications",
                column: "InstitutionId",
                principalTable: "EquivalencyInstitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EquivalencyApplications_EquivalencyMajors_MajorId",
                table: "EquivalencyApplications",
                column: "MajorId",
                principalTable: "EquivalencyMajors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquivalencyApplications_EquivalencyCountries_CountryId",
                table: "EquivalencyApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_EquivalencyApplications_EquivalencyInstitutions_InstitutionId",
                table: "EquivalencyApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_EquivalencyApplications_EquivalencyMajors_MajorId",
                table: "EquivalencyApplications");

            migrationBuilder.DropTable(
                name: "EquivalencyMajors");

            migrationBuilder.DropTable(
                name: "EquivalencyInstitutions");

            migrationBuilder.DropTable(
                name: "EquivalencyCountries");

            migrationBuilder.DropIndex(
                name: "IX_EquivalencyApplications_CountryId",
                table: "EquivalencyApplications");

            migrationBuilder.DropIndex(
                name: "IX_EquivalencyApplications_InstitutionId",
                table: "EquivalencyApplications");

            migrationBuilder.DropIndex(
                name: "IX_EquivalencyApplications_MajorId",
                table: "EquivalencyApplications");

            migrationBuilder.DropColumn(
                name: "AdditionalNotes",
                table: "EquivalencyApplications");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "EquivalencyApplications");

            migrationBuilder.DropColumn(
                name: "GraduationYear",
                table: "EquivalencyApplications");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "EquivalencyApplications");

            migrationBuilder.DropColumn(
                name: "MajorId",
                table: "EquivalencyApplications");
        }
    }
}
