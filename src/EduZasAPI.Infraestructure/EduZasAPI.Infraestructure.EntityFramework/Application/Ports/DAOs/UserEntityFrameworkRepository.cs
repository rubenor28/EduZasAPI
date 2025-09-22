namespace EduZasAPI.Infraestructure.Application.Ports.DAOs;

using EduZasAPI.Domain.Entities;
using EduZasAPI.Domain.ValueObjects.Common;

using EduZasAPI.Application.DTOs.Common;
using EduZasAPI.Application.DTOs.Users;
using EduZasAPI.Application.Ports.DAOs;

using EduZasAPI.Infraestructure.Application.DTOs;
using EduZasAPI.Infraestructure.Application.Ports.Mappers;

using Microsoft.EntityFrameworkCore;

public class UserEntityFrameworkRepository :
  EntityFrameworkRepository<ulong, User, NewUserDTO, UserUpdateDTO, UserCriteriaDTO, UserEF>,
  IUserRepositoryAsync
{

    public UserEntityFrameworkRepository(EduZasDotnetContext context, ulong pageSize) : base(context, pageSize) { }

    public override async Task<PaginatedQuery<User, UserCriteriaDTO>> GetByAsync(UserCriteriaDTO criteria)
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

        return await ExecuteQuery(query, criteria);

    }

    public async Task<Optional<User>> FindByEmail(string email)
    {
        var results = await _ctx.Users
          .OrderBy(u => u.UserId)
          .Where(u => u.Email == email)
          .Select(u => u.Into())
          .ToListAsync();

        return results.Count switch
        {
            0 => Optional<User>.None(),
            1 => Optional<User>.Some(results[0]),
            _ => throw new InvalidDataException($"Repeated email: {email}"),
        };
    }
}
