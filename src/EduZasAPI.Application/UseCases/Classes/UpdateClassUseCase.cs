using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Application.Common;

namespace EduZasAPI.Application.Classes;

/// <summary>
/// Caso de uso para actualizar una clase.
/// </summary>
public class UpdateClassUseCase : UpdateUseCase<ClassUpdateDTO, ClassDomain>
{
    private readonly IQuerierAsync<ProfessorClassRelationDTO, ProfessorClassRelationCriteriaDTO> _professorClassQuerier;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="UpdateClassUseCase"/>.
    /// </summary>
    /// <param name="updater">Servicio para actualizar la entidad de la clase.</param>
    /// <param name="validator">Servicio para validar los datos de la clase a actualizar.</param>
    /// <param name="professorClassQuerier">Servicio para consultar la relación entre profesores y clases.</param>
    public UpdateClassUseCase(
        IUpdaterAsync<ClassDomain, ClassUpdateDTO> updater,
        IBusinessValidationService<ClassUpdateDTO> validator,
        IQuerierAsync<ProfessorClassRelationDTO, ProfessorClassRelationCriteriaDTO> professorClassQuerier) :
      base(updater, validator)
    {
        _professorClassQuerier = professorClassQuerier;
    }

    /// <summary>
    /// Realiza validaciones adicionales para la actualización de la clase.
    /// </summary>
    /// <param name="value">DTO con los datos de la clase a actualizar.</param>
    /// <returns>Un resultado que indica si la validación fue exitosa o no.</returns>
    protected async override Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(ClassUpdateDTO value)
    {
        var result = await _professorClassQuerier.GetByAsync(new ProfessorClassRelationCriteriaDTO
        {
            Page = 1,
            UserId = Optional<ulong>.Some(value.Professor),
            ClassId = value.Id.ToOptional(),
        });

        if (result.Results.Count == 0 || !result.Results[0].IsOwner)
            return Result.Err(UseCaseError.UnauthorizedError());

        return Result<Unit, UseCaseErrorImpl>.Ok(Unit.Value);
    }
}
