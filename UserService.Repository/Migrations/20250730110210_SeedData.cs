using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserService.Repository.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[,]
                {
                    { 1, "admin@techshop.vn", "Nguyễn Văn Admin", "$2a$11$qTDzHOexheYCrZsBApEeyu3QzUzRodJ1tLRaEB4nl97vgLl3mVW4y", "Admin" },
                    { 2, "manager@techshop.vn", "Trần Thị Manager", "$2a$11$zI8QzMGc83g/CmPQbDOLKONWrU1Da1ubC/7dLlLGkTQl7P/sAmkD6", "Manager" },
                    { 3, "user1@gmail.com", "Lê Văn User", "$2a$11$LzjQbfFYlybLeXWrF2OKYeUuZ1DEFlzchVE.1t3IgNQr29vzwhbMy", "User" },
                    { 4, "user2@gmail.com", "Phạm Thị Hương", "$2a$11$YURV0z1hAa2g0Gz05amO/eXOfnM9.mTssh0Mn6ftgQW9gQxDzNcCS", "User" },
                    { 5, "customer@techshop.vn", "Hoàng Minh Khách", "$2a$11$2p2sgLHOHAWQoSi2SM7OregbHoWaAMZSmJWiR3b0H0rCUZ/qMeD2S", "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
