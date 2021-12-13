using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Art.EF.Sqlite.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArtifactInfoModels",
                columns: table => new
                {
                    Tool = table.Column<string>(type: "TEXT", nullable: false),
                    Group = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Date = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    Full = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtifactInfoModels", x => new { x.Tool, x.Group, x.Id });
                });

            migrationBuilder.CreateTable(
                name: "ArtifactResourceInfoModels",
                columns: table => new
                {
                    ArtifactTool = table.Column<string>(type: "TEXT", nullable: false),
                    ArtifactGroup = table.Column<string>(type: "TEXT", nullable: false),
                    ArtifactId = table.Column<string>(type: "TEXT", nullable: false),
                    File = table.Column<string>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtifactResourceInfoModels", x => new { x.ArtifactTool, x.ArtifactGroup, x.ArtifactId, x.File, x.Path });
                    table.ForeignKey(
                        name: "FK_ArtifactResourceInfoModels_ArtifactInfoModels_ArtifactTool_ArtifactGroup_ArtifactId",
                        columns: x => new { x.ArtifactTool, x.ArtifactGroup, x.ArtifactId },
                        principalTable: "ArtifactInfoModels",
                        principalColumns: new[] { "Tool", "Group", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtifactResourceInfoModels");

            migrationBuilder.DropTable(
                name: "ArtifactInfoModels");
        }
    }
}
