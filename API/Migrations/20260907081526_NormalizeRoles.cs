using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeRoles : Migration
    {
        /// <inheritdoc />
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.InsertData(
        table: "AspNetRoles",
        columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
        values: new object[,]
        {
            {
                "manager-id",
                "manager-concurrency-stamp",
                "Manager",
                "MANAGER"
            },
            {
                "equivalency-id",
                "equivalency-concurrency-stamp",
                "Equivalency",
                "EQUIVALENCY"
            },
            {
                "receiving-id",
                "receiving-concurrency-stamp",
                "Receiving",
                "RECEIVING"
            },
            {
                "inquiry-id",
                "inquiry-concurrency-stamp",
                "Inquiry",
                "INQUIRY"
            },
            {
                "archive-id",
                "archive-concurrency-stamp",
                "Archive",
                "ARCHIVE"
            },
            {
                "office-id",
                "office-concurrency-stamp",
                "Office",
                "OFFICE"
            },
            {
                "printing-id",
                "printing-concurrency-stamp",
                "Printing",
                "PRINTING"
            },
            {
                "committee_coordinator-id",
                "committee_coordinator-concurrency-stamp",
                "Committee_Coordinator",
                "COMMITTEE_COORDINATOR"
            },
            {
                "committee_member-id",
                "committee_member-concurrency-stamp",
                "Committee_Member",
                "COMMITTEE_MEMBER"
            },
            {
                "applicant-id",
                "applicant-concurrency-stamp",
                "Applicant",
                "APPLICANT"
            }
        });
}
        /// <inheritdoc />
protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DeleteData(
        table: "AspNetRoles",
        keyColumn: "Id",
        keyValues: new object[]
        {
            "manager-id",
            "equivalency-id",
            "receiving-id",
            "inquiry-id",
            "archive-id",
            "office-id",
            "printing-id",
            "committee_coordinator-id",
            "committee_member-id",
            "applicant-id"
        });
}    }
}
