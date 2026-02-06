using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OaeCrossTrackApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMeetScoreFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OpponentScore",
                table: "Meets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OurScore",
                table: "Meets",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpponentScore",
                table: "Meets");

            migrationBuilder.DropColumn(
                name: "OurScore",
                table: "Meets");
        }
    }
}
