using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldOtpFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE Name = N'OtpCode'
                      AND Object_ID = Object_ID(N'AspNetUsers')
                )
                BEGIN
                    ALTER TABLE [AspNetUsers]
                    DROP COLUMN [OtpCode];
                END
                """);

            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE Name = N'OtpExpiry'
                      AND Object_ID = Object_ID(N'AspNetUsers')
                )
                BEGIN
                    ALTER TABLE [AspNetUsers]
                    DROP COLUMN [OtpExpiry];
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE Name = N'OtpCode'
                      AND Object_ID = Object_ID(N'AspNetUsers')
                )
                BEGIN
                    ALTER TABLE [AspNetUsers]
                    ADD [OtpCode] nvarchar(max) NULL;
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE Name = N'OtpExpiry'
                      AND Object_ID = Object_ID(N'AspNetUsers')
                )
                BEGIN
                    ALTER TABLE [AspNetUsers]
                    ADD [OtpExpiry] datetime2 NULL;
                END
                """);
        }
    }
}