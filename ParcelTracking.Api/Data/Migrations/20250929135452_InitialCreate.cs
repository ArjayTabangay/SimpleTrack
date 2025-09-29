using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ParcelTracking.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Parcels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackingNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcels", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Parcels",
                columns: new[] { "Id", "CreatedAt", "Status", "TrackingNumber", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("5655145f-0137-438f-9152-44924c521402"), new DateTime(2025, 9, 28, 13, 54, 52, 247, DateTimeKind.Utc).AddTicks(3701), "Delivered", "TRK002", new DateTime(2025, 9, 29, 11, 54, 52, 247, DateTimeKind.Utc).AddTicks(3776) },
                    { new Guid("a210a62d-6896-4884-915c-7c641921b9bf"), new DateTime(2025, 9, 29, 13, 54, 52, 247, DateTimeKind.Utc).AddTicks(3331), "In Transit", "TRK001", new DateTime(2025, 9, 29, 13, 54, 52, 247, DateTimeKind.Utc).AddTicks(3514) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_TrackingNumber",
                table: "Parcels",
                column: "TrackingNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parcels");
        }
    }
}
