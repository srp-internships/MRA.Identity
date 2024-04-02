using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MRA.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Applications");
        }
    }
}
