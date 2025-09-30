using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Users;
using Microsoft.EntityFrameworkCore;
using EduZasAPI.Infraestructure.EntityFramework.Application.Common;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.Users;

public class UserEntityFrameworkRepository :
  SimpleKeyEFRepository<ulong, UserDomain, NewUserDTO, UserUpdateDTO, UserCriteriaDTO, User>
{

    public UserEntityFrameworkRepository(EduZasDotnetContext context, ulong pageSize) : base(context, pageSize) { }

    /// <inheritdoc/>
    protected override ulong GetId(User entity) => entity.UserId;

    /// <inheritdoc/>
    protected override ulong GetId(UserUpdateDTO entity) => entity.Id;

    /// <inheritdoc/>
    protected override IQueryable<User> QueryFromCriteria(UserCriteriaDTO c) =>
        _ctx.Users.AsNoTracking().AsQueryable()
        .WhereOptional(c.Active, v => u => u.Active == v)
        .WhereOptional(c.Role, r => u => u.Role == (uint)r)
        .WhereOptional(c.CreatedAt, d => u => u.CreatedAt == d)
        .WhereOptional(c.ModifiedAt, d => u => u.ModifiedAt == d)
        .WhereStringQuery(c.FirstName, u => u.FirstName)
        .WhereStringQuery(c.MidName, u => u.MidName)
        .WhereStringQuery(c.FatherLastName, u => u.FatherLastname)
        .WhereStringQuery(c.MotherLastname, u => u.MotherLastname)
        .WhereStringQuery(c.Email, u => u.Email)
        .WhereOptional(c.EnrolledInClass, cId => u => u.ClassStudents.Any(cs => cs.ClassId == cId))
        .WhereOptional(c.TeachingInClass, cId => u => u.ClassProfessors.Any(cpf => cpf.ClassId == cId));

    /// <inheritdoc/>
    protected override UserDomain MapToDomain(User efEntity) => new UserDomain
    {
        Id = efEntity.UserId,
        Active = efEntity.Active ?? false,
        Email = efEntity.Email,
        FirstName = efEntity.FirstName,
        MidName = efEntity.MidName.ToOptional(),
        FatherLastName = efEntity.FatherLastname,
        MotherLastname = efEntity.MotherLastname.ToOptional(),
        Password = efEntity.Password,
        CreatedAt = efEntity.CreatedAt,
        ModifiedAt = efEntity.ModifiedAt,
        Role =
              efEntity.Role.HasValue && Enum.IsDefined(typeof(UserType), (int)efEntity.Role.Value) ?
              (UserType)efEntity.Role.Value : UserType.STUDENT,
    };

    /// <inheritdoc/>
    protected override User NewToEF(NewUserDTO newEntity) => new User
    {
        Email = newEntity.Email,
        FirstName = newEntity.FirstName,
        MidName = newEntity.MidName.ToNullable(),
        FatherLastname = newEntity.FatherLastName,
        MotherLastname = newEntity.MotherLastname.ToNullable(),
        Password = newEntity.Password,
    };

    /// <inheritdoc/>
    protected override void UpdateProperties(User entity, UserUpdateDTO updatedProps)
    {
        entity.UserId = updatedProps.Id;
        entity.FirstName = updatedProps.FirstName;
        entity.FatherLastname = updatedProps.FatherLastName;
        entity.Email = updatedProps.Email;
        entity.Password = updatedProps.Password;
        entity.MidName = updatedProps.MidName.ToNullable();
        entity.MotherLastname = updatedProps.MotherLastname.ToNullable();
        entity.Active = updatedProps.Active;
    }
}
