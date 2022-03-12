using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Presentation.Migrations.Parbad
{
    public partial class Parbad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "PaymentTable",
                schema: "public",
                columns: table => new {
                    payment_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tracking_number = table.Column<long>(type: "bigint", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    transaction_code = table.Column<string>(type: "text", nullable: true),
                    gateway_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    gateway_account_name = table.Column<string>(type: "text", nullable: true),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    is_paid = table.Column<bool>(type: "boolean", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table => { table.PrimaryKey("payment_id", x => x.payment_id); });

            migrationBuilder.CreateTable(
                name: "TransactionTable",
                schema: "public",
                columns: table => new {
                    transaction_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    type = table.Column<byte>(type: "smallint", nullable: false),
                    is_succeed = table.Column<bool>(type: "boolean", nullable: false),
                    message = table.Column<string>(type: "text", nullable: true),
                    additional_data = table.Column<string>(type: "text", nullable: true),
                    PaymentId = table.Column<long>(type: "bigint", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("transaction_id", x => x.transaction_id);
                    table.ForeignKey(
                        name: "FK_TransactionTable_PaymentTable_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "public",
                        principalTable: "PaymentTable",
                        principalColumn: "payment_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTable_token",
                schema: "public",
                table: "PaymentTable",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTable_tracking_number",
                schema: "public",
                table: "PaymentTable",
                column: "tracking_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTable_PaymentId",
                schema: "public",
                table: "TransactionTable",
                column: "PaymentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionTable",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PaymentTable",
                schema: "public");
        }
    }
}