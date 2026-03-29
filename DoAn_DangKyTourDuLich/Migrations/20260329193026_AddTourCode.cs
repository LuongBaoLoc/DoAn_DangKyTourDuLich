using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn_DangKyTourDuLich.Migrations
{
    /// <inheritdoc />
    public partial class AddTourCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TourCode",
                table: "Tours",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TourCode",
                table: "Tours");
        }
    }
}
