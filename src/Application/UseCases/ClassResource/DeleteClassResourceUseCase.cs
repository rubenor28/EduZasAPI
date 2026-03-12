using Application.DAOs;
using Application.DTOs.ClassResources;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassResource;

/// <summary>
/// Caso de uso para eliminar la asociación de un recurso con una clase.
/// </summary>
public sealed class DeleteClassResourceUseCase(
    IDeleterAsync<ClassResourceIdDTO, ClassResourceDomain> deleter,
    IReaderAsync<ClassResourceIdDTO, ClassResourceDomain> reader,
    IReaderAsync<Guid, ResourceDomain> resourceReader,
    IBusinessValidationService<ClassResourceIdDTO>? validator = null
)
    : DeleteUseCase<ClassResourceIdDTO, ClassResourceDomain>(
        deleter,
        reader,
        validator
    )
{
    private readonly IReaderAsync<Guid, ResourceDomain> _resourceReader = resourceReader;

    /// <summary>
    /// Valida asíncronamente que el recurso exista y que el ejecutor sea el propietario o un administrador antes de eliminar la asociación.
    /// </summary>
    /// <param name="value">Datos de la acción de eliminación.</param>
    /// <param name="record">Entidad de asociación recurso-clase original.</param>
    /// <returns>Resultado exitoso o error de caso de uso.</returns>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ClassResourceIdDTO> value,
        ClassResourceDomain record
    )
    {
        var resource = await _resourceReader.GetAsync(value.Data.ResourceId);
        if (resource is null)
            return UseCaseErrors.NotFound();

        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => resource.ProfessorId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }
}
