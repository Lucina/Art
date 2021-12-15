using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Art.EF.Sqlite.Migrations
{
    public partial class AddArtifactOnResourceIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ArtifactResourceInfoModels_ArtifactTool_ArtifactGroup_ArtifactId",
                table: "ArtifactResourceInfoModels",
                columns: new[] { "ArtifactTool", "ArtifactGroup", "ArtifactId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ArtifactResourceInfoModels_ArtifactTool_ArtifactGroup_ArtifactId",
                table: "ArtifactResourceInfoModels");
        }
    }
}
