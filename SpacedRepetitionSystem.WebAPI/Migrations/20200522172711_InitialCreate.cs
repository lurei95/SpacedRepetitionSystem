using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SpacedRepetitionSystem.WebAPI.Migrations
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
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTemplates", x => x.CardTemplateId);
                    table.ForeignKey(
                        name: "FK_CardTemplates_Users_UserId",
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
                    FieldId = table.Column<int>(nullable: false),
                    CardTemplateId = table.Column<long>(nullable: false),
                    FieldName = table.Column<string>(maxLength: 100, nullable: false),
                    ShowInputForPractice = table.Column<bool>(nullable: false),
                    IsRequired = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardFieldDefinitions", x => new { x.CardTemplateId, x.FieldId });
                    table.ForeignKey(
                        name: "FK_CardFieldDefinitions_CardTemplates_CardTemplateId",
                        column: x => x.CardTemplateId,
                        principalSchema: "Cards",
                        principalTable: "CardTemplates",
                        principalColumn: "CardTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Decks",
                schema: "Cards",
                columns: table => new
                {
                    DeckId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    IsPinned = table.Column<bool>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    DefaultCardTemplateId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decks", x => x.DeckId);
                    table.ForeignKey(
                        name: "FK_Decks_CardTemplates_DefaultCardTemplateId",
                        column: x => x.DefaultCardTemplateId,
                        principalSchema: "Cards",
                        principalTable: "CardTemplates",
                        principalColumn: "CardTemplateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Decks_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                schema: "Cards",
                columns: table => new
                {
                    CardId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tags = table.Column<string>(maxLength: 256, nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    CardTemplateId = table.Column<long>(nullable: false),
                    DeckId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.CardId);
                    table.ForeignKey(
                        name: "FK_Cards_CardTemplates_CardTemplateId",
                        column: x => x.CardTemplateId,
                        principalSchema: "Cards",
                        principalTable: "CardTemplates",
                        principalColumn: "CardTemplateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cards_Decks_DeckId",
                        column: x => x.DeckId,
                        principalSchema: "Cards",
                        principalTable: "Decks",
                        principalColumn: "DeckId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cards_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "CardFields",
                schema: "Cards",
                columns: table => new
                {
                    FieldId = table.Column<int>(nullable: false),
                    CardId = table.Column<long>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    DueDate = table.Column<DateTime>(nullable: false),
                    ProficiencyLevel = table.Column<int>(nullable: false),
                    CardTemplateId = table.Column<long>(nullable: false),
                    FieldName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardFields", x => new { x.CardId, x.FieldId });
                    table.ForeignKey(
                        name: "FK_CardFields_Cards_CardId",
                        column: x => x.CardId,
                        principalSchema: "Cards",
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardFields_CardTemplates_CardTemplateId",
                        column: x => x.CardTemplateId,
                        principalSchema: "Cards",
                        principalTable: "CardTemplates",
                        principalColumn: "CardTemplateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CardFields_CardFieldDefinitions_CardTemplateId_FieldId",
                        columns: x => new { x.CardTemplateId, x.FieldId },
                        principalSchema: "Cards",
                        principalTable: "CardFieldDefinitions",
                        principalColumns: new[] { "CardTemplateId", "FieldId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PracticeHistoryEntries",
                schema: "Cards",
                columns: table => new
                {
                    PracticeHistoryEntryId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PracticeDate = table.Column<DateTime>(nullable: false),
                    WrongCount = table.Column<int>(nullable: false),
                    HardCount = table.Column<int>(nullable: false),
                    CorrectCount = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    CardId = table.Column<long>(nullable: false),
                    DeckId = table.Column<long>(nullable: false),
                    FieldId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeHistoryEntries", x => x.PracticeHistoryEntryId);
                    table.ForeignKey(
                        name: "FK_PracticeHistoryEntries_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_PracticeHistoryEntries_CardFields_CardId_FieldId",
                        columns: x => new { x.CardId, x.FieldId },
                        principalSchema: "Cards",
                        principalTable: "CardFields",
                        principalColumns: new[] { "CardId", "FieldId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardFieldDefinitions_FieldName",
                schema: "Cards",
                table: "CardFieldDefinitions",
                column: "FieldName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardFields_CardId_FieldName",
                schema: "Cards",
                table: "CardFields",
                columns: new[] { "CardId", "FieldName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardFields_CardTemplateId_FieldId",
                schema: "Cards",
                table: "CardFields",
                columns: new[] { "CardTemplateId", "FieldId" });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardTemplateId",
                schema: "Cards",
                table: "Cards",
                column: "CardTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_DeckId",
                schema: "Cards",
                table: "Cards",
                column: "DeckId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_UserId",
                schema: "Cards",
                table: "Cards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CardTemplates_UserId",
                schema: "Cards",
                table: "CardTemplates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Decks_DefaultCardTemplateId",
                schema: "Cards",
                table: "Decks",
                column: "DefaultCardTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Decks_UserId",
                schema: "Cards",
                table: "Decks",
                column: "UserId");

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
                name: "IX_PracticeHistoryEntries_UserId",
                schema: "Cards",
                table: "PracticeHistoryEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeHistoryEntries_CardId_FieldId",
                schema: "Cards",
                table: "PracticeHistoryEntries",
                columns: new[] { "CardId", "FieldId" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "Security",
                table: "RefreshTokens",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PracticeHistoryEntries",
                schema: "Cards");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "CardFields",
                schema: "Cards");

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
