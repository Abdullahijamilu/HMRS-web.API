using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMRS_web.API.Migrations
{
    /// <inheritdoc />
    public partial class AddJobRoleAndUpdateEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_UploadedBy",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Department",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_JobRole",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_UploadedBy",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "UploadedBy",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Employees",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_UserId",
                table: "Employees",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Department",
                table: "Employees",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_JobRole",
                table: "Employees",
                column: "JobRoleId",
                principalTable: "JobRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AspNetUsers_UserId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Department",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_JobRole",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_UserId",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Employees",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadedBy",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UploadedBy",
                table: "Employees",
                column: "UploadedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AspNetUsers_UploadedBy",
                table: "Employees",
                column: "UploadedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Department",
                table: "Employees",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_JobRole",
                table: "Employees",
                column: "JobRoleId",
                principalTable: "JobRoles",
                principalColumn: "Id");
        }
    }
}
