using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Common;

/// <summary>
/// Caso de uso para eliminar una entidad de forma asíncrona.
/// </summary>
/// <typeparam name="I">Tipo del identificador de la entidad.</typeparam>
/// <typeparam name="E">Tipo de la entidad que se va a eliminar, debe implementar <see cref="IIdentifiable{I}"/>.</typeparam>
public class DeleteUseCase<I, E> : IUseCaseAsync<I, Optional<E>, List<FieldErrorDTO>>
    where I : notnull
    where E : notnull, IIdentifiable<I>
{
    private readonly IDeleterAsync<I, E> _deleter;
    private readonly IBusinessValidationService<I> _validator;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="DeleteUseCase{I, E}"/>.
    /// </summary>
    /// <param name="deleter">Servicio que realiza la eliminación de la entidad.</param>
    /// <param name="validator">Servicio que valida las reglas de negocio antes de eliminar.</param>
    public DeleteUseCase(IDeleterAsync<I, E> deleter, IBusinessValidationService<I> validator)
    {
        _deleter = deleter;
        _validator = validator;
    }

    /// <summary>
    /// Ejecuta la eliminación de la entidad identificada por el ID proporcionado.
    /// </summary>
    /// <param name="id">Identificador de la entidad a eliminar.</param>
    /// <returns>
    /// Resultado que contiene la entidad eliminada si existía, o errores de validación
    /// si la eliminación no se puede realizar.
    /// </returns>
    public async Task<Result<Optional<E>, List<FieldErrorDTO>>> ExecuteAsync(I id)
    {
        var validation = _validator.IsValid(id);

        if (validation.IsErr)
        {
            var errors = validation.UnwrapErr();
            return Result<Optional<E>, List<FieldErrorDTO>>.Err(errors);
        }

        var recordReaded = await _deleter.DeleteAsync(id);
        return Result<Optional<E>, List<FieldErrorDTO>>.Ok(recordReaded);
    }
}
