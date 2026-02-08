using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OaeCrossTrackApi.Migrations
{
    /// <inheritdoc />
    public partial class AddResultDetailFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AthleteId",
                table: "Results",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "HeatNumber",
                table: "Results",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RelayTeamName",
                table: "Results",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultStatus",
                table: "Results",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Wind",
                table: "Results",
                type: "numeric(5,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeatNumber",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "RelayTeamName",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "ResultStatus",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "Wind",
                table: "Results");

            migrationBuilder.AlterColumn<int>(
                name: "AthleteId",
                table: "Results",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
