using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModernBillingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VContact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VGST = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VPincode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}
