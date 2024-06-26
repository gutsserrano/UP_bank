using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UPBank.AgencyAPI.Migrations
{
    public partial class v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agency",
                columns: table => new
                {
                    Number = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false),
                    Restriction = table.Column<bool>(type: "bit", nullable: false),
                    AddressZipCode = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    AddressNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agency", x => x.Number);
                });

            migrationBuilder.CreateTable(
                name: "AgencyEmployee",
                columns: table => new
                {
                    Cpf = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    AgencyNumber = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencyEmployee", x => x.Cpf);
                    table.ForeignKey(
                        name: "FK_AgencyEmployee_Agency_AgencyNumber",
                        column: x => x.AgencyNumber,
                        principalTable: "Agency",
                        principalColumn: "Number");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgencyEmployee_AgencyNumber",
                table: "AgencyEmployee",
                column: "AgencyNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgencyEmployee");

            migrationBuilder.DropTable(
                name: "Agency");
        }
    }
}
