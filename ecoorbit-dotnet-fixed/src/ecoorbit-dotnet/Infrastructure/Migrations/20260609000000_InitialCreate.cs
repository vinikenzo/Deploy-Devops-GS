using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecoorbit_dotnet.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PASSWORD_HASH = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ROLE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SATELLITE_IMAGES",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IMAGE_URL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    REGION = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LATITUDE = table.Column<double>(type: "float", nullable: false),
                    LONGITUDE = table.Column<double>(type: "float", nullable: false),
                    CAPTURED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SUBMITTED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    USER_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SATELLITE_IMAGES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SATELLITE_IMAGES_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FIRE_DETECTION_RESULTS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FIRE_DETECTED = table.Column<bool>(type: "bit", nullable: false),
                    RISK_LEVEL = table.Column<int>(type: "int", nullable: false),
                    CONFIDENCE_SCORE = table.Column<double>(type: "float", nullable: false),
                    NOTES = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ANALYZED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SATELLITE_IMAGE_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FIRE_DETECTION_RESULTS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FIRE_DETECTION_RESULTS_SATELLITE_IMAGES_SATELLITE_IMAGE_ID",
                        column: x => x.SATELLITE_IMAGE_ID,
                        principalTable: "SATELLITE_IMAGES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FIRE_DETECTION_RESULTS_SATELLITE_IMAGE_ID",
                table: "FIRE_DETECTION_RESULTS",
                column: "SATELLITE_IMAGE_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SATELLITE_IMAGES_USER_ID",
                table: "SATELLITE_IMAGES",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_EMAIL",
                table: "USERS",
                column: "EMAIL",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "FIRE_DETECTION_RESULTS");
            migrationBuilder.DropTable(name: "SATELLITE_IMAGES");
            migrationBuilder.DropTable(name: "USERS");
        }
    }
}
