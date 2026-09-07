using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class RenameMemberToApplicant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove the foreign keys temporarily
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Members_MemberId",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_AspNetUsers_Id",
                table: "Members");

            // Rename the existing Members table to Applicants.
            // IMPORTANT: This keeps all existing data.
            migrationBuilder.RenameTable(
                name: "Members",
                newName: "Applicants");

            // Rename Photos.MemberId -> Photos.ApplicantId
            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "Photos",
                newName: "ApplicantId");

            // Rename the index
            migrationBuilder.RenameIndex(
                name: "IX_Photos_MemberId",
                table: "Photos",
                newName: "IX_Photos_ApplicantId");

            // Recreate the FK from Applicants to AspNetUsers
            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_AspNetUsers_Id",
                table: "Applicants",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Recreate the FK from Photos to Applicants
            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Applicants_ApplicantId",
                table: "Photos",
                column: "ApplicantId",
                principalTable: "Applicants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the new foreign keys
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Applicants_ApplicantId",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_AspNetUsers_Id",
                table: "Applicants");

            // Rename Photos.ApplicantId -> Photos.MemberId
            migrationBuilder.RenameColumn(
                name: "ApplicantId",
                table: "Photos",
                newName: "MemberId");

            // Rename index back
            migrationBuilder.RenameIndex(
                name: "IX_Photos_ApplicantId",
                table: "Photos",
                newName: "IX_Photos_MemberId");

            // Rename Applicants back to Members
            migrationBuilder.RenameTable(
                name: "Applicants",
                newName: "Members");

            // Restore the original foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_Members_AspNetUsers_Id",
                table: "Members",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Members_MemberId",
                table: "Photos",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}