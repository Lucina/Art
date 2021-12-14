using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Art.EF.Sqlite.Migrations
{
    public partial class AddResourceUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Updated",
                table: "ArtifactResourceInfoModels",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Updated",
                table: "ArtifactResourceInfoModels");
        }
    }
}
