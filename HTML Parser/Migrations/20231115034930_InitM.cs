using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HTML_Parser.Migrations
{
    /// <inheritdoc />
    public partial class InitM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Laptop",
            //    columns: table => new
            //    {
            //        ID_laptop = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
            //        Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
            //        OS = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
            //        Screen_diagonal = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
            //        Processor_model = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
            //        Video_card_type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
            //        Video_card_model = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Laptop", x => x.ID_laptop);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TV",
            //    columns: table => new
            //    {
            //        ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
            //        Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
            //        Brand = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
            //        Diagonal = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
            //        Weight = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TV", x => x.ID);
            //    });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Laptop");

            migrationBuilder.DropTable(
                name: "TV");
        }
    }
}
