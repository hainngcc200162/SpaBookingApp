using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaBookingApp.Migrations
{
    /// <inheritdoc />
    public partial class _1111TenMig1rationDauTien1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpaProducts_Categories_CategoryId",
                table: "SpaProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_SpaProducts_Categories_CategoryId",
                table: "SpaProducts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpaProducts_Categories_CategoryId",
                table: "SpaProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_SpaProducts_Categories_CategoryId",
                table: "SpaProducts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
