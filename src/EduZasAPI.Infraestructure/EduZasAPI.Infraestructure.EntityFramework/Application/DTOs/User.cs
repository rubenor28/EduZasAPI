namespace EduZasAPI.Infraestructure.Application.DTOs;

using EduZasAPI.Domain.Entities;
using EduZasAPI.Domain.ValueObjects.Common;
using EduZasAPI.Domain.Enums.Users;

using EduZasAPI.Application.Ports.Mappers;

public partial class UserEF : IInto<User>
{
    public ulong UserId { get; set; }

    public bool? Active { get; set; }

    public uint? Role { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MidName { get; set; }

    public string FatherLastname { get; set; } = null!;

    public string? MotherLastname { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual ICollection<ClassProfessorEF> ClassProfessors { get; set; } = new List<ClassProfessorEF>();

    public virtual ICollection<ClassStudentEF> ClassStudents { get; set; } = new List<ClassStudentEF>();

    public virtual ICollection<ClassEF> Classes { get; set; } = new List<ClassEF>();

    public virtual ICollection<NotificationPerUserEF> NotificationPerUsers { get; set; } = new List<NotificationPerUserEF>();

    public virtual ICollection<ResourceEF> Resources { get; set; } = new List<ResourceEF>();

    public virtual ICollection<TestEF> Tests { get; set; } = new List<TestEF>();

    public virtual ICollection<ClassEF> ClassesNavigation { get; set; } = new List<ClassEF>();

    public User Into()
    {
        return new User
        {
            Id = UserId,
            Active = Active ?? false,
            Email = Email,
            FirstName = FirstName,
            MidName = MidName is null ?
              Optional<string>.None() :
              Optional<string>.Some(MidName),
            FatherLastName = FatherLastname,
            MotherLastname = MotherLastname is null ?
              Optional<string>.None() :
              Optional<string>.Some(MotherLastname),
            Password = Password,
            CreatedAt = CreatedAt,
            ModifiedAt = ModifiedAt,
            Role = 
              Role.HasValue && Enum.IsDefined(typeof(UserType), (int)Role.Value) ?
              (UserType)Role.Value : UserType.STUDENT,
        };
    }
}
