using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DkpWeb.Data.Migrations
{
    public partial class secquence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "billsplit_id_seq",
                startValue: 1000L);

            migrationBuilder.CreateSequence(
                name: "paymentidentity_id_seq",
                startValue: 100L);

            migrationBuilder.CreateSequence(
                name: "paymentmethod_id_seq",
                startValue: 10L);

            migrationBuilder.CreateSequence(
                name: "person_id_seq",
                startValue: 100L);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Person",
                nullable: false,
                defaultValue: false)
                .Annotation("Npgsql:ValueGeneratedOnAdd", true);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "Person",
                nullable: false,
                defaultValueSql: "nextval('person_id_seq')")
                .Annotation("Npgsql:ValueGeneratedOnAdd", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PaymentMethod",
                nullable: false,
                defaultValueSql: "nextval('paymentmethod_id_seq')")
                .Annotation("Npgsql:ValueGeneratedOnAdd", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PaymentIdentity",
                nullable: false,
                defaultValueSql: "nextval('paymentidentity_id_seq')")
                .Annotation("Npgsql:ValueGeneratedOnAdd", true);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "BillSplit",
                nullable: false,
                defaultValueSql: "nextval('billsplit_id_seq')")
                .Annotation("Npgsql:ValueGeneratedOnAdd", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "billsplit_id_seq");

            migrationBuilder.DropSequence(
                name: "paymentidentity_id_seq");

            migrationBuilder.DropSequence(
                name: "paymentmethod_id_seq");

            migrationBuilder.DropSequence(
                name: "person_id_seq");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Person",
                nullable: false,
                defaultValueSql: "false")
                .Annotation("Npgsql:ValueGeneratedOnAdd", true);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "Person",
                nullable: false)
                .Annotation("Npgsql:ValueGeneratedOnAdd", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PaymentMethod",
                nullable: false)
                .Annotation("Npgsql:ValueGeneratedOnAdd", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PaymentIdentity",
                nullable: false)
                .Annotation("Npgsql:ValueGeneratedOnAdd", true);

            migrationBuilder.AlterColumn<int>(
                name: "ID",
                table: "BillSplit",
                nullable: false)
                .Annotation("Npgsql:ValueGeneratedOnAdd", true);
        }
    }
}
