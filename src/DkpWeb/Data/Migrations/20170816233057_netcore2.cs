using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DkpWeb.Data.Migrations
{
    public partial class netcore2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIdentity_PaymentMethod",
                table: "PaymentIdentity");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIdentity_Person",
                table: "PaymentIdentity");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Person_Creditor",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Person_Debtor",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIdentity_PaymentMethod",
                table: "PaymentIdentity",
                column: "PaymentMethID",
                principalTable: "PaymentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIdentity_Person",
                table: "PaymentIdentity",
                column: "PersonID",
                principalTable: "Person",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Person_Creditor",
                table: "Transaction",
                column: "CreditorID",
                principalTable: "Person",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Person_Debtor",
                table: "Transaction",
                column: "DebtorID",
                principalTable: "Person",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIdentity_PaymentMethod",
                table: "PaymentIdentity");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIdentity_Person",
                table: "PaymentIdentity");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Person_Creditor",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Person_Debtor",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UserId",
                table: "AspNetUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIdentity_PaymentMethod",
                table: "PaymentIdentity",
                column: "PaymentMethID",
                principalTable: "PaymentMethod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIdentity_Person",
                table: "PaymentIdentity",
                column: "PersonID",
                principalTable: "Person",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Person_Creditor",
                table: "Transaction",
                column: "CreditorID",
                principalTable: "Person",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Person_Debtor",
                table: "Transaction",
                column: "DebtorID",
                principalTable: "Person",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
