using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class CheckRolesState : Migration
    {
        /// <inheritdoc />
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DeleteData(
        table: "AspNetRoles",
        keyColumn: "Id",
        keyValue: "moderator-id");
}
        /// <inheritdoc />
protected override void Down(MigrationBuilder migrationBuilder)
{

}
    }
}
