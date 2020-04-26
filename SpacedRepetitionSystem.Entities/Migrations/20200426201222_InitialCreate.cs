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

            migrationBuilder.CreateTable(
                name: "CardTemplates",
                schema: "Cards",
                columns: table => new
                {
                    CardTemplateId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTemplates", x => x.CardTemplateId);
                });

            migrationBuilder.CreateTable(
                name: "CardFieldDefinitions",
                schema: "Cards",
                columns: table => new
                {
                    FieldName = table.Column<string>(maxLength: 100, nullable: false),
                    CardTemplateId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardFieldDefinitions", x => new { x.CardTemplateId, x.FieldName });
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
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                schema: "Cards",
                columns: table => new
                {
                    CardId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tags = table.Column<string>(maxLength: 256, nullable: false),
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
                });

            migrationBuilder.CreateTable(
                name: "CardFields",
                schema: "Cards",
                columns: table => new
                {
                    CardId = table.Column<long>(nullable: false),
                    FieldName = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    CardTemplateId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardFields", x => new { x.CardId, x.FieldName });
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
                        name: "FK_CardFields_CardFieldDefinitions_CardTemplateId_FieldName",
                        columns: x => new { x.CardTemplateId, x.FieldName },
                        principalSchema: "Cards",
                        principalTable: "CardFieldDefinitions",
                        principalColumns: new[] { "CardTemplateId", "FieldName" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PracticeHistoryEntries",
                schema: "Cards",
                columns: table => new
                {
                    CardId = table.Column<long>(nullable: false),
                    CardFieldDefinitionId = table.Column<long>(nullable: false),
                    PracticeDate = table.Column<DateTime>(nullable: false),
                    PracticeResult = table.Column<int>(nullable: false),
                    CardTemplateId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeHistoryEntries", x => new { x.CardId, x.CardFieldDefinitionId });
                    table.ForeignKey(
                        name: "FK_PracticeHistoryEntries_Cards_CardId",
                        column: x => x.CardId,
                        principalSchema: "Cards",
                        principalTable: "Cards",
                        principalColumn: "CardId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PracticeHistoryEntries_CardTemplates_CardTemplateId",
                        column: x => x.CardTemplateId,
                        principalSchema: "Cards",
                        principalTable: "CardTemplates",
                        principalColumn: "CardTemplateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardFields_CardTemplateId_FieldName",
                schema: "Cards",
                table: "CardFields",
                columns: new[] { "CardTemplateId", "FieldName" });

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
                name: "IX_Decks_DefaultCardTemplateId",
                schema: "Cards",
                table: "Decks",
                column: "DefaultCardTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeHistoryEntries_CardTemplateId",
                schema: "Cards",
                table: "PracticeHistoryEntries",
                column: "CardTemplateId");
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
                name: "CardFieldDefinitions",
                schema: "Cards");

            migrationBuilder.DropTable(
                name: "Cards",
                schema: "Cards");

            migrationBuilder.DropTable(
                name: "Decks",
                schema: "Cards");

            migrationBuilder.DropTable(
                name: "CardTemplates",
                schema: "Cards");
        }
    }
}
