using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoAddressStorage.Migrations
{
    public partial class TranslationResource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TranslationResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResourceKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Text_En = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Text_Fr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationResources", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TranslationResources");
        }
    }
}
