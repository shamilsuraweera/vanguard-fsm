using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VanguardFSM.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedWorkers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Standard maintenance.");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "IsAvailable", "LastKnownLocation", "Name", "Role" },
                values: new object[,]
                {
                    { 1, true, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-0.14 51.5)"), "Close Worker (1km away)", "Worker" },
                    { 2, true, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-0.4543 51.47)"), "Far Worker (20km away)", "Worker" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Standard maintenance and filter change.");
        }
    }
}
