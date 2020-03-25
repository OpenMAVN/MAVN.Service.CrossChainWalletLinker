using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.CrossChainWalletLinker.MsSqlRepositories.Migrations
{
    public partial class AddedFeeValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "fee",
                schema: "cross_chain_wallet_linker",
                table: "wallet_linking_requests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fee",
                schema: "cross_chain_wallet_linker",
                table: "wallet_linking_requests");
        }
    }
}
