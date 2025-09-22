namespace EduZasAPI.Application.Ports.Mappers;

/// <summary>
/// Define un contrato para transformar una instancia de un tipo fuente en un tipo destino.
/// </summary>
/// <typeparam name="TSource">Tipo de objeto de origen.</typeparam>
/// <typeparam name="TTarget">Tipo de objeto de destino.</typeparam>
public interface IFrom<TSource, TTarget>
{
    /// <summary>
    /// Crea una instancia de <typeparamref name="TTarget"/> a partir de un objeto de tipo <typeparamref name="TSource"/>.
    /// </summary>
    /// <param name="source">Objeto fuente a transformar.</param>
    /// <returns>Objeto transformado de tipo <typeparamref name="TTarget"/>.</returns>
    static abstract TTarget From(TSource source);
}
