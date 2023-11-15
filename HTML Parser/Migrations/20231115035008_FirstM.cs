using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HTML_Parser.Migrations
{
    /// <inheritdoc />
    public partial class FirstM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "State",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "State",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Полный парсинг"});
            migrationBuilder.InsertData(
                table: "State",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Неполный парсинг" });

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "Laptop",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Laptop_StateId",
                table: "Laptop",
                column: "StateId");

            migrationBuilder.Sql("UPDATE Laptop SET StateId = 1 FROM Laptop");

            migrationBuilder.AlterColumn<int>(
                name: "StateId",
                table: "Laptop",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Laptop_State_StateId",
                table: "Laptop",
                column: "StateId",
                principalTable: "State",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Laptop_State_StateId",
                table: "Laptop");

            migrationBuilder.DropTable(
                name: "State");

            migrationBuilder.DropIndex(
                name: "IX_Laptop_StateId",
                table: "Laptop");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "Laptop");
        }
    }
}
