using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.CrossChainWalletLinker.MsSqlRepositories.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cross_chain_wallet_linker");

            migrationBuilder.CreateTable(
                name: "wallet_linking_requests",
                schema: "cross_chain_wallet_linker",
                columns: table => new
                {
                    customer_id = table.Column<string>(nullable: false),
                    public_address = table.Column<string>(nullable: true),
                    private_address = table.Column<string>(nullable: false),
                    link_code = table.Column<string>(nullable: false),
                    is_confirmed_in_private = table.Column<bool>(nullable: false),
                    is_confirmed_in_public = table.Column<bool>(nullable: false),
                    signature = table.Column<string>(nullable: true),
                    created_on = table.Column<DateTime>(nullable: false),
                    timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallet_linking_requests", x => x.customer_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wallet_linking_requests",
                schema: "cross_chain_wallet_linker");
        }
    }
}
