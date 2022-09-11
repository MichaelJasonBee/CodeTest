using Microsoft.EntityFrameworkCore.Migrations;

namespace EVoucherAndStoreAPI.Migrations
{
    public partial class initial8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentMethods",
                table: "EVouchers",
                newName: "AvailablePaymentMethods");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailablePaymentMethods",
                table: "EVouchers",
                newName: "PaymentMethods");
        }
    }
}
