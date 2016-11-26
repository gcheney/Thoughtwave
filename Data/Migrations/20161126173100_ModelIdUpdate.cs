using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sophophile.Data.Migrations
{
    public partial class ModelIdUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Questions",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answers",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AnswerId",
                table: "Answers");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Questions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Answers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Questions",
                table: "Questions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answers",
                table: "Answers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Questions",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Answers",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Answers");

            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "Questions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AnswerId",
                table: "Answers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Questions",
                table: "Questions",
                column: "QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Answers",
                table: "Answers",
                column: "AnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_QuestionId",
                table: "Answers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
