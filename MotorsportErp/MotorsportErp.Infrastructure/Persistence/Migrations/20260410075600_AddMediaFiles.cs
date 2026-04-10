using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotorsportErp.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddMediaFiles : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<Guid>(
            name: "AvatarId",
            table: "Users",
            type: "uniqueidentifier",
            nullable: true);

        _ = migrationBuilder.CreateTable(
            name: "MediaFiles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OriginalFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                SavedUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                UploadedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_MediaFiles", x => x.Id);
                _ = table.ForeignKey(
                    name: "FK_MediaFiles_Users_UploadedById",
                    column: x => x.UploadedById,
                    principalTable: "Users",
                    principalColumn: "Id");
            });

        _ = migrationBuilder.CreateTable(
            name: "CarMediaFile",
            columns: table => new
            {
                CarsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PhotosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_CarMediaFile", x => new { x.CarsId, x.PhotosId });
                _ = table.ForeignKey(
                    name: "FK_CarMediaFile_Cars_CarsId",
                    column: x => x.CarsId,
                    principalTable: "Cars",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                _ = table.ForeignKey(
                    name: "FK_CarMediaFile_MediaFiles_PhotosId",
                    column: x => x.PhotosId,
                    principalTable: "MediaFiles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateTable(
            name: "MediaFileTournament",
            columns: table => new
            {
                PhotosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TournamentsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_MediaFileTournament", x => new { x.PhotosId, x.TournamentsId });
                _ = table.ForeignKey(
                    name: "FK_MediaFileTournament_MediaFiles_PhotosId",
                    column: x => x.PhotosId,
                    principalTable: "MediaFiles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                _ = table.ForeignKey(
                    name: "FK_MediaFileTournament_Tournaments_TournamentsId",
                    column: x => x.TournamentsId,
                    principalTable: "Tournaments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateTable(
            name: "MediaFileTrack",
            columns: table => new
            {
                PhotosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TracksId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_MediaFileTrack", x => new { x.PhotosId, x.TracksId });
                _ = table.ForeignKey(
                    name: "FK_MediaFileTrack_MediaFiles_PhotosId",
                    column: x => x.PhotosId,
                    principalTable: "MediaFiles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                _ = table.ForeignKey(
                    name: "FK_MediaFileTrack_Tracks_TracksId",
                    column: x => x.TracksId,
                    principalTable: "Tracks",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        _ = migrationBuilder.CreateIndex(
            name: "IX_Users_AvatarId",
            table: "Users",
            column: "AvatarId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_CarMediaFile_PhotosId",
            table: "CarMediaFile",
            column: "PhotosId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_MediaFiles_UploadedById",
            table: "MediaFiles",
            column: "UploadedById");

        _ = migrationBuilder.CreateIndex(
            name: "IX_MediaFileTournament_TournamentsId",
            table: "MediaFileTournament",
            column: "TournamentsId");

        _ = migrationBuilder.CreateIndex(
            name: "IX_MediaFileTrack_TracksId",
            table: "MediaFileTrack",
            column: "TracksId");

        _ = migrationBuilder.AddForeignKey(
            name: "FK_Users_MediaFiles_AvatarId",
            table: "Users",
            column: "AvatarId",
            principalTable: "MediaFiles",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropForeignKey(
            name: "FK_Users_MediaFiles_AvatarId",
            table: "Users");

        _ = migrationBuilder.DropTable(
            name: "CarMediaFile");

        _ = migrationBuilder.DropTable(
            name: "MediaFileTournament");

        _ = migrationBuilder.DropTable(
            name: "MediaFileTrack");

        _ = migrationBuilder.DropTable(
            name: "MediaFiles");

        _ = migrationBuilder.DropIndex(
            name: "IX_Users_AvatarId",
            table: "Users");

        _ = migrationBuilder.DropColumn(
            name: "AvatarId",
            table: "Users");
    }
}
