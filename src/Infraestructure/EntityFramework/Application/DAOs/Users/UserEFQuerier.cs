using Application.DTOs.Users;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Users;

public class UserEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<User, UserDomain> projector,
    int pageSize
) : EFQuerier<UserDomain, UserCriteriaDTO, User>(ctx, projector, pageSize)
{
    public override IQueryable<User> BuildQuery(UserCriteriaDTO c)
    {
        var query = _dbSet
            .AsNoTracking()
            .AsQueryable()
            .WhereOptional(c.Active, v => u => u.Active == v)
            .WhereOptional(c.Role, r => u => u.Role == (uint)r)
            .WhereOptional(c.CreatedAt, d => u => u.CreatedAt == d)
            .WhereOptional(c.ModifiedAt, d => u => u.ModifiedAt == d)
            .WhereStringQuery(c.FirstName, u => u.FirstName)
            .WhereStringQuery(c.MidName, u => u.MidName)
            .WhereStringQuery(c.FatherLastname, u => u.FatherLastname)
            .WhereStringQuery(c.MotherLastname, u => u.MotherLastname)
            .WhereStringQuery(c.Email, u => u.Email)
            .WhereOptional(
                c.EnrolledInClass,
                cId => u => u.ClassStudents.Any(cs => cs.ClassId == cId)
            )
            .WhereOptional(
                c.TeachingInClass,
                cId => u => u.ClassProfessors.Any(cpf => cpf.ClassId == cId)
            );

        if (_ctx.Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
        {
            query = query.OrderBy(u => u.UserId);
        }

        return query;
    }
}
