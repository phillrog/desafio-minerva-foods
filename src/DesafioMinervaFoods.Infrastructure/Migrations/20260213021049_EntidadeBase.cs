using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DesafioMinervaFoods.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EntidadeBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PaymentConditions_PaymentConditionId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentConditions",
                table: "PaymentConditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryTerms",
                table: "DeliveryTerms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "PaymentConditionId",
                table: "PaymentConditions",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Orders",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "OrderItemId",
                table: "OrderItems",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "DeliveryTermId",
                table: "DeliveryTerms",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Customers",
                newName: "CreatedBy");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PaymentConditions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PaymentConditions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "PaymentConditions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "OrderItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OrderItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OrderItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "OrderItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "DeliveryTerms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DeliveryTerms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "DeliveryTerms",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentConditions",
                table: "PaymentConditions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryTerms",
                table: "DeliveryTerms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryTerms_OrderId",
                table: "DeliveryTerms",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryTerms_Orders_OrderId",
                table: "DeliveryTerms",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PaymentConditions_PaymentConditionId",
                table: "Orders",
                column: "PaymentConditionId",
                principalTable: "PaymentConditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryTerms_Orders_OrderId",
                table: "DeliveryTerms");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PaymentConditions_PaymentConditionId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentConditions",
                table: "PaymentConditions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryTerms",
                table: "DeliveryTerms");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryTerms_OrderId",
                table: "DeliveryTerms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PaymentConditions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PaymentConditions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "PaymentConditions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DeliveryTerms");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DeliveryTerms");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "DeliveryTerms");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "PaymentConditions",
                newName: "PaymentConditionId");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Orders",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "OrderItems",
                newName: "OrderItemId");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "DeliveryTerms",
                newName: "DeliveryTermId");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Customers",
                newName: "CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentConditions",
                table: "PaymentConditions",
                column: "PaymentConditionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems",
                column: "OrderItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryTerms",
                table: "DeliveryTerms",
                column: "DeliveryTermId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

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
    }
}
