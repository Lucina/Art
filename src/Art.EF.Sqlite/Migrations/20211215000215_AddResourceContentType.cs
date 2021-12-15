using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Art.EF.Sqlite.Migrations
{
    public partial class AddResourceContentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "ArtifactResourceInfoModels",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ArtifactResourceInfoModels");
        }
    }
}
