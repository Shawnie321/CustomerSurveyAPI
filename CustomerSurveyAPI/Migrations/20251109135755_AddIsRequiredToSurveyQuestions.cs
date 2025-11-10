using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerSurveyAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRequiredToSurveyQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "SurveyQuestions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "SurveyQuestions");
        }
    }
}
