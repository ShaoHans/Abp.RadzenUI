using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeABP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AbpBackgroundJobs_IsAbandoned_NextTryTime",
                table: "AbpBackgroundJobs");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionTime",
                table: "AbpBackgroundJobs",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpBackgroundJobs_ApplicationName_CompletionTime_IsAbandone~",
                table: "AbpBackgroundJobs",
                columns: new[] { "ApplicationName", "CompletionTime", "IsAbandoned", "NextTryTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AbpBackgroundJobs_ApplicationName_CompletionTime_IsAbandone~",
                table: "AbpBackgroundJobs");

            migrationBuilder.DropColumn(
                name: "CompletionTime",
                table: "AbpBackgroundJobs");

            migrationBuilder.CreateIndex(
                name: "IX_AbpBackgroundJobs_IsAbandoned_NextTryTime",
                table: "AbpBackgroundJobs",
                columns: new[] { "IsAbandoned", "NextTryTime" });
        }
    }
}
