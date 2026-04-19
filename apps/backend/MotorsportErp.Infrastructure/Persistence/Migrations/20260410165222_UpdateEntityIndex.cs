using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotorsportErp.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class UpdateEntityIndex : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropIndex(
            name: "IX_Users_Email",
            table: "Users");

        _ = migrationBuilder.DropIndex(
            name: "IX_TrackVotes_UserId_TrackId",
            table: "TrackVotes");

        _ = migrationBuilder.DropIndex(
            name: "IX_TournamentResults_TournamentId_UserId",
            table: "TournamentResults");

        _ = migrationBuilder.DropIndex(
            name: "IX_TournamentOrganizers_TournamentId_UserId",
            table: "TournamentOrganizers");

        _ = migrationBuilder.DropIndex(
            name: "IX_TournamentApplications_UserId_TournamentId",
            table: "TournamentApplications");

        _ = migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true,
            filter: "[IsDeleted] = 0");

        _ = migrationBuilder.CreateIndex(
            name: "IX_TrackVotes_UserId_TrackId",
            table: "TrackVotes",
            columns: new[] { "UserId", "TrackId" },
            unique: true,
            filter: "[IsDeleted] = 0");

        _ = migrationBuilder.CreateIndex(
            name: "IX_TournamentResults_TournamentId_UserId",
            table: "TournamentResults",
            columns: new[] { "TournamentId", "UserId" },
            unique: true,
            filter: "[IsDeleted] = 0");

        _ = migrationBuilder.CreateIndex(
            name: "IX_TournamentOrganizers_TournamentId_UserId",
            table: "TournamentOrganizers",
            columns: new[] { "TournamentId", "UserId" },
            unique: true,
            filter: "[IsDeleted] = 0");

        _ = migrationBuilder.CreateIndex(
            name: "IX_TournamentApplications_UserId_TournamentId",
            table: "TournamentApplications",
            columns: new[] { "UserId", "TournamentId" },
            unique: true,
            filter: "[IsDeleted] = 0");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropIndex(
            name: "IX_Users_Email",
            table: "Users");

        _ = migrationBuilder.DropIndex(
            name: "IX_TrackVotes_UserId_TrackId",
            table: "TrackVotes");

        _ = migrationBuilder.DropIndex(
            name: "IX_TournamentResults_TournamentId_UserId",
            table: "TournamentResults");

        _ = migrationBuilder.DropIndex(
            name: "IX_TournamentOrganizers_TournamentId_UserId",
            table: "TournamentOrganizers");

        _ = migrationBuilder.DropIndex(
            name: "IX_TournamentApplications_UserId_TournamentId",
            table: "TournamentApplications");

        _ = migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_TrackVotes_UserId_TrackId",
            table: "TrackVotes",
            columns: new[] { "UserId", "TrackId" },
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_TournamentResults_TournamentId_UserId",
            table: "TournamentResults",
            columns: new[] { "TournamentId", "UserId" },
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_TournamentOrganizers_TournamentId_UserId",
            table: "TournamentOrganizers",
            columns: new[] { "TournamentId", "UserId" },
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_TournamentApplications_UserId_TournamentId",
            table: "TournamentApplications",
            columns: new[] { "UserId", "TournamentId" },
            unique: true);
    }
}
