using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicantQualificationDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "qualificationType",
                table: "ApplicantQualifications",
                newName: "QualificationType");

            migrationBuilder.AddColumn<short>(
                name: "CountryId",
                table: "ApplicantQualifications",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "GraduationYear",
                table: "ApplicantQualifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InstitutionId",
                table: "ApplicantQualifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantQualifications_CountryId",
                table: "ApplicantQualifications",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantQualifications_InstitutionId",
                table: "ApplicantQualifications",
                column: "InstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantQualifications_EquivalencyCountries_CountryId",
                table: "ApplicantQualifications",
                column: "CountryId",
                principalTable: "EquivalencyCountries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantQualifications_EquivalencyInstitutions_InstitutionId",
                table: "ApplicantQualifications",
                column: "InstitutionId",
                principalTable: "EquivalencyInstitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantQualifications_EquivalencyCountries_CountryId",
                table: "ApplicantQualifications");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantQualifications_EquivalencyInstitutions_InstitutionId",
                table: "ApplicantQualifications");

            migrationBuilder.DropIndex(
                name: "IX_ApplicantQualifications_CountryId",
                table: "ApplicantQualifications");

            migrationBuilder.DropIndex(
                name: "IX_ApplicantQualifications_InstitutionId",
                table: "ApplicantQualifications");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "ApplicantQualifications");

            migrationBuilder.DropColumn(
                name: "GraduationYear",
                table: "ApplicantQualifications");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "ApplicantQualifications");

            migrationBuilder.RenameColumn(
                name: "QualificationType",
                table: "ApplicantQualifications",
                newName: "qualificationType");
        }
    }
}
