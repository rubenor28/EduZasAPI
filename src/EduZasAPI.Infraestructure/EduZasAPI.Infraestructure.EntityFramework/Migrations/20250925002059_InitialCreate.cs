using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduZasAPI.Infraestructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
                    role = table.Column<uint>(type: "int(10) unsigned", nullable: true, defaultValueSql: "'0'"),
                    first_name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mid_name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    father_lastname = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mother_lastname = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()"),
                    modified_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.user_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "classes",
                columns: table => new
                {
                    class_id = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
                    class_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    subject = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    section = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    owner_id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()"),
                    modified_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.class_id);
                    table.ForeignKey(
                        name: "classes_ibfk_1",
                        column: x => x.owner_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "resources",
                columns: table => new
                {
                    resource_id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
                    title = table.Column<string>(type: "varchar(35)", maxLength: 35, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "json", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    professor_id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.resource_id);
                    table.ForeignKey(
                        name: "resources_ibfk_1",
                        column: x => x.professor_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "tests",
                columns: table => new
                {
                    test_id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(type: "varchar(35)", maxLength: 35, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "json", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    time_limit_minutes = table.Column<uint>(type: "int(10) unsigned", nullable: true),
                    professor_id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.test_id);
                    table.ForeignKey(
                        name: "tests_ibfk_1",
                        column: x => x.professor_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "class_professors",
                columns: table => new
                {
                    class_id = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    professor_id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.class_id, x.professor_id })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "class_professors_ibfk_1",
                        column: x => x.class_id,
                        principalTable: "classes",
                        principalColumn: "class_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "class_professors_ibfk_2",
                        column: x => x.professor_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "class_students",
                columns: table => new
                {
                    class_id = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    student_id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.class_id, x.student_id })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "class_students_ibfk_1",
                        column: x => x.class_id,
                        principalTable: "classes",
                        principalColumn: "class_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "class_students_ibfk_2",
                        column: x => x.student_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notification_id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValueSql: "'1'"),
                    title = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    class_id = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.notification_id);
                    table.ForeignKey(
                        name: "notifications_ibfk_1",
                        column: x => x.class_id,
                        principalTable: "classes",
                        principalColumn: "class_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "resources_per_class",
                columns: table => new
                {
                    class_id = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    professor_id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.class_id, x.professor_id })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "resources_per_class_ibfk_1",
                        column: x => x.class_id,
                        principalTable: "classes",
                        principalColumn: "class_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "resources_per_class_ibfk_2",
                        column: x => x.professor_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "tests_per_class",
                columns: table => new
                {
                    test_id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false),
                    class_id = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    visible = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.test_id, x.class_id })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "tests_per_class_ibfk_1",
                        column: x => x.test_id,
                        principalTable: "tests",
                        principalColumn: "test_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "tests_per_class_ibfk_2",
                        column: x => x.class_id,
                        principalTable: "classes",
                        principalColumn: "class_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateTable(
                name: "notification_per_user",
                columns: table => new
                {
                    notification_id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false),
                    user_id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false),
                    readed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    modified_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.notification_id, x.user_id })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "notification_per_user_ibfk_1",
                        column: x => x.notification_id,
                        principalTable: "notifications",
                        principalColumn: "notification_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "notification_per_user_ibfk_2",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateIndex(
                name: "professor_id",
                table: "class_professors",
                column: "professor_id");

            migrationBuilder.CreateIndex(
                name: "student_id",
                table: "class_students",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "owner_id",
                table: "classes",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "user_id",
                table: "notification_per_user",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "class_id",
                table: "notifications",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "professor_id1",
                table: "resources",
                column: "professor_id");

            migrationBuilder.CreateIndex(
                name: "professor_id3",
                table: "resources_per_class",
                column: "professor_id");

            migrationBuilder.CreateIndex(
                name: "professor_id2",
                table: "tests",
                column: "professor_id");

            migrationBuilder.CreateIndex(
                name: "class_id1",
                table: "tests_per_class",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "class_professors");

            migrationBuilder.DropTable(
                name: "class_students");

            migrationBuilder.DropTable(
                name: "notification_per_user");

            migrationBuilder.DropTable(
                name: "resources");

            migrationBuilder.DropTable(
                name: "resources_per_class");

            migrationBuilder.DropTable(
                name: "tests_per_class");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "tests");

            migrationBuilder.DropTable(
                name: "classes");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
