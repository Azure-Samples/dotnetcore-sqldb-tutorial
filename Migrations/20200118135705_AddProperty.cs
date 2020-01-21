using Microsoft.EntityFrameworkCore.Migrations;

namespace DotNetCoreSqlDb.Migrations
{
    public partial class AddProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Done",
                table: "Todo",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Done",
                table: "Todo");
        }
    }
}
