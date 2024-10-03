using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GatilDosResgatadosApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedNullableFromDataFromPetMedias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Data",
                table: "PetMedias",
                type: "bytea",
                maxLength: 10485760,
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldMaxLength: 10485760,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Data",
                table: "PetMedias",
                type: "bytea",
                maxLength: 10485760,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldMaxLength: 10485760);
        }
    }
}
