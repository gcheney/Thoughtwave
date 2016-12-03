using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Thoughtwave.Migrations
{
    public partial class UserBios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "This user hasn't filled out their Bio yet.");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SignUpDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(2016, 12, 2, 18, 58, 47, 318, DateTimeKind.Local));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Comments",
                nullable: false,
                defaultValue: new DateTime(2016, 12, 2, 18, 58, 47, 326, DateTimeKind.Local));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Articles",
                nullable: false,
                defaultValue: new DateTime(2016, 12, 2, 18, 58, 47, 327, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SignUpDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(2016, 12, 1, 9, 59, 53, 456, DateTimeKind.Local));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Comments",
                nullable: false,
                defaultValue: new DateTime(2016, 12, 1, 9, 59, 53, 465, DateTimeKind.Local));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Articles",
                nullable: false,
                defaultValue: new DateTime(2016, 12, 1, 9, 59, 53, 465, DateTimeKind.Local));
        }
    }
}
