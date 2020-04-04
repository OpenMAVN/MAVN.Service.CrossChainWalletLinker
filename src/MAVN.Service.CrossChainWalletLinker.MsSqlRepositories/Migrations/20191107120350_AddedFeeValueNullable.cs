using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.CrossChainWalletLinker.MsSqlRepositories.Migrations
{
    public partial class AddedFeeValueNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "fee",
                schema: "cross_chain_wallet_linker",
                table: "wallet_linking_requests",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "fee",
                schema: "cross_chain_wallet_linker",
                table: "wallet_linking_requests",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
