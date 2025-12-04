using System;
using System.Threading.Tasks;

namespace Domain.Extensions;

/// <summary>
/// Proporciona métodos de extensión para tipos de valor anulables (structs).
/// Estos métodos ofrecen una sintaxis de estilo funcional para manejar valores `Nullable<T>`.
/// </summary>
public static class NullableStruct
{
    /// <summary>
    /// Ejecuta una acción si el valor `Nullable<T>` contiene un valor.
    /// </summary>
    /// <typeparam name="T">El tipo del struct.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="action">La acción a ejecutar con el valor no nulo.</param>
    /// <remarks>
    /// Este método es un contenedor sintáctico para un bloque 'if (value.HasValue) { action(value.Value); }'.
    /// </remarks>
    public static void IfSome<T>(this T? value, Action<T> action)
        where T : struct
    {
        if (value.HasValue)
        {
            action(value.Value);
        }
    }

    /// <summary>
    /// Ejecuta una acción asíncrona si el valor `Nullable<T>` contiene un valor.
    /// </summary>
    /// <typeparam name="T">El tipo del struct.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="action">La función que devuelve una tarea a ejecutar con el valor no nulo.</param>
    /// <returns>Una tarea que representa la operación.</returns>
    /// <remarks>
    /// Este método es un contenedor sintáctico para un bloque 'if (value.HasValue) { await action(value.Value); }'.
    /// </remarks>
    public static Task IfSome<T>(this T? value, Func<T, Task> action)
        where T : struct
    {
        if (value.HasValue)
        {
            return action(value.Value);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Ejecuta una acción si el valor `Nullable<T>` no contiene un valor (es nulo).
    /// </summary>
    /// <typeparam name="T">El tipo del struct.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="action">La acción a ejecutar.</param>
    /// <remarks>
    /// Este método es un contenedor sintáctico para un bloque 'if (!value.HasValue) { action(); }'.
    /// </remarks>
    public static void IfNull<T>(this T? value, Action action)
        where T : struct
    {
        if (!value.HasValue)
        {
            action();
        }
    }

    /// <summary>
    /// Ejecuta una acción asíncrona si el valor `Nullable<T>` no contiene un valor (es nulo).
    /// </summary>
    /// <typeparam name="T">El tipo del struct.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="action">La función que devuelve la tarea a ejecutar.</param>
    /// <returns>Una tarea que representa la operación.</returns>
    /// <remarks>
    /// Este método es un contenedor sintáctico para un bloque 'if (!value.HasValue) { await action(); }'.
    /// </remarks>
    public static Task IfNull<T>(this T? value, Func<Task> action)
        where T : struct
    {
        if (!value.HasValue)
        {
            return action();
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Devuelve el valor contenido si existe; de lo contrario, devuelve un valor por defecto.
    /// </summary>
    /// <typeparam name="T">El tipo del struct.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="defaultValue">El valor a devolver si 'value' no tiene contenido.</param>
    /// <returns>El valor contenido o el valor por defecto.</returns>
    /// <remarks>
    /// Esta función es una implementación explícita del operador de coalescencia nula '??'.
    /// Por ejemplo, 'miNullableInt.Or(0)' es equivalente a 'miNullableInt ?? 0'.
    /// </remarks>
    public static T Or<T>(this T? value, T defaultValue)
        where T : struct => value ?? defaultValue;

    /// <summary>
    /// Ejecuta una de dos acciones dependiendo de si el `Nullable<T>` tiene un valor o no.
    /// </summary>
    /// <typeparam name="T">El tipo del struct.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="someAction">La acción a ejecutar si el valor existe.</param>
    /// <param name="nullAction">La acción a ejecutar si el valor es nulo.</param>
    /// <remarks>
    /// Este método es funcionalmente equivalente a una estructura 'if (value.HasValue) { ... } else { ... }'.
    /// </remarks>
    public static void Match<T>(this T? value, Action<T> someAction, Action nullAction)
        where T : struct
    {
        if (value.HasValue)
            someAction(value.Value);
        else
            nullAction();
    }

    /// <summary>
    /// Proyecta el valor `Nullable<T>` a un nuevo tipo, ejecutando una de dos funciones dependiendo de si tiene valor.
    /// </summary>
    /// <typeparam name="T">El tipo del struct original.</typeparam>
    /// <typeparam name="U">El tipo del valor resultante.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="someAction">La función a ejecutar si el valor existe.</param>
    /// <param name="nullAction">La función a ejecutar si el valor es nulo.</param>
    /// <returns>El resultado de la función ejecutada.</returns>
    /// <remarks>
    /// Este método es funcionalmente equivalente a una expresión 'switch' o a una expresión ternaria:
    /// 'value.HasValue ? someAction(value.Value) : nullAction()'.
    /// </remarks>
    public static U Match<T, U>(this T? value, Func<T, U> someAction, Func<U> nullAction)
        where T : struct
    {
        if (value.HasValue)
            return someAction(value.Value);

        return nullAction();
    }

    /// <summary>
    /// Ejecuta una de dos acciones asíncronas dependiendo de si el `Nullable<T>` tiene valor o no.
    /// </summary>
    /// <typeparam name="T">El tipo del struct.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="someAction">La función a ejecutar si el valor existe, que devuelve una Tarea.</param>
    /// <param name="nullAction">La función a ejecutar si el valor es nulo, que devuelve una Tarea.</param>
    /// <returns>Una tarea que representa la operación.</returns>
    /// <remarks>
    /// Este método es funcionalmente equivalente a una estructura 'if (value.HasValue) { await ... } else { await ... }'.
    /// </remarks>
    public static Task Match<T>(this T? value, Func<T, Task> someAction, Func<Task> nullAction)
        where T : struct
    {
        if (value.HasValue)
            return someAction(value.Value);

        return nullAction();
    }

    /// <summary>
    /// Proyecta el valor `Nullable<T>` a un nuevo tipo de forma asíncrona, ejecutando una de dos funciones dependiendo de si tiene valor.
    /// </summary>
    /// <typeparam name="T">El tipo del struct original.</typeparam>
    /// <typeparam name="U">El tipo del valor resultante.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="someAction">La función asíncrona a ejecutar si el valor existe.</param>
    /// <param name="nullAction">La función asíncrona a ejecutar si el valor es nulo.</param>
    /// <returns>Una tarea que representa el resultado de la función ejecutada.</returns>
    /// <remarks>
    /// Este método es funcionalmente equivalente a una expresión ternaria con 'await':
    /// 'value.HasValue ? await someAction(value.Value) : await nullAction()'.
    /// </remarks>
    public static Task<U> Match<T, U>(
        this T? value,
        Func<T, Task<U>> someAction,
        Func<Task<U>> nullAction
    )
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
    /// <typeparam name="U">El tipo del valor resultante (una clase).</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="foo">La función a aplicar al valor no nulo.</param>
    /// <returns>El valor transformado si el original tenía valor; de lo contrario, nulo.</returns>
    /// <remarks>
    /// Esta función es una implementación explícita de 'Map' y es análoga al operador condicional nulo '?.'.
    /// Por ejemplo, 'miNullable.AndThen(v => v.Propiedad)' es similar a 'miNullable?.Propiedad'.
    /// </remarks>
    public static U? AndThen<T, U>(this T? value, Func<T, U?> foo)
        where T : struct
        where U : class => value.HasValue ? foo(value.Value) : null;

    /// <summary>
    /// Si el `Nullable<T>` tiene valor, aplica una función transformadora sobre él.
    /// </summary>
    /// <typeparam name="T">El tipo del struct original.</typeparam>
    /// <typeparam name="U">El tipo del valor resultante (un struct).</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="foo">La función a aplicar al valor no nulo.</param>
    /// <returns>El valor `Nullable<U>` transformado si el original tenía valor; de lo contrario, nulo.</returns>
    /// <remarks>
    /// Esta función es una implementación explícita de 'Map' y es análoga al operador condicional nulo '?.'.
    /// Por ejemplo, 'rectanguloNullable.AndThen(r => r.Ancho)' es similar a 'rectanguloNullable?.Ancho'.
    /// </remarks>
    public static U? AndThen<T, U>(this T? value, Func<T, U?> foo)
        where T : struct
        where U : struct => value.HasValue ? foo(value.Value) : null;

    /// <summary>
    /// Devuelve el valor `Nullable<T>` si contiene un valor; de lo contrario, ejecuta un proveedor para obtener un valor de respaldo.
    /// </summary>
    /// <typeparam name="T">El tipo del struct.</typeparam>
    /// <param name="value">El valor `Nullable<T>`.</param>
    /// <param name="provider">La función que proporciona un `Nullable<T>` de respaldo.</param>
    /// <returns>El valor original o el valor de respaldo.</returns>
    /// <remarks>
    /// Este método imita el comportamiento de 'Optional.OrElse' y es funcionalmente equivalente
    /// al operador de coalescencia nula '??' cuando se usa con un delegado.
    /// Por ejemplo, 'miNullableInt.OrElse(() => 10)' es similar a 'miNullableInt ?? 10'.
    /// </remarks>
    public static T? OrElse<T>(this T? value, Func<T?> provider)
        where T : struct => value ?? provider();
}
