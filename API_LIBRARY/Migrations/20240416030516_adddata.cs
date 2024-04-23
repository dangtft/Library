using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API_LIBRARY.Migrations
{
    /// <inheritdoc />
    public partial class adddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "FullName" },
                values: new object[,]
                {
                    { 1, "Author 1" },
                    { 2, "Author 2" }
                });

            migrationBuilder.InsertData(
                table: "Publisher",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Publisher A" },
                    { 2, "Publisher B" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "CoverUrl", "DateAdded", "DateRead", "Description", "Genre", "PublisherId", "Title", "isRead" },
                values: new object[,]
                {
                    { 1, "https://example.com/book1cover", new DateTime(2024, 4, 16, 10, 5, 15, 833, DateTimeKind.Local).AddTicks(6675), new DateTime(2024, 4, 16, 10, 5, 15, 833, DateTimeKind.Local).AddTicks(6652), "Description of Book 1", 1, 1, "Book 1", true },
                    { 2, "https://example.com/book2cover", new DateTime(2024, 4, 16, 10, 5, 15, 833, DateTimeKind.Local).AddTicks(6681), null, "Description of Book 2", 1, 2, "Book 2", false }
                });

            migrationBuilder.InsertData(
                table: "BookAuthor",
                columns: new[] { "Id", "AuthorId", "BookId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BookAuthor",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "BookAuthor",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Publisher",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Publisher",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
