using System.ComponentModel;
using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassProfessors;

using IClassReader = IReaderAsync<string, ClassDomain>;
using IProfessorCreator = ICreatorAsync<ProfessorClassRelationDTO, ProfessorClassRelationDTO>;
using IProfessorReader = IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO>;
using IUserReader = IReaderAsync<ulong, UserDomain>;
using UseCaseResult = Task<Result<ProfessorClassRelationDTO, UseCaseErrorImpl>>;

/// <summary>
/// Implementa el caso de uso para añadir un usuario a una clase, validando
/// que el usuario posea los permisos requeridos (Professor o Administrador).
/// Utiliza el modelo de programación asincrónica (TAP) para la validación de dependencias.
/// </summary>
public class AddProfessorToClassUseCase
    : IUseCaseAsync<AddProfessorToClassDTO, ProfessorClassRelationDTO>
{
    private readonly IProfessorCreator _creator;
    private readonly IUserReader _userReader;
    private readonly IClassReader _classReader;
    private readonly IProfessorReader _professorReader;

    private readonly Dictionary<UserType, Func<AddProfessorToClassDTO, UseCaseResult>> _handlers;

    public AddProfessorToClassUseCase(
        IProfessorCreator creator,
        IUserReader userReader,
        IClassReader classReader,
        IProfessorReader professorRelationReader
    )
    {
        _creator = creator;
        _userReader = userReader;
        _classReader = classReader;
        _professorReader = professorRelationReader;

        _handlers = new Dictionary<UserType, Func<AddProfessorToClassDTO, UseCaseResult>>
        {
            [UserType.STUDENT] = dto => Task.FromResult(Result<ProfessorClassRelationDTO, UseCaseErrorImpl>.Err(UseCaseError.UnauthorizedError())),

            [UserType.ADMIN] = async dto => await AddProfessor(dto),

            [UserType.PROFESSOR] = async dto =>
            {
                var executorRelation = await _professorReader.GetAsync(
                    new() { ClassId = dto.ClassId, UserId = dto.Executor.Id }
                );

                if (executorRelation.IsNone || !executorRelation.Unwrap().IsOwner)
                    return UseCaseError.UnauthorizedError();

                return await AddProfessor(dto);
            },
        };
    }

    public async UseCaseResult ExecuteAsync(AddProfessorToClassDTO value)
    {
        var userSearch = await _userReader.GetAsync(value.UserId);
        List<FieldErrorDTO> errors = [];

        if (userSearch.IsNone)
            errors.Add(new() { Field = "userId", Message = "Usuario no encontrado" });

        var classSearch = await _classReader.GetAsync(value.ClassId);
        if (classSearch.IsNone)
            errors.Add(new() { Field = "classId", Message = "Clase no encontrada" });

        if (errors.Count > 0)
            return UseCaseError.Input(errors);

        var relationSearch = await _professorReader.GetAsync(
            new() { UserId = value.UserId, ClassId = value.ClassId }
        );

        if (relationSearch.IsSome)
            return relationSearch.Unwrap();

        if (!_handlers.TryGetValue(value.Executor.Role, out var handler))
            throw new InvalidEnumArgumentException($"Unexpected role {value.Executor.Role}");

        return await handler(value);
    }

    private async Task<ProfessorClassRelationDTO> AddProfessor(AddProfessorToClassDTO value)
    {
        var relation = await _creator.AddAsync(
            new()
            {
                Id = new() { UserId = value.UserId, ClassId = value.ClassId },
                IsOwner = value.IsOwner,
            }
        );

        return relation;
    }
}
