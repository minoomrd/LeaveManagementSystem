using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserRoleToEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add RoleId column as nullable first
            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "Users",
                type: "uuid",
                nullable: true);

            // Step 2: Create roles if they don't exist and get their IDs
            // Note: These GUIDs should match the ones in SeedData.cs
            // Employee = 1, Admin = 2, SuperAdmin = 3
            
            // We'll use SQL to create roles and migrate data
            // First, ensure roles exist (they should be created by seed data, but we'll handle it here too)
            migrationBuilder.Sql(@"
                -- Create roles if they don't exist
                INSERT INTO ""Roles"" (""Id"", ""Name"", ""Description"", ""IsActive"", ""CreatedAt"", ""UpdatedAt"")
                SELECT 
                    '11111111-1111-1111-1111-111111111111'::uuid,
                    'Employee',
                    'Regular employee who can request leaves',
                    true,
                    NOW(),
                    NOW()
                WHERE NOT EXISTS (SELECT 1 FROM ""Roles"" WHERE ""Name"" = 'Employee');

                INSERT INTO ""Roles"" (""Id"", ""Name"", ""Description"", ""IsActive"", ""CreatedAt"", ""UpdatedAt"")
                SELECT 
                    '22222222-2222-2222-2222-222222222222'::uuid,
                    'Admin',
                    'Company administrator who manages employees and approves leaves',
                    true,
                    NOW(),
                    NOW()
                WHERE NOT EXISTS (SELECT 1 FROM ""Roles"" WHERE ""Name"" = 'Admin');

                INSERT INTO ""Roles"" (""Id"", ""Name"", ""Description"", ""IsActive"", ""CreatedAt"", ""UpdatedAt"")
                SELECT 
                    '33333333-3333-3333-3333-333333333333'::uuid,
                    'SuperAdmin',
                    'Platform super-admin who manages companies and billing',
                    true,
                    NOW(),
                    NOW()
                WHERE NOT EXISTS (SELECT 1 FROM ""Roles"" WHERE ""Name"" = 'SuperAdmin');
            ");

            // Step 3: Migrate existing Role enum values to RoleId
            migrationBuilder.Sql(@"
                UPDATE ""Users""
                SET ""RoleId"" = '11111111-1111-1111-1111-111111111111'::uuid
                WHERE ""Role"" = 1; -- Employee

                UPDATE ""Users""
                SET ""RoleId"" = '22222222-2222-2222-2222-222222222222'::uuid
                WHERE ""Role"" = 2; -- Admin

                UPDATE ""Users""
                SET ""RoleId"" = '33333333-3333-3333-3333-333333333333'::uuid
                WHERE ""Role"" = 3; -- SuperAdmin
            ");

            // Step 4: Make RoleId non-nullable
            migrationBuilder.AlterColumn<Guid>(
                name: "RoleId",
                table: "Users",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            // Step 5: Drop the old Role column
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            // Step 6: Create index and foreign key
            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
