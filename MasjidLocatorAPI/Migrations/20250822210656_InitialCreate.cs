using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace MasjidLocatorAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Masjids",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<Point>(type: "geography (point)", nullable: false),
                    DriveStartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DriveEndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ContactPerson = table.Column<string>(type: "text", nullable: true),
                    SpecialInstructions = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Masjids", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PendingMasjidSubmission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmittedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<Point>(type: "geography (point)", nullable: false),
                    DriveStartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DriveEndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    AdminNotes = table.Column<string>(type: "text", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingMasjidSubmission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Masjids_Location",
                table: "Masjids",
                column: "Location")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "IX_PendingMasjidSubmission_Location",
                table: "PendingMasjidSubmission",
                column: "Location")
                .Annotation("Npgsql:IndexMethod", "GIST");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Masjids");

            migrationBuilder.DropTable(
                name: "PendingMasjidSubmission");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
