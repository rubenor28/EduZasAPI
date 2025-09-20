namespace EduZasAPI.Domain.ValueObjects.Common;

/// <summary>
/// Define una interfaz para entidades que poseen un identificador único de tipo específico.
/// </summary>
/// <typeparam name="T">Tipo del identificador único. Debe ser un tipo no nulo.</typeparam>
/// <remarks>
/// Esta interfaz se utiliza para garantizar que las entidades implementen
/// una propiedad de identificador único que no pueda ser nula.
/// </remarks>
public interface IIdentifiable<T> where T : notnull
{
    /// <summary>
    /// Obtiene el identificador único de la entidad.
    /// </summary>
    /// <value>Identificador único de tipo <typeparamref name="T"/>.</value>
    public T Id { get; }
}
