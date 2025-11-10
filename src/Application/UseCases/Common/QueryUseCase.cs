using Application.DAOs;
using Application.DTOs.Common;
using Application.Services;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

/// <summary>
/// Caso de uso genérico para la ejecución de consultas paginadas
/// basado en criterios de búsqueda.
/// </summary>
/// <typeparam name="E">Tipo de la entidad resultante de la consulta.</typeparam>
/// <typeparam name="C">Tipo del criterio de búsqueda, debe implementar <see cref="ICriteriaDTO"/>.</typeparam>
public class QueryUseCase<C, E>(
    IQuerierAsync<E, C> querier,
    IBusinessValidationService<C>? validator = null
) : IUseCaseAsync<C, PaginatedQuery<E, C>>
    where C : notnull, CriteriaDTO
    where E : notnull
{
    protected readonly IQuerierAsync<E, C> _querier = querier;
    protected readonly IBusinessValidationService<C>? _validator = validator;

    /// <summary>
    /// Ejecuta la consulta asíncrona con los criterios especificados.
    /// </summary>
    /// <param name="criteria">Criterios de búsqueda.</param>
    /// <returns>
    /// Una consulta paginada de tipo <see cref="PaginatedQuery{E, C}"/>
    /// con los resultados correspondientes.
    /// </returns>
    public async Task<Result<PaginatedQuery<E, C>, UseCaseError>> ExecuteAsync(C criteria)
    {
        List<FieldErrorDTO> errors = [];

        if (criteria.Page < 1)
            errors.Add(new() { Field = "page", Message = "Formato invalido" });

        if (_validator is not null)
        {
            var result = _validator.IsValid(criteria);
            if (result.IsErr)
            {
                var errs = result.UnwrapErr();
                errors.AddRange(errs);
            }
        }

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        return await _querier.GetByAsync(criteria);
    }
}
