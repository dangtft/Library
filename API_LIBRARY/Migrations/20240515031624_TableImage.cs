using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_LIBRARY.Migrations
{
    /// <inheritdoc />
    public partial class TableImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(nullable: false),
                    FileDescription = table.Column<string>(nullable: true),
                    FileExtension = table.Column<string>(nullable: true),
                    FileSizeInBytes = table.Column<long>(nullable: false),
                    FilePath = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images");
        }
    }
}
