using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.CrossChainWalletLinker.MsSqlRepositories.Migrations
{
    public partial class AddPrivateAddressIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "private_address",
                schema: "cross_chain_wallet_linker",
                table: "wallet_linking_requests",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_wallet_linking_requests_private_address",
                schema: "cross_chain_wallet_linker",
                table: "wallet_linking_requests",
                column: "private_address",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_wallet_linking_requests_private_address",
                schema: "cross_chain_wallet_linker",
                table: "wallet_linking_requests");

            migrationBuilder.AlterColumn<string>(
                name: "private_address",
                schema: "cross_chain_wallet_linker",
                table: "wallet_linking_requests",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
