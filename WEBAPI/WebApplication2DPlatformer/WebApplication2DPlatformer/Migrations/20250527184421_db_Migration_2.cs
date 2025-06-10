using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2DPlatformer.Migrations
{
    /// <inheritdoc />
    public partial class db_Migration_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LevelsProgress_Level_ID",
                table: "LevelsProgress",
                column: "Level_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LevelsProgress_User_ID",
                table: "LevelsProgress",
                column: "User_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_LevelsProgress_Levels_Level_ID",
                table: "LevelsProgress",
                column: "Level_ID",
                principalTable: "Levels",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LevelsProgress_Users_User_ID",
                table: "LevelsProgress",
                column: "User_ID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LevelsProgress_Levels_Level_ID",
                table: "LevelsProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_LevelsProgress_Users_User_ID",
                table: "LevelsProgress");

            migrationBuilder.DropIndex(
                name: "IX_LevelsProgress_Level_ID",
                table: "LevelsProgress");

            migrationBuilder.DropIndex(
                name: "IX_LevelsProgress_User_ID",
                table: "LevelsProgress");
        }
    }
}
