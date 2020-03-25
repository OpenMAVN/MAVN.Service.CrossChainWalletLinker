using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.CrossChainWalletLinker.MsSqlRepositories.Migrations
{
    public partial class LinkingCounters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "wallet_linking_requests_counter",
                schema: "cross_chain_wallet_linker",
                columns: table => new
                {
                    customer_id = table.Column<string>(nullable: false),
                    approvals_counter = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallet_linking_requests_counter", x => x.customer_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wallet_linking_requests_counter",
                schema: "cross_chain_wallet_linker");
        }
    }
}
