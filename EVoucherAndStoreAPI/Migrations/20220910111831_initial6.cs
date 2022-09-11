using Microsoft.EntityFrameworkCore.Migrations;

namespace EVoucherAndStoreAPI.Migrations
{
    public partial class initial6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EVouchers_PaymenMethods_DiscountPaymentMethodId",
                table: "EVouchers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymenMethods",
                table: "PaymenMethods");

            migrationBuilder.RenameTable(
                name: "PaymenMethods",
                newName: "PaymentMethods");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethodName",
                table: "PaymentMethods",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentMethods",
                table: "PaymentMethods",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_PaymentMethodName",
                table: "PaymentMethods",
                column: "PaymentMethodName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EVouchers_PaymentMethods_DiscountPaymentMethodId",
                table: "EVouchers",
                column: "DiscountPaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EVouchers_PaymentMethods_DiscountPaymentMethodId",
                table: "EVouchers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentMethods",
                table: "PaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethods_PaymentMethodName",
                table: "PaymentMethods");

            migrationBuilder.RenameTable(
                name: "PaymentMethods",
                newName: "PaymenMethods");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethodName",
                table: "PaymenMethods",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymenMethods",
                table: "PaymenMethods",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_EVouchers_PaymenMethods_DiscountPaymentMethodId",
                table: "EVouchers",
                column: "DiscountPaymentMethodId",
                principalTable: "PaymenMethods",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
