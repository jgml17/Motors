using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiMotorsAdService.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_AnuncioWebmotors",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    marca = table.Column<string>(maxLength: 45, nullable: false),
                    modelo = table.Column<string>(maxLength: 45, nullable: false),
                    versao = table.Column<string>(maxLength: 45, nullable: false),
                    ano = table.Column<int>(nullable: false),
                    quilometragem = table.Column<int>(nullable: false),
                    observacao = table.Column<string>(maxLength: 2147483647, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_AnuncioWebmotors", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_AnuncioWebmotors");
        }
    }
}
