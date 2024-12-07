using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Art.EF.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddRetrievalDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Retrieved",
                table: "ArtifactResourceInfoModels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RetrievalDate",
                table: "ArtifactInfoModels",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Retrieved",
                table: "ArtifactResourceInfoModels");

            migrationBuilder.DropColumn(
                name: "RetrievalDate",
                table: "ArtifactInfoModels");
        }
    }
}
