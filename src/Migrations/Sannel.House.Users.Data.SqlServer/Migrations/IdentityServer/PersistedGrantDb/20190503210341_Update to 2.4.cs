﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Sannel.House.Users.Data.SqlServer.Migrations.IdentityServer.PersistedGrantDb
{
    public partial class Updateto24 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeviceCodes_UserCode",
                table: "DeviceCodes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DeviceCodes_UserCode",
                table: "DeviceCodes",
                column: "UserCode",
                unique: true);
        }
    }
}
