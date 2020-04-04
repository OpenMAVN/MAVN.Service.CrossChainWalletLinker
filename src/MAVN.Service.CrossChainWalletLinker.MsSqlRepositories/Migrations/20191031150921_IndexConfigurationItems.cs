using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.CrossChainWalletLinker.MsSqlRepositories.Migrations
{
    public partial class IndexConfigurationItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_configuration_items",
                schema: "cross_chain_wallet_linker",
                table: "configuration_items");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                schema: "cross_chain_wallet_linker",
                table: "configuration_items",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "id",
                schema: "cross_chain_wallet_linker",
                table: "configuration_items",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_configuration_items",
                schema: "cross_chain_wallet_linker",
                table: "configuration_items",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_configuration_items_type",
                schema: "cross_chain_wallet_linker",
                table: "configuration_items",
                column: "type",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_configuration_items",
                schema: "cross_chain_wallet_linker",
                table: "configuration_items");

            migrationBuilder.DropIndex(
                name: "IX_configuration_items_type",
                schema: "cross_chain_wallet_linker",
                table: "configuration_items");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "cross_chain_wallet_linker",
                table: "configuration_items");

            migrationBuilder.AlterColumn<int>(
                name: "type",
                schema: "cross_chain_wallet_linker",
                table: "configuration_items",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_configuration_items",
                schema: "cross_chain_wallet_linker",
                table: "configuration_items",
                column: "type");
        }
    }
}
