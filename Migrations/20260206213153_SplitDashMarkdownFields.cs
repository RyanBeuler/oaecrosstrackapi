using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OaeCrossTrackApi.Migrations
{
    /// <inheritdoc />
    public partial class SplitDashMarkdownFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MarkdownContent",
                table: "DashContents",
                newName: "RegistrationMarkdown");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "DashFiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Registration");

            migrationBuilder.AddColumn<string>(
                name: "CourseMapMarkdown",
                table: "DashContents",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PastResultsMarkdown",
                table: "DashContents",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "DashFiles");

            migrationBuilder.DropColumn(
                name: "CourseMapMarkdown",
                table: "DashContents");

            migrationBuilder.DropColumn(
                name: "PastResultsMarkdown",
                table: "DashContents");

            migrationBuilder.RenameColumn(
                name: "RegistrationMarkdown",
                table: "DashContents",
                newName: "MarkdownContent");
        }
    }
}
