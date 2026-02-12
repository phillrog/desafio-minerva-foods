using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DesafioMinervaFoods.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfigurarFKsObrigatorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentConditionId",
                table: "Orders",
                column: "PaymentConditionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PaymentConditions_PaymentConditionId",
                table: "Orders",
                column: "PaymentConditionId",
                principalTable: "PaymentConditions",
                principalColumn: "PaymentConditionId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PaymentConditions_PaymentConditionId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentConditionId",
                table: "Orders");
        }
    }
}
