using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Thoughtwave.Migrations
{
    public partial class AddForeignKeyProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "SignUpDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(2017, 3, 12, 12, 22, 16, 903, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 3, 3, 18, 16, 2, 153, DateTimeKind.Local));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Thoughts",
                nullable: false,
                defaultValue: new DateTime(2017, 3, 12, 12, 22, 16, 913, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 3, 3, 18, 16, 2, 163, DateTimeKind.Local));

            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "Thoughts",
                nullable: false,
                defaultValue: 6,
                oldClrType: typeof(int),
                oldDefaultValue: 6)
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "ThoughtId",
                table: "Comments",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Comments",
                nullable: false,
                defaultValue: new DateTime(2017, 3, 12, 12, 22, 16, 916, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 3, 3, 18, 16, 2, 165, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "SignUpDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(2017, 3, 3, 18, 16, 2, 153, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 3, 12, 12, 22, 16, 903, DateTimeKind.Local));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Thoughts",
                nullable: false,
                defaultValue: new DateTime(2017, 3, 3, 18, 16, 2, 163, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 3, 12, 12, 22, 16, 913, DateTimeKind.Local));

            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "Thoughts",
                nullable: false,
                defaultValue: 6,
                oldClrType: typeof(int),
                oldDefaultValue: 6)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "ThoughtId",
                table: "Comments",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Comments",
                nullable: false,
                defaultValue: new DateTime(2017, 3, 3, 18, 16, 2, 165, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2017, 3, 12, 12, 22, 16, 916, DateTimeKind.Local));
        }
    }
}
