using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.Services;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

/// <summary>
/// Clase base para casos de uso de consulta (Query) con paginación y validación.
/// </summary>
/// <typeparam name="C">Tipo de criterio de búsqueda.</typeparam>
/// <typeparam name="E">Tipo de entidad consultada.</typeparam>
public class QueryUseCase<C, E>(
    IQuerierAsync<E, C> querier,
    IBusinessValidationService<C>? validator = null
) : IUseCaseAsync<C, PaginatedQuery<E, C>>
    where C : notnull, CriteriaDTO
    where E : notnull
{
    protected readonly IQuerierAsync<E, C> _querier = querier;
    protected readonly IBusinessValidationService<C>? _validator = validator;

    ///<inheritdoc/>
    public async Task<Result<PaginatedQuery<E, C>, UseCaseError>> ExecuteAsync(
        UserActionDTO<C> request
    )
    {
        List<FieldErrorDTO> errors = [];

        if (request.Data.Page < 1)
            errors.Add(new() { Field = "page", Message = "Formato invalido" });

        if (_validator is not null)
        {
            var result = _validator.IsValid(request.Data);
            if (result.IsErr)
            {
                var errs = result.UnwrapErr();
                errors.AddRange(errs);
            }
        }

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        var validation = ExtraValidation(request);
        if (validation.IsErr)
            return validation.UnwrapErr();

        var validationAsync = await ExtraValidationAsync(request);
        if (validationAsync.IsErr)
            return validationAsync.UnwrapErr();

        var formatSync = PrevFormat(request);
        var formatAsync = await PrevFormatAsync(formatSync);

        return await _querier.GetByAsync(formatAsync.Data);
    }

    /// <summary>
    /// Realiza validaciones síncronas adicionales específicas del caso de uso.
    /// </summary>
    /// <param name="criteria">Criterios de búsqueda con contexto de usuario.</param>
    /// <returns>Resultado exitoso o error de validación.</returns>
    protected virtual Result<Unit, UseCaseError> ExtraValidation(UserActionDTO<C> criteria) =>
        Unit.Value;

    /// <summary>
    /// Realiza validaciones asíncronas adicionales específicas del caso de uso.
    /// </summary>
    /// <param name="criteria">Criterios de búsqueda con contexto de usuario.</param>
    /// <returns>Tarea con resultado exitoso o error de validación.</returns>
    protected virtual Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<C> criteria
    ) => Task.FromResult(Result<Unit, UseCaseError>.Ok(Unit.Value));

    /// <summary>
    /// Permite formatear o modificar los criterios antes de la consulta (síncrono).
    /// </summary>
    /// <param name="criteria">Referencia a los criterios de búsqueda.</param>
    protected virtual UserActionDTO<C> PrevFormat(UserActionDTO<C> criteria) => criteria;

    /// <summary>
    /// Permite formatear o modificar los criterios antes de la consulta (asíncrono).
    /// </summary>
    /// <param name="criteria">Referencia a los criterios de búsqueda.</param>
    protected virtual Task<UserActionDTO<C>> PrevFormatAsync(UserActionDTO<C> criteria) => Task.FromResult(criteria);
}
