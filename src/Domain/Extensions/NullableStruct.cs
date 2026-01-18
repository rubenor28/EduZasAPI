namespace Domain.Extensions;

/// <summary>
/// Métodos de extensión de estilo funcional para manejar tipos de valor anulables (`Nullable<T>`).
/// </summary>
public static class NullableStruct
{
    /// <summary>
    /// Ejecuta una acción si el `Nullable<T>` tiene un valor.
    /// </summary>
    /// <typeparam name="T">El tipo del struct.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="action">La acción a ejecutar con el valor.</param>
    public static T? IfSome<T>(this T? value, Action<T> action)
        where T : struct
    {
        if (value.HasValue)
            action(value.Value);

        return value;
    }

    /// <summary>
    /// Ejecuta una acción si el `Nullable<T>` es nulo.
    /// </summary>
    /// <typeparam name="T">El tipo del struct.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="action">La acción a ejecutar.</param>
    public static T? IfNull<T>(this T? value, Action action)
        where T : struct
    {
        if (!value.HasValue)
            action();

        return value;
    }

    public static async Task<T?> IfSomeAsync<T>(this T? value, Func<T, Task> action)
        where T : struct
    {
        if (value.HasValue)
            await action(value.Value);

        return value;
    }

    public static async Task<T?> IfNullAsync<T>(this T? value, Func<Task> action)
        where T : struct
    {
        if (!value.HasValue)
            await action();

        return value;
    }

    /// <summary>
    /// Devuelve el valor si existe; de lo contrario, devuelve un valor por defecto.
    /// </summary>
    /// <typeparam name="T">El tipo del struct.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="defaultValue">El valor a devolver si 'value' es nulo.</param>
    /// <returns>El valor contenido o el valor por defecto. Análogo a '??'.</returns>
    public static T Or<T>(this T? value, T defaultValue)
        where T : struct => value ?? defaultValue;

    /// <summary>
    /// Ejecuta una de dos acciones dependiendo de si el `Nullable<T>` tiene valor o no.
    /// </summary>
    /// <typeparam name="T">El tipo del struct.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="someAction">La acción para un valor existente.</param>
    /// <param name="nullAction">La acción para un valor nulo.</param>
    public static void Match<T>(this T? value, Action<T> someAction, Action nullAction)
        where T : struct
    {
        if (value.HasValue)
            someAction(value.Value);
        else
            nullAction();
    }

    /// <summary>
    /// Proyecta el `Nullable<T>` a un nuevo tipo, ejecutando una de dos funciones según si tiene valor.
    /// </summary>
    /// <typeparam name="T">El tipo del struct original.</typeparam>
    /// <typeparam name="U">El tipo del valor resultante.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="someAction">La función para un valor existente.</param>
    /// <param name="nullAction">La función para un valor nulo.</param>
    /// <returns>El resultado de la función ejecutada.</returns>
    public static U Match<T, U>(this T? value, Func<T, U> someAction, Func<U> nullAction)
        where T : struct
    {
        if (value.HasValue)
            return someAction(value.Value);

        return nullAction();
    }

    /// <summary>
    /// Si el `Nullable<T>` tiene valor, aplica una función transformadora sobre él.
    /// </summary>
    /// <typeparam name="T">El tipo del struct original.</typeparam>
    /// <typeparam name="U">El tipo del valor resultante (clase).</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="foo">La función a aplicar al valor.</param>
    /// <returns>El valor transformado si el original tenía valor; de lo contrario, nulo. Análogo a '?.'.</returns>
    public static U? AndThen<T, U>(this T? value, Func<T, U?> foo)
        where T : struct
        where U : class => value.HasValue ? foo(value.Value) : null;

    /// <summary>
    /// Si el `Nullable<T>` tiene valor, aplica una función transformadora sobre él.
    /// </summary>
    /// <typeparam name="T">El tipo del struct original.</typeparam>
    /// <typeparam name="U">El tipo del valor resultante (struct).</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="foo">La función a aplicar al valor.</param>
    /// <returns>Un `Nullable<U>` con el valor transformado si el original tenía valor; de lo contrario, nulo. Análogo a '?.'.</returns>
    public static U? AndThen<T, U>(this T? value, Func<T, U?> foo)
        where T : struct
        where U : struct => value.HasValue ? foo(value.Value) : null;

    /// <summary>
    /// Devuelve el valor `Nullable<T>` si tiene valor; de lo contrario, ejecuta un proveedor para obtener un valor de respaldo.
    /// </summary>
    /// <typeparam name="T">El tipo del struct.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="provider">La función que proporciona un `Nullable<T>` de respaldo.</param>
    /// <returns>El valor original o el valor de respaldo.</returns>
    public static T? OrElse<T>(this T? value, Func<T?> provider)
        where T : struct => value ?? provider();
}
