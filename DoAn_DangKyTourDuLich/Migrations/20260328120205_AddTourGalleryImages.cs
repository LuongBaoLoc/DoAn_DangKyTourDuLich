using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn_DangKyTourDuLich.Migrations
{
    public partial class AddTourGalleryImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrlsData",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrlsData",
                table: "Tours");
        }
    }
}
