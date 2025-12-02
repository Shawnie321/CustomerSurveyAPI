using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerSurveyAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddConsentGivenToSurveyResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConsentGiven",
                table: "SurveyResponses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsentGiven",
                table: "SurveyResponses");
        }
    }
}
