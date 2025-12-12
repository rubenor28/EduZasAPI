using DotNetEnv;
using EntityFramework.InterfaceAdapters.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EntityFramework.Application.DTOs;

/// <summary>
/// Contexto de base de datos principal de la aplicación.
/// </summary>
public partial class EduZasDotnetContext : DbContext
{
    private readonly IConfiguration _cfg;

    /// <summary>
    /// Inicializa el contexto cargando la configuración.
    /// </summary>
    public EduZasDotnetContext()
    {
        var environment = Environment.GetEnvironmentVariable("ServerOptions__Environment");
        if (environment != "Production")
        {
            var solutionRoot = Directory.GetCurrentDirectory();
            var envPath = Path.Combine(solutionRoot, "..", "..", "..", ".env");
            Env.Load(envPath);
        }

        _cfg = new ConfigurationBuilder().AddEnvironmentVariables().Build();
    }

    /// <summary>
    /// Inicializa el contexto con opciones y configuración.
    /// </summary>
    /// <param name="options">Opciones del contexto.</param>
    public EduZasDotnetContext(DbContextOptions<EduZasDotnetContext> options)
        : base(options)
    {
        var environment = Environment.GetEnvironmentVariable("ServerOptions__Environment");
        if (environment != "Production")
        {
            var solutionRoot = Directory.GetCurrentDirectory();
            var envPath = Path.Combine(solutionRoot, "..", "..", "..", ".env");
            Env.Load(envPath);
        }

        _cfg = new ConfigurationBuilder().AddEnvironmentVariables().Build();
    }

    /// <summary>Contactos de agenda.</summary>
    public virtual DbSet<AgendaContact> AgendaContacts { get; set; }

    /// <summary>Respuestas de exámenes.</summary>
    public virtual DbSet<Answer> Answers { get; set; }

    /// <summary>Clases.</summary>
    public virtual DbSet<Class> Classes { get; set; }

    /// <summary>Profesores por clase.</summary>
    public virtual DbSet<ClassProfessor> ClassProfessors { get; set; }

    /// <summary>Estudiantes por clase.</summary>
    public virtual DbSet<ClassStudent> ClassStudents { get; set; }

    /// <summary>Notificaciones.</summary>
    public virtual DbSet<Notification> Notifications { get; set; }

    /// <summary>Notificaciones por usuario.</summary>
    public virtual DbSet<NotificationPerUser> NotificationPerUsers { get; set; }

    /// <summary>Recursos.</summary>
    public virtual DbSet<Resource> Resources { get; set; }

    /// <summary>Recursos por clase.</summary>
    public virtual DbSet<ClassResource> ClassResources { get; set; }

    /// <summary>Etiquetas.</summary>
    public virtual DbSet<Tag> Tags { get; set; }

    /// <summary>Etiquetas de contactos.</summary>
    public virtual DbSet<ContactTag> ContactTags { get; set; }

    /// <summary>Exámenes.</summary>
    public virtual DbSet<Test> Tests { get; set; }

    /// <summary>Exámenes por clase.</summary>
    public virtual DbSet<TestPerClass> TestsPerClasses { get; set; }

    /// <summary>Usuarios.</summary>
    public virtual DbSet<User> Users { get; set; }

    /// <summary>Sesiones de vista de recursos (metadata de uso y tiempo de visualización).</summary>
    public virtual DbSet<ResourceViewSession> ResourceViewSessions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(
                _cfg.GetConnectionString("DefaultConnection"),
                ServerVersion.Parse("12.0.2-mariadb")
            );
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
        {
            modelBuilder.UseCollation("utf8mb4_uca1400_ai_ci").HasCharSet("utf8mb4");
        }

        modelBuilder.Entity<AgendaContact>(agendaContactBuilder =>
        {
            agendaContactBuilder.HasKey(e => new { e.AgendaOwnerId, e.UserId }).HasName("PRIMARY");
            agendaContactBuilder.ToTable("agenda_contacts");

            agendaContactBuilder.Property(e => e.Alias).HasMaxLength(40).HasColumnName("alias");
            agendaContactBuilder
                .Property(e => e.Notes)
                .HasColumnType("TEXT")
                .HasColumnName("notes");
            agendaContactBuilder.Property(e => e.AgendaOwnerId).HasColumnName("agenda_owner_id");
            agendaContactBuilder.Property(e => e.UserId).HasColumnName("contact_id");

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                agendaContactBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                agendaContactBuilder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
            }
            else
            {
                agendaContactBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                agendaContactBuilder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("modified_at");
            }

            agendaContactBuilder
                .HasOne(e => e.AgendaOwner)
                .WithMany(e => e.AgendaContactAgendaOwners)
                .HasForeignKey(e => e.AgendaOwnerId)
                .OnDelete(DeleteBehavior.Cascade);
            agendaContactBuilder
                .HasOne(e => e.Contact)
                .WithMany(e => e.AgendaContactContacts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Answer>(answerBuilder =>
        {
            answerBuilder
                .HasKey(e => new
                {
                    e.TestId,
                    e.ClassId,
                    e.UserId,
                })
                .HasName("PRIMARY");
            answerBuilder.ToTable("answer");

            answerBuilder.Property(e => e.Content).HasColumnType("json").HasColumnName("content");
            answerBuilder.Property(e => e.UserId).HasColumnName("user_id");
            answerBuilder.Property(e => e.TestId).HasColumnName("test_id");
            answerBuilder.Property(e => e.ClassId).HasColumnName("class_id");

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                answerBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                answerBuilder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
            }
            else
            {
                answerBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                answerBuilder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("modified_at");
            }

            answerBuilder.HasOne(e => e.User).WithMany(e => e.Answers).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            answerBuilder
                .HasOne(e => e.TestPerClass)
                .WithMany(e => e.Answers)
                .HasForeignKey(e => new { e.TestId, e.ClassId })
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Class>(classBuilder =>
        {
            classBuilder.HasKey(e => e.ClassId).HasName("PRIMARY");
            classBuilder.ToTable("classes");

            classBuilder.Property(e => e.ClassId).HasMaxLength(15).HasColumnName("class_id");
            classBuilder
                .Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            classBuilder.Property(e => e.ClassName).HasMaxLength(100).HasColumnName("class_name");
            classBuilder.Property(e => e.Section).HasMaxLength(100).HasColumnName("section");
            classBuilder.Property(e => e.Subject).HasMaxLength(100).HasColumnName("subject");
            classBuilder
                .Property(e => e.Color)
                .HasMaxLength(7)
                .HasColumnName("color")
                .HasDefaultValueSql("'#007bff'");

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                classBuilder.UseCollation("utf8mb4_unicode_ci");
                classBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                classBuilder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
            }
            else
            {
                classBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                classBuilder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("modified_at");
            }
        });

        modelBuilder.Entity<ClassProfessor>(classProfessorBuilder =>
        {
            classProfessorBuilder.HasKey(e => new { e.ClassId, e.ProfessorId }).HasName("PRIMARY");
            classProfessorBuilder.ToTable("class_professors");
            classProfessorBuilder.HasIndex(e => e.ProfessorId, "idx_class_professors_professor_id");
            classProfessorBuilder
                .Property(e => e.ClassId)
                .HasMaxLength(20)
                .HasColumnName("class_id");
            classProfessorBuilder
                .Property(e => e.IsOwner)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("is_owner");

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                classProfessorBuilder
                    .Property(e => e.ProfessorId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasColumnName("professor_id");
                classProfessorBuilder.HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                classProfessorBuilder.UseCollation("utf8mb4_unicode_ci");
                classProfessorBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
            }
            else
            {
                classProfessorBuilder.Property(e => e.ProfessorId).HasColumnName("professor_id");
                classProfessorBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
            }

            classProfessorBuilder
                .HasOne(d => d.Class)
                .WithMany(p => p.ClassProfessors)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("class_professors_ibfk_1");
            classProfessorBuilder
                .HasOne(d => d.Professor)
                .WithMany(p => p.ClassProfessors)
                .HasForeignKey(d => d.ProfessorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("class_professors_ibfk_2");
        });

        modelBuilder.Entity<ClassStudent>(classStudentBuilder =>
        {
            classStudentBuilder.HasKey(e => new { e.ClassId, e.StudentId }).HasName("PRIMARY");
            classStudentBuilder.ToTable("class_students");
            classStudentBuilder.HasIndex(e => e.StudentId, "idx_class_students_student_id");
            classStudentBuilder.Property(e => e.ClassId).HasMaxLength(20).HasColumnName("class_id");
            classStudentBuilder
                .Property(e => e.Hidden)
                .IsRequired()
                .HasColumnName("hidden")
                .HasDefaultValue(false);

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                classStudentBuilder
                    .Property(e => e.StudentId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasColumnName("student_id");
                classStudentBuilder.HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                classStudentBuilder.UseCollation("utf8mb4_unicode_ci");
                classStudentBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
            }
            else
            {
                classStudentBuilder.Property(e => e.StudentId).HasColumnName("student_id");
                classStudentBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
            }

            classStudentBuilder
                .HasOne(d => d.Class)
                .WithMany(p => p.ClassStudents)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("class_students_ibfk_1");
            classStudentBuilder
                .HasOne(d => d.Student)
                .WithMany(p => p.ClassStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("class_students_ibfk_2");
        });

        modelBuilder.Entity<Notification>(notificationBuilder =>
        {
            notificationBuilder.HasKey(e => e.NotificationId).HasName("PRIMARY");
            notificationBuilder.ToTable("notifications");
            notificationBuilder.HasIndex(e => e.ClassId, "idx_notifications_class_id");
            notificationBuilder
                .Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            notificationBuilder.Property(e => e.ClassId).HasMaxLength(20).HasColumnName("class_id");
            notificationBuilder.Property(e => e.Title).HasMaxLength(60).HasColumnName("title");

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                notificationBuilder.UseCollation("utf8mb4_unicode_ci");
                notificationBuilder
                    .Property(e => e.NotificationId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasColumnName("notification_id");
                notificationBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
            }
            else
            {
                notificationBuilder
                    .Property(e => e.NotificationId)
                    .HasColumnName("notification_id");
                notificationBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
            }

            notificationBuilder
                .HasOne(d => d.Class)
                .WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("notifications_ibfk_1");
        });

        modelBuilder.Entity<NotificationPerUser>(notificationPerUserBuilder =>
        {
            notificationPerUserBuilder
                .HasKey(e => new { e.NotificationId, e.UserId })
                .HasName("PRIMARY");
            notificationPerUserBuilder.ToTable("notification_per_user");
            notificationPerUserBuilder.HasIndex(e => e.UserId, "idx_notification_per_user_user_id");
            notificationPerUserBuilder.Property(e => e.Readed).HasColumnName("readed");

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                notificationPerUserBuilder.UseCollation("utf8mb4_unicode_ci");
                notificationPerUserBuilder.HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                notificationPerUserBuilder
                    .Property(e => e.NotificationId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasColumnName("notification_id");
                notificationPerUserBuilder
                    .Property(e => e.UserId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasColumnName("user_id");
                notificationPerUserBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                notificationPerUserBuilder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
            }
            else
            {
                notificationPerUserBuilder
                    .Property(e => e.NotificationId)
                    .HasColumnName("notification_id");
                notificationPerUserBuilder.Property(e => e.UserId).HasColumnName("user_id");
                notificationPerUserBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                notificationPerUserBuilder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("modified_at");
            }

            notificationPerUserBuilder
                .HasOne(d => d.Notification)
                .WithMany(p => p.NotificationPerUsers)
                .HasForeignKey(d => d.NotificationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("notification_per_user_ibfk_1");
            notificationPerUserBuilder
                .HasOne(d => d.User)
                .WithMany(p => p.NotificationPerUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("notification_per_user_ibfk_2");
        });

        modelBuilder.Entity<Resource>(resourceBuilder =>
        {
            resourceBuilder.HasKey(e => e.ResourceId).HasName("PRIMARY");
            resourceBuilder.ToTable("resources");
            resourceBuilder.HasIndex(e => e.ProfessorId, "idx_resources_professor_id");
            resourceBuilder
                .Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            var contentProperty = resourceBuilder
                .Property(e => e.Content)
                .HasConversion(new JsonNodeToStringConverter())
                .HasColumnName("content");
            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                contentProperty.HasColumnType("json");
            }
            resourceBuilder.Property(e => e.Title).HasMaxLength(60).HasColumnName("title");
            resourceBuilder
                .Property(e => e.Color)
                .HasMaxLength(7)
                .HasColumnName("color")
                .HasDefaultValueSql("'#1976d2'");

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                resourceBuilder.UseCollation("utf8mb4_unicode_ci");
                var resourceIdProperty = resourceBuilder
                    .Property(e => e.ResourceId)
                    .HasColumnType("char(36)")
                    .HasColumnName("resource_id");
                resourceBuilder
                    .Property(e => e.ProfessorId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasColumnName("professor_id");
                resourceBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                resourceBuilder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
            }
            else
            {
                resourceBuilder.Property(e => e.ResourceId).HasColumnName("resource_id");
                resourceBuilder.Property(e => e.ProfessorId).HasColumnName("professor_id");
                resourceBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                resourceBuilder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("modified_at");
            }

            resourceBuilder
                .HasOne(d => d.Professor)
                .WithMany(p => p.Resources)
                .HasForeignKey(d => d.ProfessorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("resources_ibfk_1");
        });

        modelBuilder.Entity<ClassResource>(classResourceBuilder =>
        {
            classResourceBuilder.HasKey(e => new { e.ClassId, e.ResourceId }).HasName("PRIMARY");
            classResourceBuilder.ToTable("class_resources");
            classResourceBuilder.HasIndex(e => e.ResourceId, "idx_class_resources_resource_id");

            classResourceBuilder
                .Property(e => e.ClassId)
                .HasMaxLength(20)
                .HasColumnName("class_id");

            classResourceBuilder
                .Property(e => e.Hidden)
                .IsRequired()
                .HasColumnName("hidden")
                .HasDefaultValue(false);

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                classResourceBuilder.UseCollation("utf8mb4_unicode_ci");
                classResourceBuilder.HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                classResourceBuilder
                    .Property(e => e.ResourceId)
                    .HasColumnType("char(36)")
                    .HasColumnName("resource_id");

                classResourceBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
            }
            else
            {
                classResourceBuilder.Property(e => e.ResourceId).HasColumnName("resource_id");
                classResourceBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
            }

            classResourceBuilder
                .HasOne(d => d.Class)
                .WithMany(p => p.ClassResources)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("class_resources_ibfk_1");

            classResourceBuilder
                .HasOne(d => d.Resource)
                .WithMany(p => p.ClassResources)
                .HasForeignKey(d => d.ResourceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("class_resources_ibfk_2");
        });

        modelBuilder.Entity<Tag>(tagBuilder =>
        {
            tagBuilder.HasKey(e => e.TagId).HasName("PRIMARY");
            tagBuilder.ToTable("tags");

            tagBuilder.HasIndex(e => e.Text, "idx_tags_text").IsUnique();

            tagBuilder.Property(e => e.Text).HasMaxLength(30).HasColumnName("text");

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                tagBuilder
                    .Property(e => e.TagId)
                    .HasColumnType("bigint(20) unsigned")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("tag_id");
                tagBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
            }
            else
            {
                tagBuilder.Property(e => e.TagId).ValueGeneratedOnAdd().HasColumnName("tag_id");
                tagBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
            }
        });

        modelBuilder.Entity<ContactTag>(contactTagBuilder =>
        {
            contactTagBuilder
                .HasKey(e => new
                {
                    e.TagId,
                    e.AgendaOwnerId,
                    e.UserId,
                })
                .HasName("PRIMARY");
            contactTagBuilder.ToTable("contact_tags");

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                contactTagBuilder
                    .Property(e => e.TagId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasColumnName("tag_id");
                contactTagBuilder
                    .Property(e => e.AgendaOwnerId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasColumnName("agenda_owner_id");
                contactTagBuilder
                    .Property(e => e.UserId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasColumnName("contact_id");

                contactTagBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
            }
            else
            {
                contactTagBuilder.Property(e => e.TagId).HasColumnName("tag_id");
                contactTagBuilder.Property(e => e.AgendaOwnerId).HasColumnName("agenda_owner_id");
                contactTagBuilder.Property(e => e.UserId).HasColumnName("user_id");

                contactTagBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
            }

            contactTagBuilder
                .HasOne(e => e.Tag)
                .WithMany(e => e.ContactTags)
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Cascade);
            contactTagBuilder
                .HasOne(e => e.AgendaContact)
                .WithMany(e => e.ContactTags)
                .HasForeignKey(e => new { e.AgendaOwnerId, e.UserId })
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Test>(testBuilder =>
        {
            testBuilder.HasKey(e => e.TestId).HasName("PRIMARY");
            testBuilder.ToTable("tests");
            testBuilder.HasIndex(e => e.ProfessorId, "idx_tests_professor_id");
            testBuilder
                .Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            testBuilder
                .Property(e => e.Color)
                .HasMaxLength(7)
                .HasColumnName("color")
                .HasDefaultValueSql("'#1976d2'");

            var contentProperty = testBuilder
                .Property(e => e.Content)
                .HasConversion(new JsonNodeToStringConverter())
                .HasColumnName("content");

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                contentProperty.HasColumnType("json");
            }

            testBuilder.Property(e => e.Title).HasMaxLength(60).HasColumnName("title");

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                testBuilder.UseCollation("utf8mb4_unicode_ci");
                testBuilder
                    .Property(e => e.TestId)
                    .HasColumnType("char(36)")
                    .HasColumnName("test_id");
                testBuilder
                    .Property(e => e.ProfessorId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasColumnName("professor_id");
                testBuilder
                    .Property(e => e.TimeLimitMinutes)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("time_limit_minutes");
                testBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                testBuilder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
            }
            else
            {
                testBuilder.Property(e => e.TestId).HasColumnName("test_id");
                testBuilder.Property(e => e.ProfessorId).HasColumnName("professor_id");
                testBuilder.Property(e => e.TimeLimitMinutes).HasColumnName("time_limit_minutes");
                testBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                testBuilder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("modified_at");
            }

            testBuilder
                .HasOne(d => d.Professor)
                .WithMany(p => p.Tests)
                .HasForeignKey(d => d.ProfessorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("tests_ibfk_1");
        });

        modelBuilder.Entity<TestPerClass>(testPerClassBuilder =>
        {
            testPerClassBuilder.HasKey(e => new { e.TestId, e.ClassId }).HasName("PRIMARY");
            testPerClassBuilder.ToTable("tests_per_class");
            testPerClassBuilder.HasIndex(e => e.ClassId, "idx_tests_per_class_class_id");
            testPerClassBuilder.Property(e => e.ClassId).HasMaxLength(20).HasColumnName("class_id");
            testPerClassBuilder.Property(e => e.Visible).HasColumnName("visible");

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                testPerClassBuilder.UseCollation("utf8mb4_unicode_ci");
                testPerClassBuilder.HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                testPerClassBuilder
                    .Property(e => e.TestId)
                    .HasColumnType("char(36)")
                    .HasColumnName("test_id");
                testPerClassBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
            }
            else
            {
                testPerClassBuilder.Property(e => e.TestId).HasColumnName("test_id");
                testPerClassBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
            }

            testPerClassBuilder
                .HasOne(d => d.Class)
                .WithMany(p => p.TestsPerClasses)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("tests_per_class_ibfk_2");
            testPerClassBuilder
                .HasOne(d => d.Test)
                .WithMany(p => p.TestsPerClasses)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("tests_per_class_ibfk_1");
        });

        modelBuilder.Entity<User>(userBuilder =>
        {
            userBuilder.HasKey(e => e.UserId).HasName("PRIMARY");
            userBuilder.ToTable("users");
            userBuilder.HasIndex(e => e.Email, "email").IsUnique();
            userBuilder
                .Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            userBuilder.Property(e => e.Email).HasMaxLength(30).HasColumnName("email");
            userBuilder
                .Property(e => e.FatherLastname)
                .HasMaxLength(20)
                .HasColumnName("father_lastname");
            userBuilder.Property(e => e.FirstName).HasMaxLength(20).HasColumnName("first_name");
            userBuilder.Property(e => e.MidName).HasMaxLength(20).HasColumnName("mid_name");
            userBuilder
                .Property(e => e.MotherLastname)
                .HasMaxLength(20)
                .HasColumnName("mother_lastname");
            userBuilder.Property(e => e.Password).HasMaxLength(60).HasColumnName("password");

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                userBuilder.UseCollation("utf8mb4_unicode_ci");
                userBuilder
                    .Property(e => e.UserId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasColumnName("user_id");
                userBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                userBuilder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
                userBuilder
                    .Property(e => e.Role)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("role");
            }
            else
            {
                userBuilder.Property(e => e.UserId).HasColumnName("user_id");
                userBuilder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                userBuilder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("modified_at");
                userBuilder.Property(e => e.Role).HasDefaultValueSql("'0'").HasColumnName("role");
            }
        });

        modelBuilder.Entity<ResourceViewSession>(builder =>
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");
            builder.ToTable("ResourceViewSessions");

            builder.HasIndex(e => e.UserId, "idx_resourceviewsessions_user_id");
            builder.HasIndex(e => e.ResourceId, "idx_resourceviewsessions_resource_id");
            builder.HasIndex(e => e.ClassId, "idx_resourceviewsessions_class_id");

            builder.Property(e => e.Id).HasColumnType("char(36)").HasColumnName("id");
            builder.Property(e => e.StartTimeUtc).HasColumnName("start_time_utc");
            builder.Property(e => e.EndTimeUtc).HasColumnName("end_time_utc");
            builder.Property(e => e.DurationSeconds).HasColumnName("duration_seconds");

            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                builder
                    .Property(e => e.UserId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasColumnName("user_id");
                builder
                    .Property(e => e.ResourceId)
                    .HasColumnType("char(36)")
                    .HasColumnName("resource_id");
                builder.Property(e => e.ClassId).HasMaxLength(15).HasColumnName("class_id");
                builder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");
                builder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("current_timestamp()")
                    .HasColumnType("datetime")
                    .HasColumnName("modified_at");
            }
            else
            {
                builder.Property(e => e.UserId).HasColumnName("user_id");
                builder.Property(e => e.ResourceId).HasColumnName("resource_id");
                builder.Property(e => e.ClassId).HasColumnName("class_id");
                builder
                    .Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");
                builder
                    .Property(e => e.ModifiedAt)
                    .ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("modified_at");
            }

            builder
                .HasOne(d => d.User)
                .WithMany(p => p.ResourceViewSessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasOne(d => d.Resource)
                .WithMany(p => p.ResourceViewSessions)
                .HasForeignKey(d => d.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasOne(d => d.Class)
                .WithMany(p => p.ResourceViewSessions)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
