using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Art.EF.Sqlite.Migrations
{
    public partial class AddChecksums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChecksumId",
                table: "ArtifactResourceInfoModels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ChecksumValue",
                table: "ArtifactResourceInfoModels",
                type: "BLOB",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChecksumId",
                table: "ArtifactResourceInfoModels");

            migrationBuilder.DropColumn(
                name: "ChecksumValue",
                table: "ArtifactResourceInfoModels");
        }
    }
}
