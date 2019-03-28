using Microsoft.EntityFrameworkCore.Migrations;

namespace AliasMailApi.Migrations
{
    public partial class originaldate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalDate",
                table: "Messages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalDate",
                table: "Messages");
        }
    }
}
