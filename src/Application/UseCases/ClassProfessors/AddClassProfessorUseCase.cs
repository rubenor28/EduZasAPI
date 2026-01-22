using Application.DAOs;
using Application.DTOs.ClassProfessors;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
using Domain.ValueObjects;

namespace Application.UseCases.ClassProfessors;

/// <summary>
/// Caso de uso para añadir un profesor a una clase.
/// </summary>
public class AddClassProfessorUseCase(
    ICreatorAsync<ClassProfessorDomain, NewClassProfessorDTO> creator,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> reader,
    IReaderAsync<ulong, UserDomain> userReader,
    IReaderAsync<string, ClassDomain> classReader,
    IBusinessValidationService<NewClassProfessorDTO>? validator = null
) : AddUseCase<NewClassProfessorDTO, ClassProfessorDomain>(creator, validator)
{
    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _reader = reader;
    private readonly IReaderAsync<ulong, UserDomain> _userReader = userReader;
    private readonly IReaderAsync<string, ClassDomain> _classReader = classReader;

    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<NewClassProfessorDTO> value
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(
                value.Executor.Id,
                value.Data.ClassId
            ),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        List<FieldErrorDTO> errors = [];

        (await _userReader.GetAsync(value.Data.UserId)).IfNull(() =>
            errors.Add(new() { Field = "userId", Message = "Usuario no encontrado" })
        );

        (await _classReader.GetAsync(value.Data.ClassId)).IfNull(() =>
            errors.Add(new() { Field = "classId", Message = "Clase no encontrada" })
        );

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        var relationSearch = await _reader.GetAsync(
            new() { UserId = value.Data.UserId, ClassId = value.Data.ClassId }
        );

        if (relationSearch is not null)
            return UseCaseErrors.Conflict("El recurso ya existe");

        return Unit.Value;
    }

    private async Task<bool> IsProfessorAuthorized(ulong professorId, string classId)
    {
        var professor = await _reader.GetAsync(new() { ClassId = classId, UserId = professorId });
        return professor is not null && professor.IsOwner;
    }
}
