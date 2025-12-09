using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.Services;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

/// <summary>
/// Caso de uso base para la lectura o recuperación de entidades.
/// </summary>
/// <typeparam name="I">Tipo del identificador de la entidad.</typeparam>
/// <typeparam name="E">Tipo de la entidad de dominio a recuperar.</typeparam>
public abstract class ReadUseCase<I, E>(
    IReaderAsync<I, E> reader,
    IBusinessValidationService<I>? validator = null
) : IUseCaseAsync<I, E>
    where I : notnull
    where E : notnull
{
    /// <summary>
    /// Entidad encargada de recuperar una entidad por ID
    /// </summary>
    protected readonly IReaderAsync<I, E> _reader = reader;

    /// <summary>
    /// Entidad encargada de validar la entrada brindada
    /// </summary>
    protected readonly IBusinessValidationService<I>? _validator = validator;

    /// <inheritdoc/>
    public async Task<Result<E, UseCaseError>> ExecuteAsync(UserActionDTO<I> request)
    {
        if (_validator is not null)
        {
            var validation = _validator.IsValid(request.Data);

            if (validation.IsErr)
            {
                var errors = validation.UnwrapErr();
                return UseCaseErrors.Input(errors);
            }
        }

        var syncCheck = ExtraValidation(request);
        if (syncCheck.IsErr)
            return syncCheck.UnwrapErr();

        var asyncCheck = await ExtraValidationAsync(request);
        if (asyncCheck.IsErr)
            return asyncCheck.UnwrapErr();

        var record = await _reader.GetAsync(request.Data);

        if (record is null)
            return UseCaseErrors.NotFound();

        return record;
    }

    /// <summary>
    /// Realiza validaciones síncronas adicionales antes de la lectura.
    /// </summary>
    /// <param name="value">DTO de entrada con contexto de usuario.</param>
    /// <returns>Resultado exitoso o error de validación.</returns>
    protected virtual Result<Unit, UseCaseError> ExtraValidation(UserActionDTO<I> value) =>
        Result<Unit, UseCaseError>.Ok(Unit.Value);

    /// <summary>
    /// Realiza validaciones asíncronas adicionales antes de la lectura.
    /// </summary>
    /// <param name="value">DTO de entrada con contexto de usuario.</param>
    /// <returns>Tarea con resultado exitoso o error de validación.</returns>
    protected virtual Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<I> value
    ) => Task.FromResult(Result<Unit, UseCaseError>.Ok(Unit.Value));
}
