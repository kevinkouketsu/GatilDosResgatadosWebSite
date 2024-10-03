using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GatilDosResgatadosApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixedMigrationsBecauseIAmDumb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Pets",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Pets",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "PetMedias",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", maxLength: 10485760, nullable: true),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    OwnerId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PetMedias_Pets_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PetMedias_OwnerId",
                table: "PetMedias",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PetMedias");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Pets",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Pets",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);
        }
    }
}
