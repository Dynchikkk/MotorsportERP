using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotorsportErp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntityIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TrackVotes_UserId_TrackId",
                table: "TrackVotes");

            migrationBuilder.DropIndex(
                name: "IX_TournamentResults_TournamentId_UserId",
                table: "TournamentResults");

            migrationBuilder.DropIndex(
                name: "IX_TournamentOrganizers_TournamentId_UserId",
                table: "TournamentOrganizers");

            migrationBuilder.DropIndex(
                name: "IX_TournamentApplications_UserId_TournamentId",
                table: "TournamentApplications");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TrackVotes_UserId_TrackId",
                table: "TrackVotes",
                columns: new[] { "UserId", "TrackId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentResults_TournamentId_UserId",
                table: "TournamentResults",
                columns: new[] { "TournamentId", "UserId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentOrganizers_TournamentId_UserId",
                table: "TournamentOrganizers",
                columns: new[] { "TournamentId", "UserId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentApplications_UserId_TournamentId",
                table: "TournamentApplications",
                columns: new[] { "UserId", "TournamentId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TrackVotes_UserId_TrackId",
                table: "TrackVotes");

            migrationBuilder.DropIndex(
                name: "IX_TournamentResults_TournamentId_UserId",
                table: "TournamentResults");

            migrationBuilder.DropIndex(
                name: "IX_TournamentOrganizers_TournamentId_UserId",
                table: "TournamentOrganizers");

            migrationBuilder.DropIndex(
                name: "IX_TournamentApplications_UserId_TournamentId",
                table: "TournamentApplications");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrackVotes_UserId_TrackId",
                table: "TrackVotes",
                columns: new[] { "UserId", "TrackId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentResults_TournamentId_UserId",
                table: "TournamentResults",
                columns: new[] { "TournamentId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentOrganizers_TournamentId_UserId",
                table: "TournamentOrganizers",
                columns: new[] { "TournamentId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentApplications_UserId_TournamentId",
                table: "TournamentApplications",
                columns: new[] { "UserId", "TournamentId" },
                unique: true);
        }
    }
}
