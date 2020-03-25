using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.CrossChainWalletLinker.MsSqlRepositories.Migrations
{
    public partial class AddConfigurationItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "configuration_items",
                schema: "cross_chain_wallet_linker",
                columns: table => new
                {
                    type = table.Column<int>(nullable: false),
                    value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configuration_items", x => x.type);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "configuration_items",
                schema: "cross_chain_wallet_linker");
        }
    }
}
