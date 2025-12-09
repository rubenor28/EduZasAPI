namespace Domain.Extensions;

/// <summary>
/// Métodos de extensión de estilo funcional para manejar tipos de referencia anulables.
/// </summary>
public static class NullableClass
{
    /// <summary>
    /// Ejecuta una acción si el valor no es nulo.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto (clase).</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="action">La acción a ejecutar con el valor no nulo.</param>
    public static void IfSome<T>(this T? value, Action<T> action)
        where T : class
    {
        if (value is null)
            return;

        action(value);
    }

    /// <summary>
    /// Ejecuta una acción asíncrona si el valor no es nulo.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto (clase).</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="action">La función asíncrona a ejecutar con el valor no nulo.</param>
    public static Task IfSome<T>(this T? value, Func<T, Task> action)
        where T : class
    {
        if (value is null)
            return Task.CompletedTask;

        return action(value);
    }

    /// <summary>
    /// Ejecuta una acción si el valor es nulo.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto (clase).</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="action">La acción a ejecutar.</param>
    public static void IfNull<T>(this T? value, Action action)
        where T : class
    {
        if (value is not null)
            return;

        action();
    }

    /// <summary>
    /// Ejecuta una acción asíncrona si el valor es nulo.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto (clase).</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="action">La función asíncrona a ejecutar.</param>
    public static Task IfNull<T>(this T? value, Func<Task> action)
        where T : class
    {
        if (value is not null)
            return Task.CompletedTask;

        return action();
    }

    /// <summary>
    /// Devuelve el valor si no es nulo; de lo contrario, devuelve un valor por defecto.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto (clase).</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="defaultValue">El valor a devolver si 'value' es nulo.</param>
    /// <returns>El valor original o el valor por defecto. Análogo a '??'.</returns>
    public static T Or<T>(this T? value, T defaultValue)
        where T : class => value is not null ? value : defaultValue;

    /// <summary>
    /// Ejecuta una de dos acciones dependiendo de si el valor es nulo o no.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto (clase).</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="someAction">La acción para un valor no nulo.</param>
    /// <param name="nullAction">La acción para un valor nulo.</param>
    public static void Match<T>(this T? value, Action<T> someAction, Action nullAction)
        where T : class
    {
        if (value is not null)
            someAction(value);
        else
            nullAction();
    }

    /// <summary>
    /// Proyecta el valor a un nuevo tipo, ejecutando una de dos funciones según si es nulo.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto original.</typeparam>
    /// <typeparam name="U">El tipo del valor resultante.</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="someAction">La función para un valor no nulo.</param>
    /// <param name="nullAction">La función para un valor nulo.</param>
    /// <returns>El resultado de la función ejecutada.</returns>
    public static U Match<T, U>(this T? value, Func<T, U> someAction, Func<U> nullAction)
        where T : class
    {
        if (value is not null)
            return someAction(value);

        return nullAction();
    }

    /// <summary>
    /// Ejecuta una de dos acciones asíncronas dependiendo de si el valor es nulo o no.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto (clase).</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="someAction">La función asíncrona para un valor no nulo.</param>
    /// <param name="nullAction">La función asíncrona para un valor nulo.</param>
    public static Task Match<T>(this T? value, Func<T, Task> someAction, Func<Task> nullAction)
        where T : class
    {
        if (value is not null)
            return someAction(value);

        return nullAction();
    }

    /// <summary>
    /// Proyecta asíncronamente el valor a un nuevo tipo, ejecutando una de dos funciones según si es nulo.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto original.</typeparam>
    /// <typeparam name="U">El tipo del valor resultante.</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="someAction">La función asíncrona para un valor no nulo.</param>
    /// <param name="nullAction">La función asíncrona para un valor nulo.</param>
    /// <returns>Una tarea con el resultado de la función ejecutada.</returns>
    public static Task<U> Match<T, U>(
        this T? value,
        Func<T, Task<U>> someAction,
        Func<Task<U>> nullAction
    )
        where T : class
    {
        if (value is not null)
            return someAction(value);

        return nullAction();
    }

    /// <summary>
    /// Si el valor no es nulo, aplica una función transformadora sobre él.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto original (clase).</typeparam>
    /// <typeparam name="U">El tipo del valor resultante (clase).</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="foo">La función a aplicar al valor no nulo.</param>
    /// <returns>El valor transformado si el original no era nulo; de lo contrario, nulo. Análogo a '?.'.</returns>
    public static U? AndThen<T, U>(this T? value, Func<T, U?> foo)
        where T : class
        where U : class => value is not null ? foo(value) : null;

    /// <summary>
    /// Si el valor no es nulo, aplica una función transformadora sobre él.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto original (clase).</typeparam>
    /// <typeparam name="U">El tipo del valor resultante (struct).</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="foo">La función a aplicar al valor no nulo.</param>
    /// <returns>El valor transformado si el original no era nulo; de lo contrario, nulo. Análogo a '?.'.</returns>
    public static U? AndThen<T, U>(this T? value, Func<T, U?> foo)
        where T : class
        where U : struct => value is not null ? foo(value) : null;

    /// <summary>
    /// Devuelve el valor original si no es nulo; de lo contrario, ejecuta un proveedor para obtener un valor de respaldo.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto (clase).</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="provider">La función que proporciona un valor de respaldo si el original es nulo.</param>
    /// <returns>El valor original o el valor de respaldo.</returns>
    public static T? OrElse<T>(this T? value, Func<T?> provider)
        where T : class => value ?? provider();
}
