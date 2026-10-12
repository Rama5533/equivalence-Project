using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQualificationRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "QualificationRequirements",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.UpdateData(
                table: "QualificationRequirements",
                keyColumn: "Id",
                keyValue: 1,
                column: "QualificationType",
                value: 3);

            migrationBuilder.UpdateData(
                table: "QualificationRequirements",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "QualificationType", "RequiredQualificationType" },
                values: new object[] { 4, 3 });

            migrationBuilder.UpdateData(
                table: "QualificationRequirements",
                keyColumn: "Id",
                keyValue: 3,
                column: "RequiredQualificationType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QualificationRequirements",
                keyColumn: "Id",
                keyValue: 4,
                column: "QualificationType",
                value: 5);

            migrationBuilder.UpdateData(
                table: "QualificationRequirements",
                keyColumn: "Id",
                keyValue: 5,
                column: "RequiredQualificationType",
                value: 3);

            migrationBuilder.UpdateData(
                table: "QualificationRequirements",
                keyColumn: "Id",
                keyValue: 6,
                column: "RequiredQualificationType",
                value: 4);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "QualificationRequirements",
                keyColumn: "Id",
                keyValue: 1,
                column: "QualificationType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "QualificationRequirements",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "QualificationType", "RequiredQualificationType" },
                values: new object[] { 3, 1 });

            migrationBuilder.UpdateData(
                table: "QualificationRequirements",
                keyColumn: "Id",
                keyValue: 3,
                column: "RequiredQualificationType",
                value: 3);

            migrationBuilder.UpdateData(
                table: "QualificationRequirements",
                keyColumn: "Id",
                keyValue: 4,
                column: "QualificationType",
                value: 4);

            migrationBuilder.UpdateData(
                table: "QualificationRequirements",
                keyColumn: "Id",
                keyValue: 5,
                column: "RequiredQualificationType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "QualificationRequirements",
                keyColumn: "Id",
                keyValue: 6,
                column: "RequiredQualificationType",
                value: 3);

            migrationBuilder.InsertData(
                table: "QualificationRequirements",
                columns: new[] { "Id", "QualificationType", "RequiredQualificationType" },
                values: new object[] { 7, 5, 4 });
        }
    }
}
