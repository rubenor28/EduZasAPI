/// <summary>
/// Define una interfaz para transformar o convertir un tipo en otro tipo específico.
/// </summary>
/// <typeparam name="T">Tipo de destino de la conversión.</typeparam>
/// <remarks>
/// Esta interfaz proporciona un contrato uniforme para realizar conversiones
/// entre tipos diferentes.
/// </remarks>
public interface IInto<T>
{
    /// <summary>
    /// Convierte la instancia actual en una instancia del tipo especificado.
    /// </summary>
    /// <returns>Una nueva instancia de tipo <typeparamref name="T"/> resultante de la conversión.</returns>
    T Into();
}
