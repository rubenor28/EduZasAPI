namespace Domain.Extensions;

/// <summary>
/// Proporciona métodos de extensión para tipos de referencia anulables (clases).
/// Estos métodos ofrecen una sintaxis de estilo funcional para manejar valores nulos.
/// </summary>
public static class NullableClass
{
    /// <summary>
    /// Ejecuta una acción si el valor no es nulo.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto, restringido a ser una clase.</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="action">La acción a ejecutar con el valor no nulo.</param>
    /// <remarks>
    /// Este método es un contenedor sintáctico para un bloque 'if (value is not null) { action(value); }'.
    /// </remarks>
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
    /// <typeparam name="T">El tipo del objeto, restringido a ser una clase.</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="action">La función que devuelve una tarea a ejecutar con el valor no nulo.</param>
    /// <returns>Una tarea que representa la operación.</returns>
    /// <remarks>
    /// Este método es un contenedor sintáctico para un bloque 'if (value is not null) { await action(value); }'.
    /// </remarks>
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
    /// <typeparam name="T">El tipo del objeto, restringido a ser una clase.</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="action">La acción a ejecutar.</param>
    /// <remarks>
    /// Este método es un contenedor sintáctico para un bloque 'if (value is null) { action(); }'.
    /// </remarks>
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
    /// <typeparam name="T">El tipo del objeto, restringido a ser una clase.</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="action">La función que devuelve la tarea a ejecutar.</param>
    /// <returns>Una tarea que representa la operación.</returns>
    /// <remarks>
    /// Este método es un contenedor sintáctico para un bloque 'if (value is null) { await action(); }'.
    /// </remarks>
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
    /// <typeparam name="T">El tipo del objeto, restringido a ser una clase.</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="defaultValue">El valor a devolver si 'value' es nulo.</param>
    /// <returns>El valor original o el valor por defecto.</returns>
    /// <remarks>
    /// Esta función es una implementación explícita del operador de coalescencia nula '??'.
    /// Por ejemplo, 'miObjeto.Or(otroObjeto)' es equivalente a 'miObjeto ?? otroObjeto'.
    /// </remarks>
    public static T Or<T>(this T? value, T defaultValue)
        where T : class => value is not null ? value : defaultValue;

    /// <summary>
    /// Ejecuta una de dos acciones dependiendo de si el valor es nulo o no.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto, restringido a ser una clase.</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="someAction">La acción a ejecutar si el valor no es nulo.</param>
    /// <param name="nullAction">La acción a ejecutar si el valor es nulo.</param>
    /// <remarks>
    /// Este método es funcionalmente equivalente a una estructura 'if (value is not null) { ... } else { ... }'.
    /// </remarks>
    public static void Match<T>(this T? value, Action<T> someAction, Action nullAction)
        where T : class
    {
        if (value is not null)
            someAction(value);
        else
            nullAction();
    }

    /// <summary>
    /// Proyecta el valor a un nuevo tipo, ejecutando una de dos funciones dependiendo de si el valor es nulo.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto original.</typeparam>
    /// <typeparam name="U">El tipo del valor resultante.</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="someAction">La función a ejecutar si el valor no es nulo.</param>
    /// <param name="nullAction">La función a ejecutar si el valor es nulo.</param>
    /// <returns>El resultado de la función ejecutada.</returns>
    /// <remarks>
    /// Este método es funcionalmente equivalente a una expresión 'switch' o a una expresión ternaria:
    /// 'value is not null ? someAction(value) : nullAction()'.
    /// </remarks>
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
    /// <typeparam name="T">El tipo del objeto, restringido a ser una clase.</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="someAction">La función a ejecutar si el valor no es nulo, que devuelve una Tarea.</param>
    /// <param name="nullAction">La función a ejecutar si el valor es nulo, que devuelve una Tarea.</param>
    /// <returns>Una tarea que representa la operación.</returns>
    /// <remarks>
    /// Este método es funcionalmente equivalente a una estructura 'if (value is not null) { await ... } else { await ... }'.
    /// </remarks>
    public static Task Match<T>(this T? value, Func<T, Task> someAction, Func<Task> nullAction)
        where T : class
    {
        if (value is not null)
            return someAction(value);

        return nullAction();
    }

    /// <summary>
    /// Proyecta el valor a un nuevo tipo de forma asíncrona, ejecutando una de dos funciones dependiendo de si el valor es nulo.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto original.</typeparam>
    /// <typeparam name="U">El tipo del valor resultante.</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="someAction">La función asíncrona a ejecutar si el valor no es nulo.</param>
    /// <param name="nullAction">La función asíncrona a ejecutar si el valor es nulo.</param>
    /// <returns>Una tarea que representa el resultado de la función ejecutada.</returns>
    /// <remarks>
    /// Este método es funcionalmente equivalente a una expresión ternaria con 'await':
    /// 'value is not null ? await someAction(value) : await nullAction()'.
    /// </remarks>
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

    /// Si el valor no es nulo, aplica una función transformadora sobre él.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto original.</typeparam>
    /// <typeparam name="U">El tipo del valor resultante.</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="foo">La función a aplicar al valor no nulo.</param>
    /// <returns>El valor transformado si el original no era nulo; de lo contrario, nulo.</returns>
    /// <remarks>
    /// Esta función es una implementación explícita de 'Map' y es análoga al operador condicional nulo '?.'.
    /// Por ejemplo, 'usuario.AndThen(u => u.Direccion)' es similar a 'usuario?.Direccion'.
    /// </remarks>
    public static U? AndThen<T, U>(this T? value, Func<T, U?> foo)
        where T : class
        where U : class => value is not null ? foo(value) : null;

    /// <summary>
    /// Si el valor no es nulo, aplica una función transformadora sobre él.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto original.</typeparam>
    /// <typeparam name="U">El tipo del valor resultante (un struct).</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="foo">La función a aplicar al valor no nulo.</param>
    /// <returns>El valor transformado si el original no era nulo; de lo contrario, nulo.</returns>
    /// <remarks>
    /// Esta función es una implementación explícita de 'Map' y es análoga al operador condicional nulo '?.'.
    /// Por ejemplo, 'usuario.AndThen(u => u.Edad)' es similar a 'usuario?.Edad'.
    /// </remarks>
    public static U? AndThen<T, U>(this T? value, Func<T, U?> foo)
        where T : class
        where U : struct => value is not null ? foo(value) : null;

    /// <summary>
    /// Devuelve el valor original si no es nulo; de lo contrario, ejecuta un proveedor para obtener un valor de respaldo.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto, restringido a ser una clase.</typeparam>
    /// <param name="value">El objeto que puede ser nulo.</param>
    /// <param name="provider">La función que proporciona un valor de respaldo si el original es nulo.</param>
    /// <returns>El valor original o el valor de respaldo.</returns>
    /// <remarks>
    /// Este método imita el comportamiento de 'Optional.OrElse' y es funcionalmente equivalente
    /// al operador de coalescencia nula '??' cuando se usa con un delegado.
    /// Por ejemplo, 'miObjeto.OrElse(() => new Objeto())' es similar a 'miObjeto ?? new Objeto()'.
    /// </remarks>
    public static T? OrElse<T>(this T? value, Func<T?> provider)
        where T : class => value ?? provider();
}
