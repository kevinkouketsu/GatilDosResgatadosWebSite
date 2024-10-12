using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GatilDosResgatadosApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedImageToSubscriptionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Subscriptions",
                type: "bytea",
                maxLength: 10485760,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Subscriptions");
        }
    }
}
