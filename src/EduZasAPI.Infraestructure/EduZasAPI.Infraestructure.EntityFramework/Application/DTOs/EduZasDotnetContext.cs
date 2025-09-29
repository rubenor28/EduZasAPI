using Microsoft.EntityFrameworkCore;

using EduZasAPI.Infraestructure.EntityFramework.Application.Classes;
using EduZasAPI.Infraestructure.EntityFramework.Application.ClassProfessors;
using EduZasAPI.Infraestructure.EntityFramework.Application.ClassStudents;
using EduZasAPI.Infraestructure.EntityFramework.Application.Notifications;
using EduZasAPI.Infraestructure.EntityFramework.Application.NotificationsPerUser;
using EduZasAPI.Infraestructure.EntityFramework.Application.Resources;
using EduZasAPI.Infraestructure.EntityFramework.Application.Tests;
using EduZasAPI.Infraestructure.EntityFramework.Application.TestsPerClass;
using EduZasAPI.Infraestructure.EntityFramework.Application.Users;

using Microsoft.Extensions.Configuration;
using DotNetEnv;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.Common;

public partial class EduZasDotnetContext : DbContext
{
    private readonly IConfiguration _cfg;

    public EduZasDotnetContext()
    {
        var environment = Environment.GetEnvironmentVariable("ServerOptions__Environment");
        if (environment != "Production")
        {
            var solutionRoot = Directory.GetCurrentDirectory();
            var envPath = Path.Combine(solutionRoot, "..", "..", "..", ".env");
            Env.Load(envPath);
        }

        _cfg = new ConfigurationBuilder()
          .AddEnvironmentVariables()
          .Build();
    }

    public EduZasDotnetContext(DbContextOptions<EduZasDotnetContext> options)
        : base(options)
    {
        var environment = Environment.GetEnvironmentVariable("ServerOptions__Environment");
        if (environment != "Production")
        {
            var solutionRoot = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..");
            Env.Load(solutionRoot);
        }

        _cfg = new ConfigurationBuilder()
          .AddEnvironmentVariables()
          .Build();
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassProfessor> ClassProfessors { get; set; }

    public virtual DbSet<ClassStudent> ClassStudents { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationPerUser> NotificationPerUsers { get; set; }

    public virtual DbSet<Resource> Resources { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<TestPerClass> TestsPerClasses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql(
            _cfg.GetConnectionString("DefaultConnection"),
            Microsoft.EntityFrameworkCore.ServerVersion.Parse("12.0.2-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_uca1400_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PRIMARY");

            entity
                .ToTable("classes")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.OwnerId, "owner_id");

            entity.Property(e => e.ClassId)
                .HasMaxLength(20)
                .HasColumnName("class_id");
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.ClassName)
                .HasMaxLength(100)
                .HasColumnName("class_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ModifiedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("modified_at");
            entity.Property(e => e.OwnerId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("owner_id");
            entity.Property(e => e.Section)
                .HasMaxLength(100)
                .HasColumnName("section");
            entity.Property(e => e.Subject)
                .HasMaxLength(100)
                .HasColumnName("subject");

            entity.HasOne(d => d.Owner).WithMany(p => p.Classes)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("classes_ibfk_1");

            entity.HasMany(d => d.Professors).WithMany(p => p.ClassesNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "ResourcesPerClass",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("ProfessorId")
                        .HasConstraintName("resources_per_class_ibfk_2"),
                    l => l.HasOne<Class>().WithMany()
                        .HasForeignKey("ClassId")
                        .HasConstraintName("resources_per_class_ibfk_1"),
                    j =>
                    {
                        j.HasKey("ClassId", "ProfessorId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j
                            .ToTable("resources_per_class")
                            .UseCollation("utf8mb4_unicode_ci");
                        j.HasIndex(new[] { "ProfessorId" }, "professor_id3");
                        j.IndexerProperty<string>("ClassId")
                            .HasMaxLength(20)
                            .HasColumnName("class_id");
                        j.IndexerProperty<ulong>("ProfessorId")
                            .HasColumnType("bigint(20) unsigned")
                            .HasColumnName("professor_id");
                    });
        });

        modelBuilder.Entity<ClassProfessor>(entity =>
        {
            entity.HasKey(e => new { e.ClassId, e.ProfessorId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("class_professors")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.ProfessorId, "professor_id");

            entity.Property(e => e.ClassId)
                .HasMaxLength(20)
                .HasColumnName("class_id");
            entity.Property(e => e.ProfessorId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("professor_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Class).WithMany(p => p.ClassProfessors)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("class_professors_ibfk_1");

            entity.HasOne(d => d.Professor).WithMany(p => p.ClassProfessors)
                .HasForeignKey(d => d.ProfessorId)
                .HasConstraintName("class_professors_ibfk_2");
        });

        modelBuilder.Entity<ClassStudent>(entity =>
        {
            entity.HasKey(e => new { e.ClassId, e.StudentId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("class_students")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.StudentId, "student_id");

            entity.Property(e => e.ClassId)
                .HasMaxLength(20)
                .HasColumnName("class_id");
            entity.Property(e => e.StudentId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("student_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("created_at");

            entity.HasOne(d => d.Class).WithMany(p => p.ClassStudents)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("class_students_ibfk_1");

            entity.HasOne(d => d.Student).WithMany(p => p.ClassStudents)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("class_students_ibfk_2");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PRIMARY");

            entity
                .ToTable("notifications")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.ClassId, "class_id");

            entity.Property(e => e.NotificationId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("notification_id");
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.ClassId)
                .HasMaxLength(20)
                .HasColumnName("class_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Title)
                .HasMaxLength(20)
                .HasColumnName("title");

            entity.HasOne(d => d.Class).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("notifications_ibfk_1");
        });

        modelBuilder.Entity<NotificationPerUser>(entity =>
        {
            entity.HasKey(e => new { e.NotificationId, e.UserId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("notification_per_user")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.NotificationId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("notification_id");
            entity.Property(e => e.UserId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("user_id");
            entity.Property(e => e.ModifiedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("modified_at");
            entity.Property(e => e.Readed).HasColumnName("readed");

            entity.HasOne(d => d.Notification).WithMany(p => p.NotificationPerUsers)
                .HasForeignKey(d => d.NotificationId)
                .HasConstraintName("notification_per_user_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationPerUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("notification_per_user_ibfk_2");
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.ResourceId).HasName("PRIMARY");

            entity
                .ToTable("resources")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.ProfessorId, "professor_id1");

            entity.Property(e => e.ResourceId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("resource_id");
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.Content)
                .HasColumnType("json")
                .HasColumnName("content");
            entity.Property(e => e.ProfessorId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("professor_id");
            entity.Property(e => e.Title)
                .HasMaxLength(35)
                .HasColumnName("title");

            entity.HasOne(d => d.Professor).WithMany(p => p.Resources)
                .HasForeignKey(d => d.ProfessorId)
                .HasConstraintName("resources_ibfk_1");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.TestId).HasName("PRIMARY");

            entity
                .ToTable("tests")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.ProfessorId, "professor_id2");

            entity.Property(e => e.TestId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("test_id");
            entity.Property(e => e.Content)
                .HasColumnType("json")
                .HasColumnName("content");
            entity.Property(e => e.ProfessorId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("professor_id");
            entity.Property(e => e.TimeLimitMinutes)
                .HasColumnType("int(10) unsigned")
                .HasColumnName("time_limit_minutes");
            entity.Property(e => e.Title)
                .HasMaxLength(35)
                .HasColumnName("title");

            entity.HasOne(d => d.Professor).WithMany(p => p.Tests)
                .HasForeignKey(d => d.ProfessorId)
                .HasConstraintName("tests_ibfk_1");
        });

        modelBuilder.Entity<TestPerClass>(entity =>
        {
            entity.HasKey(e => new { e.TestId, e.ClassId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("tests_per_class")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.ClassId, "class_id1");

            entity.Property(e => e.TestId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("test_id");
            entity.Property(e => e.ClassId)
                .HasMaxLength(20)
                .HasColumnName("class_id");
            entity.Property(e => e.Visible).HasColumnName("visible");

            entity.HasOne(d => d.Class).WithMany(p => p.TestsPerClasses)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("tests_per_class_ibfk_2");

            entity.HasOne(d => d.Test).WithMany(p => p.TestsPerClasses)
                .HasForeignKey(d => d.TestId)
                .HasConstraintName("tests_per_class_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity
                .ToTable("users")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.Property(e => e.UserId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("user_id");
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .HasColumnName("email");
            entity.Property(e => e.FatherLastname)
                .HasMaxLength(20)
                .HasColumnName("father_lastname");
            entity.Property(e => e.FirstName)
                .HasMaxLength(20)
                .HasColumnName("first_name");
            entity.Property(e => e.MidName)
                .HasMaxLength(20)
                .HasColumnName("mid_name");
            entity.Property(e => e.ModifiedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("modified_at");
            entity.Property(e => e.MotherLastname)
                .HasMaxLength(20)
                .HasColumnName("mother_lastname");
            entity.Property(e => e.Password)
                .HasMaxLength(60)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(10) unsigned")
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
