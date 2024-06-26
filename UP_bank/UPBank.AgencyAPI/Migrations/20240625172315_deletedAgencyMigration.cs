using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UPBank.AgencyAPI.Migrations
{
    public partial class deletedAgencyMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeletedAgency",
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
                    table.PrimaryKey("PK_DeletedAgency", x => x.Number);
                });

            migrationBuilder.CreateTable(
                name: "DeletedAgencyEmployee",
                columns: table => new
                {
                    Cpf = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    DeletedAgencyNumber = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedAgencyEmployee", x => x.Cpf);
                    table.ForeignKey(
                        name: "FK_DeletedAgencyEmployee_DeletedAgency_DeletedAgencyNumber",
                        column: x => x.DeletedAgencyNumber,
                        principalTable: "DeletedAgency",
                        principalColumn: "Number");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeletedAgencyEmployee_DeletedAgencyNumber",
                table: "DeletedAgencyEmployee",
                column: "DeletedAgencyNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeletedAgencyEmployee");

            migrationBuilder.DropTable(
                name: "DeletedAgency");
        }
    }
}
