using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dangNhapDangKy.Migrations
{
    /// <inheritdoc />
    public partial class Adsizeinorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "OrderItem",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "OrderItem");
        }
    }
}
