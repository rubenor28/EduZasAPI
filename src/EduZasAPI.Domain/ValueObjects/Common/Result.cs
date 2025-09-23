namespace EduZasAPI.Domain.ValueObjects.Common;

/// <summary>
/// Métodos de fábrica para crear instancias de <see cref="Result{T, E}"/>.
/// </summary>
public static class Result
{
    /// <summary>
    /// Crea un resultado exitoso que contiene un valor.
    /// </summary>
    /// <typeparam name="T">Tipo del valor.</typeparam>
    /// <param name="value">Valor a encapsular.</param>
    /// <returns>Un resultado exitoso con el valor proporcionado.</returns>
    public static Result<T, Unit> Ok<T>(T value) where T : notnull
        => Result<T, Unit>.Ok(value);

    /// <summary>
    /// Crea un resultado exitoso que contiene un valor.
    /// </summary>
    /// <typeparam name="T">Tipo del valor.</typeparam>
    /// <typeparam name="E">Tipo del error.</typeparam>
    /// <param name="value">Valor a encapsular.</param>
    /// <returns>Un resultado exitoso con el valor proporcionado.</returns>
    public static Result<T, E> Ok<T, E>(T value) where T : notnull where E : notnull
        => Result<T, E>.Ok(value);

    /// <summary>
    /// Crea un resultado fallido que contiene un error.
    /// </summary>
    /// <typeparam name="T">Tipo del valor.</typeparam>
    /// <typeparam name="E">Tipo del error.</typeparam>
    /// <param name="error">Error a encapsular.</param>
    /// <returns>Un resultado fallido con el error proporcionado.</returns>
    public static Result<T, E> Err<T, E>(E error) where T : notnull where E : notnull
        => Result<T, E>.Err(error);
}

/// <summary>
/// Representa un resultado que puede ser exitoso (Ok) con un valor de tipo T
/// o un error (Err) con un valor de tipo E.
/// </summary>
/// <typeparam name="T">Tipo del valor en caso de éxito.</typeparam>
/// <typeparam name="E">Tipo del error en caso de fallo.</typeparam>
public abstract class Result<T, E>
    where T : notnull
    where E : notnull
{
    /// <summary>
    /// Obtiene un valor que indica si el resultado es exitoso (Ok).
    /// </summary>
    /// <value>true si el resultado es exitoso; de lo contrario, false.</value>
    public abstract bool IsOk { get; }

    /// <summary>
    /// Obtiene un valor que indica si el resultado es un error (Err).
    /// </summary>
    /// <value>true si el resultado es un error; de lo contrario, false.</value>
    public abstract bool IsErr { get; }

    /// <summary>
    /// Desenvuelve el valor del resultado exitoso.
    /// </summary>
    /// <returns>El valor contenido en el resultado Ok.</returns>
    /// <exception cref="InvalidOperationException">
    /// Se lanza cuando se intenta desenvolver un resultado Err.
    /// </exception>
    public abstract T Unwrap();

    /// <summary>
    /// Desenvuelve el error del resultado no exitoso.
    /// </summary>
    /// <returns>El valor contenido en el resultado Err.</returns>
    /// <exception cref="InvalidOperationException">
    /// Se lanza cuando se intenta desenvolver un resultado Ok.
    /// </exception>
    public abstract E UnwrapErr();

    /// <summary>
    /// Desenvuelve el valor del resultado exitoso o devuelve un valor por defecto si es un error.
    /// </summary>
    /// <param name="defaultValue">Valor por defecto a devolver en caso de error.</param>
    /// <returns>
    /// El valor contenido en el resultado Ok o el valor por defecto si es un Err.
    /// </returns>
    public abstract T UnwrapOr(T defaultValue);

    /// <summary>
    /// Ejecuta una acción diferente dependiendo de si el Result es Ok o Err.
    /// </summary>
    /// <param name="fnOk">Acción a ejecutar si el Result es Ok, recibiendo el valor contenido.</param>
    /// <param name="fnErr">Acción a ejecutar si el Result es Err, recibiendo el error contenido.</param>
    /// <remarks>
    /// Este método permite manejar ambos casos (éxito y error) de forma declarativa
    /// sin necesidad de verificar explícitamente el estado del Result.
    /// </remarks>
    /// <example>
    /// resultado.Match(
    ///     valor => Console.WriteLine($"Éxito: {valor}"),
    ///     error => Console.WriteLine($"Error: {error}")
    /// );
    /// </example>
    public abstract void Match(Action<T> fnOk, Action<E> fnErr);

    /// <summary>
    /// Transforma el Result en un valor de tipo U aplicando una función diferente según su estado.
    /// </summary>
    /// <typeparam name="U">Tipo del valor resultante de la transformación.</typeparam>
    /// <param name="fnOk">Función a aplicar si el Result es Ok, recibiendo el valor contenido.</param>
    /// <param name="fnErr">Función a aplicar si el Result es Err, recibiendo el error contenido.</param>
    /// <returns>
    /// El resultado de aplicar la función correspondiente al estado del Result.
    /// </returns>
    /// <remarks>
    /// Este método permite transformar un Result en otro tipo de valor de forma segura,
    /// manejando explícitamente ambos casos posibles (éxito y error).
    /// </remarks>
    /// <example>
    /// string mensaje = resultado.Match(
    ///     valor => $"Operación exitosa: {valor}",
    ///     error => $"Error en la operación: {error}"
    /// );
    /// </example>
    public abstract U Match<U>(Func<T, U> fnOk, Func<E, U> fnErr);

    /// <summary>
    /// Aplica una función al valor contenido si el resultado es Ok.
    /// </summary>
    /// <typeparam name="U">Tipo del nuevo valor resultante.</typeparam>
    /// <param name="fn">Función a aplicar al valor contenido.</param>
    /// <returns>
    /// Un nuevo Result con el valor transformado si era Ok, o el mismo error si era Err.
    /// </returns>
    public abstract Result<U, E> Map<U>(Func<T, U> fn) where U : notnull;

    /// <summary>
    /// Aplica una función al error contenido si el resultado es Err.
    /// </summary>
    /// <typeparam name="F">Tipo del nuevo error resultante.</typeparam>
    /// <param name="fn">Función a aplicar al error contenido.</param>
    /// <returns>
    /// Un nuevo Result con el error transformado si era Err, o el mismo valor si era Ok.
    /// </returns>
    public abstract Result<T, F> MapErr<F>(Func<E, F> fn) where F : notnull;

    /// <summary>
    /// Encadena operaciones aplicando una función que devuelve otro Result.
    /// </summary>
    /// <typeparam name="U">Tipo del nuevo valor resultante.</typeparam>
    /// <param name="fn">Función que toma el valor y devuelve un nuevo Result.</param>
    /// <returns>
    /// El Result devuelto por la función si era Ok, o el mismo error si era Err.
    /// </returns>
    public abstract Result<U, E> AndThen<U>(Func<T, Result<U, E>> fn) where U : notnull;

    /// <summary>
    /// Maneja errores aplicando una función que devuelve otro Result en caso de error.
    /// </summary>
    /// <typeparam name="F">Tipo del nuevo error resultante.</typeparam>
    /// <param name="fn">Función que toma el error y devuelve un nuevo Result.</param>
    /// <returns>
    /// El Result devuelto por la función si era Err, o el mismo valor si era Ok.
    /// </returns>
    public abstract Result<T, F> OrElse<F>(Func<E, Result<T, F>> fn) where F : notnull;

    /// <summary>
    /// Ejecuta una acción si el resultado es exitoso.
    /// </summary>
    /// <param name="action">Acción a ejecutar con el valor exitoso.</param>
    public abstract void IfOk(Action<T> action);

    /// <summary>
    /// Ejecuta una acción si el resultado contiene un error.
    /// </summary>
    /// <param name="action">Acción a ejecutar con el valor de error.</param>
    public abstract void IfErr(Action<E> action);

    /// <summary>
    /// Crea un resultado exitoso (Ok) con el valor especificado.
    /// </summary>
    /// <param name="value">Valor a contener en el resultado exitoso.</param>
    /// <returns>Una instancia de Result en estado Ok.</returns>
    public static Result<T, E> Ok(T value) => new OkResult(value);

    /// <summary>
    /// Crea un resultado de error (Err) con el error especificado.
    /// </summary>
    /// <param name="error">Error a contener en el resultado fallido.</param>
    /// <returns>Una instancia de Result en estado Err.</returns>
    public static Result<T, E> Err(E error) => new ErrResult(error);

    /// <summary>
    /// Implementación concreta de Result para el estado Ok.
    /// </summary>
    private sealed class OkResult : Result<T, E>
    {
        private readonly T _value;

        /// <summary>
        /// Inicializa una nueva instancia de la clase OkResult.
        /// </summary>
        /// <param name="value">Valor a contener en el resultado exitoso.</param>
        public OkResult(T value) => _value = value;

        /// <inheritdoc/>
        public override bool IsOk => true;

        /// <inheritdoc/>
        public override bool IsErr => false;

        /// <inheritdoc/>
        public override T Unwrap() => _value;

        /// <inheritdoc/>
        public override E UnwrapErr() => throw new InvalidOperationException($"Tried to UnwrapErr from Ok: {_value}");

        /// <inheritdoc/>
        public override T UnwrapOr(T defaultValue) => _value;

        /// <inheritdoc/>
        public override void Match(Action<T> fnOk, Action<E> fnErr) => fnOk(_value);

        /// <inheritdoc/>
        public override U Match<U>(Func<T, U> fnOk, Func<E, U> fnErr) => fnOk(_value);

        /// <inheritdoc/>
        public override Result<U, E> Map<U>(Func<T, U> fn) => Result<U, E>.Ok(fn(_value));

        /// <inheritdoc/>
        public override Result<T, F> MapErr<F>(Func<E, F> fn) => Result<T, F>.Ok(_value);

        /// <inheritdoc/>
        public override Result<U, E> AndThen<U>(Func<T, Result<U, E>> fn) => fn(_value);

        /// <inheritdoc/>
        public override Result<T, F> OrElse<F>(Func<E, Result<T, F>> fn) => Result<T, F>.Ok(_value);

        /// <inheritdoc/>
        public override void IfOk(Action<T> action) => action(_value);

        /// <inheritdoc/>
        public override void IfErr(Action<E> action) { }
    }

    /// <summary>
    /// Implementación concreta de Result para el estado Err.
    /// </summary>
    private sealed class ErrResult : Result<T, E>
    {
        private readonly E _error;

        /// <summary>
        /// Inicializa una nueva instancia de la clase ErrResult.
        /// </summary>
        /// <param name="error">Error a contener en el resultado fallido.</param>
        public ErrResult(E error) => _error = error;

        /// <inheritdoc/>
        public override bool IsOk => false;

        /// <inheritdoc/>
        public override bool IsErr => true;

        /// <inheritdoc/>
        public override T Unwrap() => throw new InvalidOperationException($"Tried to Unwrap from Err: {_error}");

        /// <inheritdoc/>
        public override E UnwrapErr() => _error;

        /// <inheritdoc/>
        public override T UnwrapOr(T defaultValue) => defaultValue;

        /// <inheritdoc/>
        public override void Match(Action<T> fnOk, Action<E> fnErr) => fnErr(_error);

        /// <inheritdoc/>
        public override U Match<U>(Func<T, U> fnOk, Func<E, U> fnErr) => fnErr(_error);

        /// <inheritdoc/>
        public override Result<U, E> Map<U>(Func<T, U> fn) => Result<U, E>.Err(_error);

        /// <inheritdoc/>
        public override Result<T, F> MapErr<F>(Func<E, F> fn) => Result<T, F>.Err(fn(_error));

        /// <inheritdoc/>
        public override Result<U, E> AndThen<U>(Func<T, Result<U, E>> fn) => Result<U, E>.Err(_error);

        /// <inheritdoc/>
        public override Result<T, F> OrElse<F>(Func<E, Result<T, F>> fn) => fn(_error);

        /// <inheritdoc/>
        public override void IfOk(Action<T> action) { }

        /// <inheritdoc/>
        public override void IfErr(Action<E> action) => action(_error);
    }
}
