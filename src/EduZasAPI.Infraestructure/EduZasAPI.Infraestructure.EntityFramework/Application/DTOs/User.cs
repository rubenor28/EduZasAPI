namespace EduZasAPI.Infraestructure.Application.DTOs;

using EduZasAPI.Domain.Entities;
using EduZasAPI.Domain.ValueObjects.Common;
using EduZasAPI.Domain.Enums.Users;

using EduZasAPI.Application.DTOs.Users;
using EduZasAPI.Application.Ports.Mappers;

public partial class UserEF :
    IIdentifiable<ulong>, IInto<User>, IFrom<NewUserDTO, UserEF>, IFrom<UserUpdateDTO, UserEF>
{
    public ulong UserId { get; set; }

    public ulong Id => UserId;

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
            MidName = Optional<string>.ToOptional(MidName),
            FatherLastName = FatherLastname,
            MotherLastname = Optional<string>.ToOptional(MotherLastname),
            Password = Password,
            CreatedAt = CreatedAt,
            ModifiedAt = ModifiedAt,
            Role =
              Role.HasValue && Enum.IsDefined(typeof(UserType), (int)Role.Value) ?
              (UserType)Role.Value : UserType.STUDENT,
        };
    }

    public static UserEF From(NewUserDTO dto) => new UserEF
    {
        Email = dto.Email,
        FirstName = dto.FirstName,
        MidName = dto.MidName.ToNullable(),
        FatherLastname = dto.FatherLastName,
        MotherLastname = dto.MotherLastname.ToNullable(),
        Password = dto.Password,
    };

    public static UserEF From(UserUpdateDTO dto) => new UserEF
    {
        UserId = dto.Id,
        FirstName = dto.FirstName,
        FatherLastname = dto.FatherLastName,
        Email = dto.Email,
        Password = dto.Password,
        MidName = dto.MidName.ToNullable(),
        MotherLastname = dto.MotherLastname.ToNullable(),
        Active = dto.Active
    };
}
