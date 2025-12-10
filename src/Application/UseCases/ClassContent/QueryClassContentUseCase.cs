using Application.DAOs;
using Application.DTOs;
using Application.DTOs.ClassContent;
using Application.DTOs.Common;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassContent;

///<summary>
/// Define una entidad encargada de buscar los recursos o evaluaciones asociadas a una
/// clase
///<summary/>
public sealed class QueryClassContentUseCase(
    IQuerierAsync<ClassContentDTO, ClassContentCriteriaDTO> querier,
    IReaderAsync<UserClassRelationId, ClassStudentDomain> studentReader,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> professorReader
) : QueryUseCase<ClassContentCriteriaDTO, ClassContentDTO>(querier, null)
{
    private readonly IReaderAsync<UserClassRelationId, ClassStudentDomain> _studentReader =
        studentReader;
    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _professorReader =
        professorReader;
    private UserType? _userTypeOnClass = null;

    ///<inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ClassContentCriteriaDTO> criteria
    )
    {
        var userType = criteria.Executor.Role switch
        {
            UserType.ADMIN => UserType.ADMIN,
            UserType.PROFESSOR => await CheckProfessorClassUserType(
                criteria.Executor.Id,
                criteria.Data.ClassId
            ),
            UserType.STUDENT => await CheckStudentClassUserType(
                criteria.Executor.Id,
                criteria.Data.ClassId
            ),
            _ => throw new NotImplementedException(),
        };

        if (userType is null)
            return UseCaseErrors.Unauthorized();

        _userTypeOnClass = userType;

        return Unit.Value;
    }

    ///<inheritdoc/>
    protected override UserActionDTO<ClassContentCriteriaDTO> PrevFormat(
        UserActionDTO<ClassContentCriteriaDTO> criteria
    )
    {
        if (_userTypeOnClass is null)
            throw new NullReferenceException();

        return _userTypeOnClass switch
        {
            UserType.ADMIN or UserType.PROFESSOR => criteria,
            UserType.STUDENT => criteria with { Data = criteria.Data with { Visible = true } },
            _ => throw new NotImplementedException(),
        };
    }

    ///<summary>
    /// Obtener el tipo de usuario de un profesor sobre una clase especifica
    ///</summary>
    private async Task<UserType?> CheckProfessorClassUserType(ulong professorId, string classId)
    {
        var id = new UserClassRelationId { ClassId = classId, UserId = professorId };

        var professor = await _professorReader.GetAsync(id);
        if (professor is not null)
            return UserType.PROFESSOR;

        var student = await _studentReader.GetAsync(id);
        if (student is not null)
            return UserType.STUDENT;

        return null;
    }

    ///<summary>
    /// Obtener el tipo de usuario de un estudiante sobre una clase especifica
    ///</summary>
    private async Task<UserType?> CheckStudentClassUserType(ulong professorId, string classId)
    {
        var id = new UserClassRelationId { ClassId = classId, UserId = professorId };

        var student = await _studentReader.GetAsync(id);
        if (student is not null)
            return UserType.STUDENT;

        return null;
    }
}
