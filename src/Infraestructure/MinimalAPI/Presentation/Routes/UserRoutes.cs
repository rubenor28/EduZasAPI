using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.UseCases.Users;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Users;
using MinimalAPI.Presentation.Mappers;

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
        UserCriteriaMAPI request,
        UserQueryUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils,
        IMapper<UseCaseError, IResult> useCaseErrorMapper,
        IMapper<
            UserCriteriaMAPI,
            Result<UserCriteriaDTO, IEnumerable<FieldErrorDTO>>
        > criteriaMapper
    )
    {
        return utils.HandleResponseAsync(async () =>
        {
            var executor = utils.GetExecutorFromContext(ctx);
            var parse = criteriaMapper.Map(request);

            if (parse.IsErr)
            {
                var useCaseError = UseCaseErrors.Input(parse.UnwrapErr());
                return useCaseErrorMapper.Map(useCaseError);
            }

            var criteria = parse.Unwrap();
            var result = await useCase.ExecuteAsync(parse.Unwrap());

            if (result.IsErr)
                return useCaseErrorMapper.Map(result.UnwrapErr());

            var search = result.Unwrap();

            return Results.Ok(
                new PaginatedQuery<UserDomain, UserCriteriaMAPI>()
                {
                    Page = search.Page,
                    Criteria = request,
                    TotalPages = search.TotalPages,
                    Results = search.Results,
                }
            );
        });
    }

    public static Task<IResult> UpdateUser(
        UserUpdateMAPI request,
        UpdateUserUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils,
        IMapper<UseCaseError, IResult> useCaseErrorMapper,
        IMapper<uint, Result<UserType, Unit>> roleMapper
    )
    {
        return utils.HandleResponseAsync(async () =>
        {
            var executor = utils.GetExecutorFromContext(ctx);
            var role = roleMapper.Map(request.Role);

            if (role.IsErr)
            {
                var error = UseCaseErrors.Input(
                    [new() { Field = "role", Message = "Formato inválido" }]
                );

                return useCaseErrorMapper.Map(error);
            }

            var result = await useCase.ExecuteAsync(
                new UserUpdateDTO
                {
                    Id = request.Id,
                    Active = request.Active,
                    Email = request.Email,
                    FatherLastName = request.FatherLastName,
                    FirstName = request.FirstName,
                    MotherLastname = request.MotherLastname.ToOptional(),
                    MidName = request.MidName.ToOptional(),
                    Password = request.Password,
                    Role = role.Unwrap(),
                    Executor = executor,
                }
            );

            if (result.IsErr)
                return useCaseErrorMapper.Map(result.UnwrapErr());

            return Results.Ok(result.Unwrap());
        });
    }

    public static Task<IResult> DeleteUser(
        ulong userId,
        DeleteUserUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils,
        IMapper<UseCaseError, IResult> useCaseErrorMapper,
        IMapper<UserDomain, PublicUserMAPI> userMapper
    )
    {
        return utils.HandleResponseAsync(async () =>
        {
            var executor = utils.GetExecutorFromContext(ctx);
            var result = await useCase.ExecuteAsync(new() { Id = userId, Executor = executor });

            if (result.IsErr)
                return useCaseErrorMapper.Map(result.UnwrapErr());

            var user = result.Unwrap();
            return Results.Ok(userMapper.Map(user));
        });
    }
}
