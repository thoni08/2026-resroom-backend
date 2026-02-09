using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ResRoomApi.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialSeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [Rooms] WHERE [Id] = 1)
                BEGIN
                    INSERT INTO [Rooms] ([Id], [Capacity], [CreatedAt], [DeletedAt], [Description], [Location], [Name], [UpdatedAt])
                    VALUES (1, 30, '2026-02-09T00:00:00.0000000', NULL, 'A spacious conference room equipped with a projector and whiteboard.', 'First Floor - East Building', 'Conference Room A', '2026-02-09T00:00:00.0000000');
                END;

                IF NOT EXISTS (SELECT 1 FROM [Rooms] WHERE [Id] = 2)
                BEGIN
                    INSERT INTO [Rooms] ([Id], [Capacity], [CreatedAt], [DeletedAt], [Description], [Location], [Name], [UpdatedAt])
                    VALUES (2, 10, '2026-02-09T00:00:00.0000000', NULL, 'A cozy meeting room suitable for small team discussions.', 'Second Floor - West Building', 'Meeting Room B', '2026-02-09T00:00:00.0000000');
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
