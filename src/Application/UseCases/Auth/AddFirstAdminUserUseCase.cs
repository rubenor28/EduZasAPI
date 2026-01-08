using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.Services;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Auth;

/// <summary>
/// Caso de uso para crear el primer usuario administrador (Setup inicial).
/// </summary>
/// <remarks>
/// Solo se ejecuta si no existen usuarios en el sistema.
/// </remarks>

public sealed class AddFirstAdminUserUseCase(
    IHashService hasher,
    ICreatorAsync<UserDomain, NewUserDTO> creator,
    IQuerierAsync<UserDomain, UserCriteriaDTO> querier,
    IBusinessValidationService<NewUserDTO> validator
) : IGuestUseCaseAsync<NewUserDTO, UserDomain>
{
    private readonly IHashService _hasher = hasher;
    private readonly ICreatorAsync<UserDomain, NewUserDTO> _creator = creator;
    private readonly IQuerierAsync<UserDomain, UserCriteriaDTO> _querier = querier;
    private readonly IBusinessValidationService<NewUserDTO> _validator = validator;

    public async Task<Result<UserDomain, UseCaseError>> ExecuteAsync(NewUserDTO request)
    {
        var haveUsers = await _querier.AnyAsync(new() { });
        if (haveUsers)
            return UseCaseErrors.Unauthorized();

        var user = request with
        {
            FirstName = request.FirstName.ToUpperInvariant(),
            MidName = request.MidName?.ToUpperInvariant(),
            FatherLastname = request.FatherLastname.ToUpperInvariant(),
            MotherLastname = request.MotherLastname?.ToUpperInvariant(),
        };

        var validation = _validator.IsValid(user);
        if (validation.IsErr)
            return UseCaseErrors.Input(validation.UnwrapErr());

        user = user with { Password = _hasher.Hash(request.Password), Role = UserType.ADMIN };

        return await _creator.AddAsync(user);
    }
}
