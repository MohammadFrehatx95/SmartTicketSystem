using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentAndRequestBody : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Agents");

            migrationBuilder.AddColumn<string>(
                name: "RequestBody",
                table: "IdempotencyKeys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AssignmentSource",
                table: "AssignmentAttempts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                table: "Agents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agents_DepartmentId",
                table: "Agents",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_Departments_DepartmentId",
                table: "Agents",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agents_Departments_DepartmentId",
                table: "Agents");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Agents_DepartmentId",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "RequestBody",
                table: "IdempotencyKeys");

            migrationBuilder.DropColumn(
                name: "AssignmentSource",
                table: "AssignmentAttempts");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Agents");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Agents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
