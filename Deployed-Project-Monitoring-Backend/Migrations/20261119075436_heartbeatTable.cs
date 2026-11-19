using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Deployed_Project_Monitoring_Backend.Migrations
{
    /// <inheritdoc />
    public partial class heartbeatTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Heartbeats",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<int>(type: "integer", nullable: false),
                    instance_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    version = table.Column<string>(type: "text", nullable: true),
                    environment = table.Column<string>(type: "text", nullable: true),
                    node_version = table.Column<string>(type: "text", nullable: true),
                    uptime_seconds = table.Column<long>(type: "bigint", nullable: false),
                    memory_mb = table.Column<double>(type: "double precision", nullable: true),
                    cpu_percent = table.Column<double>(type: "double precision", nullable: true),
                    interval_seconds = table.Column<int>(type: "integer", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    first_seen_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_heartbeat_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Heartbeats", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Heartbeats_project_id_instance_id",
                table: "Heartbeats",
                columns: new[] { "project_id", "instance_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Heartbeats");
        }
    }
}
