using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerSurveyAPI.Migrations
{
    public partial class BackfillUserFieldsWithDefaults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Backfill strings that were set to empty string by previous migration.
            // Build email from username to reduce risk of uniqueness conflicts.
            migrationBuilder.Sql(@"
UPDATE ""Users""
SET ""FirstName"" = 'User'
WHERE ""FirstName"" = '';

UPDATE ""Users""
SET ""LastName"" = ""Username""
WHERE ""LastName"" = '';

UPDATE ""Users""
SET ""Email"" = COALESCE(NULLIF(""Username"", ''''), ''user'') || ''.'' || ""Id""::text || ''@example.local''
WHERE ""Email"" IS NULL OR ""Email"" = '''';

-- Replace sentinel DateOnly(0001-01-01) with a safe placeholder (1900-01-01)
UPDATE ""Users""
SET ""DateOfBirth"" = DATE ''1900-01-01''
WHERE ""DateOfBirth"" = DATE ''0001-01-01'';
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert only if needed — restore to the original defaults used by previous migration.
            migrationBuilder.Sql(@"
UPDATE ""Users""
SET ""FirstName"" = ''
WHERE ""FirstName"" = 'User';

UPDATE ""Users""
SET ""LastName"" = ''
WHERE ""LastName"" = ""Username"";

UPDATE ""Users""
SET ""Email"" = ''
WHERE ""Email"" LIKE ''%@example.local'';

UPDATE ""Users""
SET ""DateOfBirth"" = DATE ''0001-01-01'''
WHERE ""DateOfBirth"" = DATE ''1900-01-01'';
");
        }
    }
}