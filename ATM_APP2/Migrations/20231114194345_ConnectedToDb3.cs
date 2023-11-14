using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATM_APP2.Migrations
{
    public partial class ConnectedToDb3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_BookId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "Transactions",
                newName: "CardId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_BookId",
                table: "Transactions",
                newName: "IX_Transactions_CardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_CardId",
                table: "Transactions",
                column: "CardId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_CardId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "CardId",
                table: "Transactions",
                newName: "BookId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_CardId",
                table: "Transactions",
                newName: "IX_Transactions_BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_BookId",
                table: "Transactions",
                column: "BookId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
