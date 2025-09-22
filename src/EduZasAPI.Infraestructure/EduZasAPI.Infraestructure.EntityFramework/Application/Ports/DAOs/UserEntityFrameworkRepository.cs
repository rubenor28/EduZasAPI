namespace EduZasAPI.Infraestructure.Application.Ports.DAOs;

using EduZasAPI.Domain.Entities;
using EduZasAPI.Domain.Enums.Users;
using EduZasAPI.Domain.ValueObjects.Common;

using EduZasAPI.Application.DTOs.Users;
using EduZasAPI.Application.Ports.DAOs;

using EduZasAPI.Infraestructure.Application.DTOs;
using EduZasAPI.Infraestructure.Application.Ports.Mappers;

using Microsoft.EntityFrameworkCore;

public class UserEntityFrameworkRepository :
  EntityFrameworkRepository<ulong, UserDomain, NewUserDTO, UserUpdateDTO, UserCriteriaDTO, User>,
  IUserRepositoryAsync
{

    public UserEntityFrameworkRepository(EduZasDotnetContext context, ulong pageSize) : base(context, pageSize) { }


    protected override ulong GetId(User entity) => entity.UserId;

    protected override ulong GetId(UserUpdateDTO entity) => entity.Id;

    protected override IQueryable<User> QueryFromCriteria(UserCriteriaDTO criteria)
    {
        var query = _ctx.Users.AsNoTracking().AsQueryable();

        query = query.WhereOptional(criteria.Active, v => u => u.Active == v);
        query = query.WhereOptional(criteria.Role, r => u => u.Role == (uint)r);

        query = query.WhereOptional(criteria.CreatedAt, d => u => u.CreatedAt == d);
        query = query.WhereOptional(criteria.ModifiedAt, d => u => u.ModifiedAt == d);

        query = query.WhereStringQuery(criteria.FirstName, u => u.FirstName);
        query = query.WhereStringQuery(criteria.MidName, u => u.MidName);
        query = query.WhereStringQuery(criteria.FatherLastName, u => u.FatherLastname);
        query = query.WhereStringQuery(criteria.MotherLastname, u => u.MotherLastname);
        query = query.WhereStringQuery(criteria.Email, u => u.Email);

        return query;
    }

    public async Task<Optional<UserDomain>> FindByEmail(string email)
    {
        var results = await _ctx.Users
          .OrderBy(u => u.UserId)
          .Where(u => u.Email == email)
          .Select(u => MapToDomain(u))
          .ToListAsync();

        return results.Count switch
        {
            0 => Optional<UserDomain>.None(),
            1 => Optional<UserDomain>.Some(results[0]),
            _ => throw new InvalidDataException($"Repeated email: {email}"),
        };
    }


    protected override UserDomain MapToDomain(User efEntity) => new UserDomain
    {
        Id = efEntity.UserId,
        Active = efEntity.Active ?? false,
        Email = efEntity.Email,
        FirstName = efEntity.FirstName,
        MidName = Optional<string>.ToOptional(efEntity.MidName),
        FatherLastName = efEntity.FatherLastname,
        MotherLastname = Optional<string>.ToOptional(efEntity.MotherLastname),
        Password = efEntity.Password,
        CreatedAt = efEntity.CreatedAt,
        ModifiedAt = efEntity.ModifiedAt,
        Role =
              efEntity.Role.HasValue && Enum.IsDefined(typeof(UserType), (int)efEntity.Role.Value) ?
              (UserType)efEntity.Role.Value : UserType.STUDENT,
    };

    protected override User NewToEF(NewUserDTO newEntity) => new User
    {
        Email = newEntity.Email,
        FirstName = newEntity.FirstName,
        MidName = newEntity.MidName.ToNullable(),
        FatherLastname = newEntity.FatherLastName,
        MotherLastname = newEntity.MotherLastname.ToNullable(),
        Password = newEntity.Password,
    };

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
