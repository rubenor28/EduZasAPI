using Application.DAOs;
using Application.DTOs.Resources;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Resources;

/// <summary>
/// Caso de uso para añadir un recurso.
/// </summary>
public sealed class AddResourceUseCase(
    ICreatorAsync<ResourceDomain, NewResourceDTO> creator,
    IReaderAsync<ulong, UserDomain> userReader,
    IBusinessValidationService<NewResourceDTO> validator
) : AddUseCase<NewResourceDTO, ResourceDomain>(creator, validator)
{
    private readonly IReaderAsync<ulong, UserDomain> _userReader = userReader;

    /// <summary>
    /// Valida que el ejecutor tenga permisos de administrador o sea el profesor propietario del recurso.
    /// </summary>
    /// <param name="value">Datos del nuevo recurso.</param>
    /// <returns>Resultado exitoso o error de autorización.</returns>
    protected override Result<Unit, UseCaseError> ExtraValidation(UserActionDTO<NewResourceDTO> value)
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => value.Data.ProfessorId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    /// <summary>
    /// Valida asíncronamente que el profesor especificado exista en el sistema.
    /// </summary>
    /// <param name="value">Datos del nuevo recurso.</param>
    /// <returns>Resultado exitoso o error de validación de entrada.</returns>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<NewResourceDTO> value
    )
    {
        List<FieldErrorDTO> errors = [];

        var userSearch = await _userReader.GetAsync(value.Data.ProfessorId);
        if (userSearch is null)
            errors.Add(new() { Field = "professorId", Message = "Usuario no encontrado" });

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        return Unit.Value;
    }
}
