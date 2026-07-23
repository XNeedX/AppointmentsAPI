using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentsAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReceptionistFromAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Receptionists_ReceptionistId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ReceptionistId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ReceptionistId",
                table: "Appointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReceptionistId",
                table: "Appointments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ReceptionistId",
                table: "Appointments",
                column: "ReceptionistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Receptionists_ReceptionistId",
                table: "Appointments",
                column: "ReceptionistId",
                principalTable: "Receptionists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
