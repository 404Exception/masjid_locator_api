using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace MasjidLocatorAPI.Migrations
{
    /// <inheritdoc />
    public partial class refreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Masjids");

            migrationBuilder.DropTable(
                name: "PendingMasjidSubmission");

            migrationBuilder.DropColumn(
                name: "HomeOfficeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "MasjidSubmission",
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
                    table.PrimaryKey("PK_MasjidSubmission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByIp = table.Column<string>(type: "text", nullable: false),
                    Revoked = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RevokedByIp = table.Column<string>(type: "text", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MasjidSubmission_Location",
                table: "MasjidSubmission",
                column: "Location")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MasjidSubmission");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.AddColumn<Guid>(
                name: "HomeOfficeId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Masjids",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    ContactPerson = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DriveEndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DriveStartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Location = table.Column<Point>(type: "geography (point)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SpecialInstructions = table.Column<string>(type: "text", nullable: true)
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
                    Address = table.Column<string>(type: "text", nullable: false),
                    AdminNotes = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: false),
                    DriveEndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DriveStartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Location = table.Column<Point>(type: "geography (point)", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubmittedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingMasjidSubmission", x => x.Id);
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
    }
}
