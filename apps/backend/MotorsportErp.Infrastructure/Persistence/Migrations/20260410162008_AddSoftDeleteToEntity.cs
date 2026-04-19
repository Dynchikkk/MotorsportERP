using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotorsportErp.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddSoftDeleteToEntity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Users",
            type: "bit",
            nullable: false,
            defaultValue: false);

        _ = migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "TrackVotes",
            type: "bit",
            nullable: false,
            defaultValue: false);

        _ = migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Tracks",
            type: "bit",
            nullable: false,
            defaultValue: false);

        _ = migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Tournaments",
            type: "bit",
            nullable: false,
            defaultValue: false);

        _ = migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "TournamentResults",
            type: "bit",
            nullable: false,
            defaultValue: false);

        _ = migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "TournamentOrganizers",
            type: "bit",
            nullable: false,
            defaultValue: false);

        _ = migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "TournamentApplications",
            type: "bit",
            nullable: false,
            defaultValue: false);

        _ = migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "MediaFiles",
            type: "bit",
            nullable: false,
            defaultValue: false);

        _ = migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Cars",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Users");

        _ = migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "TrackVotes");

        _ = migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Tracks");

        _ = migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Tournaments");

        _ = migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "TournamentResults");

        _ = migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "TournamentOrganizers");

        _ = migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "TournamentApplications");

        _ = migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "MediaFiles");

        _ = migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Cars");
    }
}
