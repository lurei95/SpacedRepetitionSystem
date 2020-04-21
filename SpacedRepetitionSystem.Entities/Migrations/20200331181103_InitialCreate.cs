using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SpacedRepetitionSystem.Entities.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "SmartCards");

            migrationBuilder.CreateTable(
                name: "SmartCardDefinition",
                schema: "SmartCards",
                columns: table => new
                {
                    SmartCardDefinitionId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartCardDefinition", x => x.SmartCardDefinitionId);
                });

            migrationBuilder.CreateTable(
                name: "PracticeSets",
                schema: "SmartCards",
                columns: table => new
                {
                    PracticeSetId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    DefaultSmartCardDefinitionId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeSets", x => x.PracticeSetId);
                    table.ForeignKey(
                        name: "FK_PracticeSets_SmartCardDefinition_DefaultSmartCardDefinitionId",
                        column: x => x.DefaultSmartCardDefinitionId,
                        principalSchema: "SmartCards",
                        principalTable: "SmartCardDefinition",
                        principalColumn: "SmartCardDefinitionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SmartCardFieldDefinitions",
                schema: "SmartCards",
                columns: table => new
                {
                    FieldName = table.Column<string>(maxLength: 100, nullable: false),
                    SmartCardDefinitionId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartCardFieldDefinitions", x => new { x.SmartCardDefinitionId, x.FieldName });
                    table.ForeignKey(
                        name: "FK_SmartCardFieldDefinitions_SmartCardDefinition_SmartCardDefinitionId",
                        column: x => x.SmartCardDefinitionId,
                        principalSchema: "SmartCards",
                        principalTable: "SmartCardDefinition",
                        principalColumn: "SmartCardDefinitionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmartCards",
                schema: "SmartCards",
                columns: table => new
                {
                    SmartCardId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tags = table.Column<string>(maxLength: 256, nullable: false),
                    SmartCardDefinitionId = table.Column<long>(nullable: false),
                    PracticeSetId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartCards", x => x.SmartCardId);
                    table.ForeignKey(
                        name: "FK_SmartCards_PracticeSets_PracticeSetId",
                        column: x => x.PracticeSetId,
                        principalSchema: "SmartCards",
                        principalTable: "PracticeSets",
                        principalColumn: "PracticeSetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmartCards_SmartCardDefinition_SmartCardDefinitionId",
                        column: x => x.SmartCardDefinitionId,
                        principalSchema: "SmartCards",
                        principalTable: "SmartCardDefinition",
                        principalColumn: "SmartCardDefinitionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PracticeHistoryEntries",
                schema: "SmartCards",
                columns: table => new
                {
                    SmartCardId = table.Column<long>(nullable: false),
                    SmartCardFieldDefinitionId = table.Column<long>(nullable: false),
                    PracticeDate = table.Column<DateTime>(nullable: false),
                    PracticeResult = table.Column<int>(nullable: false),
                    SmartCardDefinitionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeHistoryEntries", x => new { x.SmartCardId, x.SmartCardFieldDefinitionId });
                    table.ForeignKey(
                        name: "FK_PracticeHistoryEntries_SmartCardDefinition_SmartCardDefinitionId",
                        column: x => x.SmartCardDefinitionId,
                        principalSchema: "SmartCards",
                        principalTable: "SmartCardDefinition",
                        principalColumn: "SmartCardDefinitionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PracticeHistoryEntries_SmartCards_SmartCardId",
                        column: x => x.SmartCardId,
                        principalSchema: "SmartCards",
                        principalTable: "SmartCards",
                        principalColumn: "SmartCardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmartCardFields",
                schema: "SmartCards",
                columns: table => new
                {
                    SmartCardId = table.Column<long>(nullable: false),
                    FieldName = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    SmartCardDefinitionId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartCardFields", x => new { x.SmartCardId, x.FieldName });
                    table.ForeignKey(
                        name: "FK_SmartCardFields_SmartCardDefinition_SmartCardDefinitionId",
                        column: x => x.SmartCardDefinitionId,
                        principalSchema: "SmartCards",
                        principalTable: "SmartCardDefinition",
                        principalColumn: "SmartCardDefinitionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmartCardFields_SmartCards_SmartCardId",
                        column: x => x.SmartCardId,
                        principalSchema: "SmartCards",
                        principalTable: "SmartCards",
                        principalColumn: "SmartCardId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmartCardFields_SmartCardFieldDefinitions_SmartCardDefinitionId_FieldName",
                        columns: x => new { x.SmartCardDefinitionId, x.FieldName },
                        principalSchema: "SmartCards",
                        principalTable: "SmartCardFieldDefinitions",
                        principalColumns: new[] { "SmartCardDefinitionId", "FieldName" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PracticeHistoryEntries_SmartCardDefinitionId",
                schema: "SmartCards",
                table: "PracticeHistoryEntries",
                column: "SmartCardDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeSets_DefaultSmartCardDefinitionId",
                schema: "SmartCards",
                table: "PracticeSets",
                column: "DefaultSmartCardDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_SmartCardFields_SmartCardDefinitionId_FieldName",
                schema: "SmartCards",
                table: "SmartCardFields",
                columns: new[] { "SmartCardDefinitionId", "FieldName" });

            migrationBuilder.CreateIndex(
                name: "IX_SmartCards_PracticeSetId",
                schema: "SmartCards",
                table: "SmartCards",
                column: "PracticeSetId");

            migrationBuilder.CreateIndex(
                name: "IX_SmartCards_SmartCardDefinitionId",
                schema: "SmartCards",
                table: "SmartCards",
                column: "SmartCardDefinitionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PracticeHistoryEntries",
                schema: "SmartCards");

            migrationBuilder.DropTable(
                name: "SmartCardFields",
                schema: "SmartCards");

            migrationBuilder.DropTable(
                name: "SmartCards",
                schema: "SmartCards");

            migrationBuilder.DropTable(
                name: "SmartCardFieldDefinitions",
                schema: "SmartCards");

            migrationBuilder.DropTable(
                name: "PracticeSets",
                schema: "SmartCards");

            migrationBuilder.DropTable(
                name: "SmartCardDefinition",
                schema: "SmartCards");
        }
    }
}
