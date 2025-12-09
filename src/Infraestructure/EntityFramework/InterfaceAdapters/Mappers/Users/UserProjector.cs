using System.Linq.Expressions;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Users;

/// <summary>
/// Proyector de consultas para usuarios.
/// </summary>
public class UserProjector : IEFProjector<User, UserDomain, UserCriteriaDTO>
{
    private static UserType MapRole(uint? role) =>
        role switch
        {
            1 => UserType.PROFESSOR,
            2 => UserType.ADMIN,
            _ => UserType.STUDENT,
        };

    /// <inheritdoc/>
    public Expression<Func<User, UserDomain>> GetProjection(UserCriteriaDTO criteria) =>
        source =>
            new()
            {
                Id = source.UserId,
                Active = source.Active ?? false,
                Email = source.Email,
                FatherLastname = source.FatherLastname,
                FirstName = source.FirstName,
                MidName = source.MidName,
                MotherLastname = source.MotherLastname,
                CreatedAt = source.CreatedAt,
                ModifiedAt = source.ModifiedAt,
                Password = source.Password,
                Role = MapRole(source.Role),
            };
}
