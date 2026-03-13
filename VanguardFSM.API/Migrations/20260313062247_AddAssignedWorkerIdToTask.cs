using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VanguardFSM.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedWorkerIdToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedWorkerId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 1,
                column: "AssignedWorkerId",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedWorkerId",
                table: "Tasks");
        }
    }
}
