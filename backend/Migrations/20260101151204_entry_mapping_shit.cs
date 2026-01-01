using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mess.Migrations
{
    /// <inheritdoc />
    public partial class entry_mapping_shit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_Members_AssociatedMemberId",
                table: "Participant");

            migrationBuilder.RenameColumn(
                name: "AssociatedMemberId",
                table: "Participant",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Participant_AssociatedMemberId",
                table: "Participant",
                newName: "IX_Participant_MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_Members_MemberId",
                table: "Participant",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_Members_MemberId",
                table: "Participant");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "Participant",
                newName: "AssociatedMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Participant_MemberId",
                table: "Participant",
                newName: "IX_Participant_AssociatedMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_Members_AssociatedMemberId",
                table: "Participant",
                column: "AssociatedMemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
