using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotorsportErp.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class RemoveImageFromTrack : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "LayoutImageUrl",
            table: "Tracks");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<string>(
            name: "LayoutImageUrl",
            table: "Tracks",
            type: "nvarchar(max)",
            nullable: true);
    }
}
