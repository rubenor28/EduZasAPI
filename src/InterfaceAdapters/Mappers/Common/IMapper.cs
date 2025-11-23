namespace InterfaceAdapters.Mappers.Common;

/// <summary>
/// Define un contrato para un mapeador que transforma un objeto de un tipo a otro.
/// </summary>
/// <typeparam name="TIn">El tipo del objeto de origen (entrada).</typeparam>
/// <typeparam name="TOut">El tipo del objeto de destino (salida).</typeparam>
public interface IMapper<TIn, TOut>
{
    /// <summary>
    /// Mapea un objeto de tipo <typeparamref name="TIn"/> a un objeto de tipo <typeparamref name="TOut"/>.
    /// </summary>
    /// <param name="input">El objeto de origen a mapear.</param>
    /// <returns>El objeto mapeado de tipo <typeparamref name="TOut"/>.</returns>
    public TOut Map(TIn input);
}

/// <summary>
/// Define un contrato para un mapeador que combina dos objetos de entrada en un único objeto de salida.
/// </summary>
/// <typeparam name="T1">El tipo del primer objeto de origen.</typeparam>
/// <typeparam name="T2">El tipo del segundo objeto de origen.</typeparam>
/// <typeparam name="TOut">El tipo del objeto de destino (salida).</typeparam>
public interface IMapper<T1, T2, TOut>
{
    /// <summary>
    /// Mapea dos objetos de origen a un único objeto de destino.
    /// </summary>
    /// <param name="in1">El primer objeto de origen.</param>
    /// <param name="in2">El segundo objeto de origen.</param>
    /// <returns>El objeto mapeado de tipo <typeparamref name="TOut"/>.</returns>
    public TOut Map(T1 in1, T2 in2);
}

/// <summary>
/// Define un contrato para un mapeador que combina tres objetos de entrada en un único objeto de salida.
/// </summary>
/// <typeparam name="T1">El tipo del primer objeto de origen.</typeparam>
/// <typeparam name="T2">El tipo del segundo objeto de origen.</typeparam>
/// <typeparam name="T3">El tipo del tercer objeto de origen.</typeparam>
/// <typeparam name="TOut">El tipo del objeto de destino (salida).</typeparam>
public interface IMapper<T1, T2, T3, TOut>
{
    /// <summary>
    /// Mapea tres objetos de origen a un único objeto de destino.
    /// </summary>
    /// <param name="in1">El primer objeto de origen.</param>
    /// <param name="in2">El segundo objeto de origen.</param>
    /// <param name="in3">El tercer objeto de origen.</param>
    /// <returns>El objeto mapeado de tipo <typeparamref name="TOut"/>.</returns>
    public TOut Map(T1 in1, T2 in2, T3 in3);
}

/// <summary>
/// Define un contrato para un mapeador que combina tres objetos de entrada en un único objeto de salida.
/// </summary>
/// <typeparam name="T1">El tipo del primer objeto de origen.</typeparam>
/// <typeparam name="T2">El tipo del segundo objeto de origen.</typeparam>
/// <typeparam name="T3">El tipo del tercer objeto de origen.</typeparam>
/// <typeparam name="T4">El tipo del cuarto objeto de origen.</typeparam>
/// <typeparam name="TOut">El tipo del objeto de destino (salida).</typeparam>
public interface IMapper<T1, T2, T3, T4, TOut>
{
    /// <summary>
    /// Mapea tres objetos de origen a un único objeto de destino.
    /// </summary>
    /// <param name="in1">El primer objeto de origen.</param>
    /// <param name="in2">El segundo objeto de origen.</param>
    /// <param name="in3">El tercer objeto de origen.</param>
    /// <param name="in4">El cuarto objeto de origen.</param>
    /// <returns>El objeto mapeado de tipo <typeparamref name="TOut"/>.</returns>
    public TOut Map(T1 in1, T2 in2, T3 in3, T4 in4);
}

/// <summary>
/// Define un contrato para un mapeador que combina tres objetos de entrada en un único objeto de salida.
/// </summary>
/// <typeparam name="T1">El tipo del primer objeto de origen.</typeparam>
/// <typeparam name="T2">El tipo del segundo objeto de origen.</typeparam>
/// <typeparam name="T3">El tipo del tercer objeto de origen.</typeparam>
/// <typeparam name="T4">El tipo del cuarto objeto de origen.</typeparam>
/// <typeparam name="T5">El tipo del quinto objeto de origen.</typeparam>
/// <typeparam name="TOut">El tipo del objeto de destino (salida).</typeparam>
public interface IMapper<T1, T2, T3, T4, T5, TOut>
{
    /// <summary>
    /// Mapea tres objetos de origen a un único objeto de destino.
    /// </summary>
    /// <param name="in1">El primer objeto de origen.</param>
    /// <param name="in2">El segundo objeto de origen.</param>
    /// <param name="in3">El tercer objeto de origen.</param>
    /// <param name="in4">El cuarto objeto de origen.</param>
    /// <param name="in5">El cuarto objeto de origen.</param>
    /// <returns>El objeto mapeado de tipo <typeparamref name="TOut"/>.</returns>
    public TOut Map(T1 in1, T2 in2, T3 in3, T4 in4, T5 in5);
}
