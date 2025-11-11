using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.UseCases.Users;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Users;

namespace MinimalAPI.Presentation.Routes;

public static class UserRoutes
{
    public static RouteGroupBuilder MapUserRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/users").WithTags("Usuarios");

        group.MapPost("/users", SearchUsers).RequireAuthorization("Admin");
        group.MapPut("/users", UpdateUser).RequireAuthorization("Admin");

        return group;
    }

    public static Task<IResult> SearchUsers(
        UserCriteriaMAPI criteria,
        UserQueryUseCase useCase,
        IMapper<UserCriteriaMAPI, Result<UserCriteriaDTO, IEnumerable<FieldErrorDTO>>> reqMapper,
        IMapper<
            PaginatedQuery<UserDomain, UserCriteriaDTO>,
            PaginatedQuery<PublicUserMAPI, UserCriteriaMAPI>
        > resMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(criteria),
            mapResponse: (search) => Results.Ok(resMapper.Map(search))
        );
    }

    public static Task<IResult> UpdateUser(
        UserUpdateMAPI request,
        UpdateUserUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils,
        IMapper<UserUpdateMAPI, Executor, UserUpdateDTO> reqMapper,
        IMapper<UserDomain, PublicUserMAPI> resMapper
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(request, utils.GetExecutorFromContext(ctx)),
            mapResponse: (user) => Results.Ok(resMapper.Map(user))
        );
    }

    public static Task<IResult> DeleteUser(
        ulong userId,
        DeleteUserUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils,
        IMapper<ulong, Executor, DeleteUserDTO> reqMapper,
        IMapper<UserDomain, PublicUserMAPI> resMapper
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(userId, utils.GetExecutorFromContext(ctx)),
            mapResponse: (user) => Results.Ok(resMapper.Map(user))
        );
    }
}
