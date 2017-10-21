using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jy.CRM.EntityFrameworkCore.Migrations
{
    public partial class addCRM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    City = table.Column<string>(type: "VARCHAR(200)", nullable: true),
                    Country = table.Column<string>(type: "VARCHAR(200)", nullable: true),
                    Province = table.Column<string>(type: "VARCHAR(200)", nullable: true),
                    Street = table.Column<string>(type: "VARCHAR(200)", nullable: true),
                    ZipCode = table.Column<string>(type: "VARCHAR(72)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Name = table.Column<string>(type: "VARCHAR(72)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Commodity",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Des = table.Column<string>(type: "VARCHAR(200)", nullable: true),
                    MaxGouNum = table.Column<int>(nullable: false),
                    MaxNum = table.Column<int>(nullable: false),
                    Name = table.Column<string>(type: "VARCHAR(72)", nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    Url = table.Column<string>(type: "VARCHAR(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commodity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Name = table.Column<string>(type: "VARCHAR(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    Address = table.Column<string>(type: "VARCHAR(200)", nullable: true),
                    EMail = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    MobileNumber = table.Column<string>(type: "VARCHAR(36)", nullable: true),
                    NickName = table.Column<string>(type: "VARCHAR(72)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecKillOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    AddressId = table.Column<int>(nullable: false),
                    CommodityId = table.Column<Guid>(nullable: false),
                    CreatTime = table.Column<DateTime>(nullable: false),
                    Num = table.Column<int>(nullable: false),
                    OrderStatusId = table.Column<int>(nullable: false),
                    PayOutTime = table.Column<DateTime>(nullable: false),
                    PayTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecKillOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecKillOrder_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SecKillOrder_Commodity_CommodityId",
                        column: x => x.CommodityId,
                        principalTable: "Commodity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SecKillOrder_OrderStatus_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "OrderStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SecKillOrder_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAddress",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    AddressId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddress", x => new { x.UserId, x.AddressId });
                    table.ForeignKey(
                        name: "FK_UserAddress_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAddress_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    CardTypeId = table.Column<int>(nullable: false),
                    ChannelCode = table.Column<string>(type: "VARCHAR(36)", nullable: true),
                    PaymentAccount = table.Column<string>(type: "VARCHAR(72)", nullable: true),
                    PaymentAmount = table.Column<decimal>(nullable: false),
                    PaymentMode = table.Column<string>(type: "VARCHAR(36)", nullable: true),
                    SecKillOrderId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_CardType_CardTypeId",
                        column: x => x.CardTypeId,
                        principalTable: "CardType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_SecKillOrder_SecKillOrderId",
                        column: x => x.SecKillOrderId,
                        principalTable: "SecKillOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_CardTypeId",
                table: "Payment",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_SecKillOrderId",
                table: "Payment",
                column: "SecKillOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SecKillOrder_AddressId",
                table: "SecKillOrder",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_SecKillOrder_CommodityId",
                table: "SecKillOrder",
                column: "CommodityId");

            migrationBuilder.CreateIndex(
                name: "IX_SecKillOrder_OrderStatusId",
                table: "SecKillOrder",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SecKillOrder_UserId",
                table: "SecKillOrder",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddress_AddressId",
                table: "UserAddress",
                column: "AddressId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "UserAddress");

            migrationBuilder.DropTable(
                name: "CardType");

            migrationBuilder.DropTable(
                name: "SecKillOrder");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Commodity");

            migrationBuilder.DropTable(
                name: "OrderStatus");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
