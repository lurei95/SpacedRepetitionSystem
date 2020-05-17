using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SpacedRepetitionSystem.Entities.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Cards");

            migrationBuilder.EnsureSchema(
                name: "Security");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Security",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(maxLength: 256, nullable: false),
                    Password = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "CardTemplates",
                schema: "Cards",
                columns: table => new
                {
                    CardTemplateId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTemplates", x => new { x.UserId, x.CardTemplateId });
                    table.ForeignKey(
                        name: "FK_CardTemplates_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PracticeHistoryEntries",
                schema: "Cards",
                columns: table => new
                {
                    PracticeHistoryEntryId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(nullable: false),
                    PracticeDate = table.Column<DateTime>(nullable: false),
                    WrongCount = table.Column<int>(nullable: false),
                    HardCount = table.Column<int>(nullable: false),
                    CorrectCount = table.Column<int>(nullable: false),
                    CardId = table.Column<long>(nullable: false),
                    DeckId = table.Column<long>(nullable: false),
                    FieldName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeHistoryEntries", x => new { x.UserId, x.PracticeHistoryEntryId });
                    table.ForeignKey(
                        name: "FK_PracticeHistoryEntries_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "Security",
                columns: table => new
                {
                    TokenId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(nullable: false),
                    Token = table.Column<string>(unicode: false, maxLength: 200, nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardFieldDefinitions",
                schema: "Cards",
                columns: table => new
                {
                    FieldName = table.Column<string>(maxLength: 100, nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    CardTemplateId = table.Column<long>(nullable: false),
                    ShowInputForPractice = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardFieldDefinitions", x => new { x.UserId, x.CardTemplateId, x.FieldName });
                    table.ForeignKey(
                        name: "FK_CardFieldDefinitions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_CardFieldDefinitions_CardTemplates_UserId_CardTemplateId",
                        columns: x => new { x.UserId, x.CardTemplateId },
                        principalSchema: "Cards",
                        principalTable: "CardTemplates",
                        principalColumns: new[] { "UserId", "CardTemplateId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Decks",
                schema: "Cards",
                columns: table => new
                {
                    DeckId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    IsPinned = table.Column<bool>(nullable: false),
                    DefaultCardTemplateId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decks", x => new { x.UserId, x.DeckId });
                    table.ForeignKey(
                        name: "FK_Decks_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Decks_CardTemplates_UserId_DefaultCardTemplateId",
                        columns: x => new { x.UserId, x.DefaultCardTemplateId },
                        principalSchema: "Cards",
                        principalTable: "CardTemplates",
                        principalColumns: new[] { "UserId", "CardTemplateId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                schema: "Cards",
                columns: table => new
                {
                    CardId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(nullable: false),
                    Tags = table.Column<string>(maxLength: 256, nullable: false),
                    CardTemplateId = table.Column<long>(nullable: false),
                    DeckId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => new { x.UserId, x.CardId });
                    table.ForeignKey(
                        name: "FK_Cards_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Cards_CardTemplates_UserId_CardTemplateId",
                        columns: x => new { x.UserId, x.CardTemplateId },
                        principalSchema: "Cards",
                        principalTable: "CardTemplates",
                        principalColumns: new[] { "UserId", "CardTemplateId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cards_Decks_UserId_DeckId",
                        columns: x => new { x.UserId, x.DeckId },
                        principalSchema: "Cards",
                        principalTable: "Decks",
                        principalColumns: new[] { "UserId", "DeckId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardFields",
                schema: "Cards",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    CardId = table.Column<long>(nullable: false),
                    FieldName = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    DueDate = table.Column<DateTime>(nullable: false),
                    ProficiencyLevel = table.Column<int>(nullable: false),
                    CardTemplateId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardFields", x => new { x.UserId, x.CardId, x.FieldName });
                    table.ForeignKey(
                        name: "FK_CardFields_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_CardFields_Cards_UserId_CardId",
                        columns: x => new { x.UserId, x.CardId },
                        principalSchema: "Cards",
                        principalTable: "Cards",
                        principalColumns: new[] { "UserId", "CardId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardFields_CardTemplates_UserId_CardTemplateId",
                        columns: x => new { x.UserId, x.CardTemplateId },
                        principalSchema: "Cards",
                        principalTable: "CardTemplates",
                        principalColumns: new[] { "UserId", "CardTemplateId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CardFields_CardFieldDefinitions_UserId_CardTemplateId_FieldName",
                        columns: x => new { x.UserId, x.CardTemplateId, x.FieldName },
                        principalSchema: "Cards",
                        principalTable: "CardFieldDefinitions",
                        principalColumns: new[] { "UserId", "CardTemplateId", "FieldName" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardFields_UserId_CardTemplateId_FieldName",
                schema: "Cards",
                table: "CardFields",
                columns: new[] { "UserId", "CardTemplateId", "FieldName" });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_UserId_CardTemplateId",
                schema: "Cards",
                table: "Cards",
                columns: new[] { "UserId", "CardTemplateId" });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_UserId_DeckId",
                schema: "Cards",
                table: "Cards",
                columns: new[] { "UserId", "DeckId" });

            migrationBuilder.CreateIndex(
                name: "IX_Decks_UserId_DefaultCardTemplateId",
                schema: "Cards",
                table: "Decks",
                columns: new[] { "UserId", "DefaultCardTemplateId" });

            migrationBuilder.CreateIndex(
                name: "IX_PracticeHistoryEntries_CardId",
                schema: "Cards",
                table: "PracticeHistoryEntries",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeHistoryEntries_DeckId",
                schema: "Cards",
                table: "PracticeHistoryEntries",
                column: "DeckId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "Security",
                table: "RefreshTokens",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardFields",
                schema: "Cards");

            migrationBuilder.DropTable(
                name: "PracticeHistoryEntries",
                schema: "Cards");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "Cards",
                schema: "Cards");

            migrationBuilder.DropTable(
                name: "CardFieldDefinitions",
                schema: "Cards");

            migrationBuilder.DropTable(
                name: "Decks",
                schema: "Cards");

            migrationBuilder.DropTable(
                name: "CardTemplates",
                schema: "Cards");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Security");
        }
    }
}
