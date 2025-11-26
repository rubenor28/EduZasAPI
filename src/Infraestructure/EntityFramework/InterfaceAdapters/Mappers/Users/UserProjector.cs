using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers.Users;

public class UserProjector : IEFProjector<User, UserDomain>
{
    public Expression<Func<User, UserDomain>> Projection =>
        source =>
            new()
            {
                Id = source.UserId,
                Active = source.Active ?? false,
                Email = source.Email,
                FatherLastname = source.FatherLastname,
                FirstName = source.FirstName,
                MidName = source.MidName.ToOptional(),
                MotherLastname = source.MotherLastname.ToOptional(),
                CreatedAt = source.CreatedAt,
                ModifiedAt = source.ModifiedAt,
                Password = source.Password,
                Role = MapRole(source.Role),
            };

    private static UserType MapRole(uint? role) =>
        role switch
        {
            1 => UserType.PROFESSOR,
            2 => UserType.ADMIN,
            _ => UserType.STUDENT,
        };

    private static readonly Lazy<Func<User, UserDomain>> _mapFunc = new(() =>
        new UserProjector().Projection.Compile()
    );

    public UserDomain Map(User source) => _mapFunc.Value(source);
}
