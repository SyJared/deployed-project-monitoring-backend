using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Deployed_Project_Monitoring_Backend.Migrations
{
    /// <inheritdoc />
    public partial class apikeyInProjectsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "api_key_hash",
                table: "Projects",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_api_key_hash",
                table: "Projects",
                column: "api_key_hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_api_key_hash",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "api_key_hash",
                table: "Projects");
        }
    }
}
