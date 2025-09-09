using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECareClinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordRecoveryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "passwordResetVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OtpCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passwordResetVerifications", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "passwordResetVerifications");
        }
    }
}
