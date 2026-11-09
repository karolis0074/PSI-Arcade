using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportMatch.API.Migrations
{
    /// <inheritdoc />
    public partial class AccountsUseIdKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQLite can't change a primary key in place, so the table is rebuilt by hand.
            // The generated version gave every existing account Id 0, which fails as soon as
            // there are two accounts. Leaving Id out of the INSERT lets SQLite number the rows.
            migrationBuilder.Sql(@"
                CREATE TABLE ""Accounts_new"" (
                    ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Accounts"" PRIMARY KEY AUTOINCREMENT,
                    ""Username"" TEXT COLLATE NOCASE NOT NULL,
                    ""PasswordHash"" TEXT NOT NULL
                );");

            migrationBuilder.Sql(@"
                INSERT INTO ""Accounts_new"" (""Username"", ""PasswordHash"")
                SELECT ""Username"", ""PasswordHash"" FROM ""Accounts"";");

            migrationBuilder.Sql(@"DROP TABLE ""Accounts"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Accounts_new"" RENAME TO ""Accounts"";");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Username",
                table: "Accounts",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // display names are gone for good, so the username is used as the display name when rolling back
            migrationBuilder.DropIndex(
                name: "IX_Accounts_Username",
                table: "Accounts");

            migrationBuilder.Sql(@"
                CREATE TABLE ""Accounts_old"" (
                    ""Username"" TEXT COLLATE NOCASE NOT NULL CONSTRAINT ""PK_Accounts"" PRIMARY KEY,
                    ""DisplayName"" TEXT NOT NULL,
                    ""PasswordHash"" TEXT NOT NULL
                );");

            migrationBuilder.Sql(@"
                INSERT INTO ""Accounts_old"" (""Username"", ""DisplayName"", ""PasswordHash"")
                SELECT ""Username"", ""Username"", ""PasswordHash"" FROM ""Accounts"";");

            migrationBuilder.Sql(@"DROP TABLE ""Accounts"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Accounts_old"" RENAME TO ""Accounts"";");
        }
    }
}
