using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorIdToExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "Exams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exams_DoctorId",
                table: "Exams",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Doctors_DoctorId",
                table: "Exams",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Doctors_DoctorId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_DoctorId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Exams");
        }
    }
}
