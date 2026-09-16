using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddQualificationRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QualificationRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QualificationType = table.Column<int>(type: "int", nullable: false),
                    RequiredQualificationType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualificationRequirements", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "QualificationRequirements",
                columns: new[] { "Id", "QualificationType", "RequiredQualificationType" },
                values: new object[,]
                {
                    { 1, 2, 1 },
                    { 2, 3, 1 },
                    { 3, 4, 3 },
                    { 4, 4, 1 },
                    { 5, 5, 1 },
                    { 6, 5, 3 },
                    { 7, 5, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QualificationRequirements");
        }
    }
}
