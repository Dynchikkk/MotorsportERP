using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotorsportErp.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Roles = table.Column<int>(type: "int", nullable: false),
                Nickname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Bio = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                RaceCount = table.Column<int>(type: "int", nullable: false),
                IsBlocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Users", x => x.Id);
            });

        _ = migrationBuilder.CreateTable(
            name: "Cars",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CarClass = table.Column<int>(type: "int", nullable: false),
                Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Year = table.Column<int>(type: "int", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Cars", x => x.Id);
                _ = table.ForeignKey(
                    name: "FK_Cars_Users_OwnerId",
                    column: x => x.OwnerId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateTable(
            name: "Tracks",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                LayoutImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ConfirmationThreshold = table.Column<int>(type: "int", nullable: false),
                CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Tracks", x => x.Id);
                _ = table.ForeignKey(
                    name: "FK_Tracks_Users_CreatedById",
                    column: x => x.CreatedById,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        _ = migrationBuilder.CreateTable(
            name: "Tournaments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                AllowedCarClass = table.Column<int>(type: "int", nullable: false),
                RequiredRaceCount = table.Column<int>(type: "int", nullable: false),
                RequiredParticipants = table.Column<int>(type: "int", nullable: false),
                TrackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_Tournaments", x => x.Id);
                _ = table.ForeignKey(
                    name: "FK_Tournaments_Tracks_TrackId",
                    column: x => x.TrackId,
                    principalTable: "Tracks",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                _ = table.ForeignKey(
                    name: "FK_Tournaments_Users_CreatorId",
                    column: x => x.CreatorId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        _ = migrationBuilder.CreateTable(
            name: "TrackVotes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                IsPositive = table.Column<bool>(type: "bit", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TrackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_TrackVotes", x => x.Id);
                _ = table.ForeignKey(
                    name: "FK_TrackVotes_Tracks_TrackId",
                    column: x => x.TrackId,
                    principalTable: "Tracks",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                _ = table.ForeignKey(
                    name: "FK_TrackVotes_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateTable(
            name: "TournamentApplications",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TournamentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_TournamentApplications", x => x.Id);
                _ = table.ForeignKey(
                    name: "FK_TournamentApplications_Cars_CarId",
                    column: x => x.CarId,
                    principalTable: "Cars",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                _ = table.ForeignKey(
                    name: "FK_TournamentApplications_Tournaments_TournamentId",
                    column: x => x.TournamentId,
                    principalTable: "Tournaments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                _ = table.ForeignKey(
                    name: "FK_TournamentApplications_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateTable(
            name: "TournamentOrganizers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TournamentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_TournamentOrganizers", x => x.Id);
                _ = table.ForeignKey(
                    name: "FK_TournamentOrganizers_Tournaments_TournamentId",
                    column: x => x.TournamentId,
                    principalTable: "Tournaments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                _ = table.ForeignKey(
                    name: "FK_TournamentOrganizers_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateTable(
            name: "TournamentResults",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TournamentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Position = table.Column<int>(type: "int", nullable: false),
                BestLapTime = table.Column<TimeSpan>(type: "time", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_TournamentResults", x => x.Id);
                _ = table.ForeignKey(
                    name: "FK_TournamentResults_Tournaments_TournamentId",
                    column: x => x.TournamentId,
                    principalTable: "Tournaments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                _ = table.ForeignKey(
                    name: "FK_TournamentResults_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_Cars_OwnerId",
            table: "Cars",
            column: "OwnerId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_TournamentApplications_CarId",
            table: "TournamentApplications",
            column: "CarId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_TournamentApplications_TournamentId",
            table: "TournamentApplications",
            column: "TournamentId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_TournamentApplications_UserId_TournamentId",
            table: "TournamentApplications",
            columns: new[] { "UserId", "TournamentId" },
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_TournamentOrganizers_TournamentId_UserId",
            table: "TournamentOrganizers",
            columns: new[] { "TournamentId", "UserId" },
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_TournamentOrganizers_UserId",
            table: "TournamentOrganizers",
            column: "UserId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_TournamentResults_TournamentId_UserId",
            table: "TournamentResults",
            columns: new[] { "TournamentId", "UserId" },
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_TournamentResults_UserId",
            table: "TournamentResults",
            column: "UserId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_Tournaments_CreatorId",
            table: "Tournaments",
            column: "CreatorId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_Tournaments_TrackId",
            table: "Tournaments",
            column: "TrackId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_Tracks_CreatedById",
            table: "Tracks",
            column: "CreatedById");

        _ = migrationBuilder.CreateIndex(
            name: "IX_TrackVotes_TrackId",
            table: "TrackVotes",
            column: "TrackId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_TrackVotes_UserId_TrackId",
            table: "TrackVotes",
            columns: new[] { "UserId", "TrackId" },
            unique: true);

        _ = migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropTable(
            name: "TournamentApplications");

        _ = migrationBuilder.DropTable(
            name: "TournamentOrganizers");

        _ = migrationBuilder.DropTable(
            name: "TournamentResults");

        _ = migrationBuilder.DropTable(
            name: "TrackVotes");

        _ = migrationBuilder.DropTable(
            name: "Cars");

        _ = migrationBuilder.DropTable(
            name: "Tournaments");

        _ = migrationBuilder.DropTable(
            name: "Tracks");

        _ = migrationBuilder.DropTable(
            name: "Users");
    }
}
