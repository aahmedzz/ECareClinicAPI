using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECareClinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_ScheduleTb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeOfVisit",
                table: "Appointments");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VisitTypeId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DoctorSchedules",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorSchedules", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_DoctorSchedules_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VisitType",
                columns: table => new
                {
                    VisitTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitType", x => x.VisitTypeId);
                });

            migrationBuilder.CreateTable(
                name: "DoctorVisitTypes",
                columns: table => new
                {
                    DoctorVisitTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VisitTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorVisitTypes", x => x.DoctorVisitTypeId);
                    table.ForeignKey(
                        name: "FK_DoctorVisitTypes_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoctorVisitTypes_VisitType_VisitTypeId",
                        column: x => x.VisitTypeId,
                        principalTable: "VisitType",
                        principalColumn: "VisitTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ScheduleId",
                table: "Appointments",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_VisitTypeId",
                table: "Appointments",
                column: "VisitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSchedules_DoctorId",
                table: "DoctorSchedules",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorVisitTypes_DoctorId",
                table: "DoctorVisitTypes",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorVisitTypes_VisitTypeId",
                table: "DoctorVisitTypes",
                column: "VisitTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_DoctorSchedules_ScheduleId",
                table: "Appointments",
                column: "ScheduleId",
                principalTable: "DoctorSchedules",
                principalColumn: "ScheduleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_VisitType_VisitTypeId",
                table: "Appointments",
                column: "VisitTypeId",
                principalTable: "VisitType",
                principalColumn: "VisitTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_DoctorSchedules_ScheduleId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_VisitType_VisitTypeId",
                table: "Appointments");

            migrationBuilder.DropTable(
                name: "DoctorSchedules");

            migrationBuilder.DropTable(
                name: "DoctorVisitTypes");

            migrationBuilder.DropTable(
                name: "VisitType");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ScheduleId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_VisitTypeId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "VisitTypeId",
                table: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "TypeOfVisit",
                table: "Appointments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
