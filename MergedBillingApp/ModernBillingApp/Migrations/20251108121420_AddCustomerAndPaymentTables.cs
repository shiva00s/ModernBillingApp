using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModernBillingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerAndPaymentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CContact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CGST = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CPincode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CardNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Points = table.Column<double>(type: "float", nullable: true),
                    CusType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutstandingBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BillNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerPayments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPayments_CustomerId",
                table: "CustomerPayments",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerPayments");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
