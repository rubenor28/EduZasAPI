namespace Domain.ValueObjects;

/// <summary>
/// Proporciona métodos de fábrica para crear instancias de <see cref="Result{T, E}"/>.
/// </summary>
public static class Result
{
    /// <summary>
    /// Crea un resultado exitoso (`Ok`) con un valor.
    /// </summary>
    public static Result<T, Unit> Ok<T>(T value) => Result<T, Unit>.Ok(value);

    /// <summary>
    /// Crea un resultado fallido (`Err`) con un error.
    /// </summary>
    public static Result<Unit, E> Err<E>(E error) => Result<Unit, E>.Err(error);

    /// <summary>
    /// Crea un resultado exitoso (`Ok`) con un valor, especificando el tipo de error.
    /// </summary>
    public static Result<T, E> Ok<T, E>(T value) => Result<T, E>.Ok(value);

    /// <summary>
    /// Crea un resultado fallido (`Err`) con un error, especificando el tipo de valor.
    /// </summary>
    public static Result<T, E> Err<T, E>(E error) => Result<T, E>.Err(error);
}

/// <summary>
/// Representa un resultado que puede ser exitoso (`Ok`) con un valor de tipo <typeparamref name="T"/>,
/// o fallido (`Err`) con un error de tipo <typeparamref name="E"/>.
/// </summary>
/// <typeparam name="T">Tipo del valor en caso de éxito.</typeparam>
/// <typeparam name="E">Tipo del error en caso de fallo.</typeparam>
public abstract class Result<T, E>
{
    /// <summary>
    /// Obtiene un valor que indica si el resultado es `Ok`.
    /// </summary>
    public abstract bool IsOk { get; }

    /// <summary>
    /// Obtiene un valor que indica si el resultado es `Err`.
    /// </summary>
    public abstract bool IsErr { get; }

    /// <summary>
    /// Desenvuelve el valor de un resultado `Ok`.
    /// </summary>
    /// <returns>El valor contenido.</returns>
    /// <exception cref="InvalidOperationException">Se lanza si el resultado es `Err`.</exception>
    public abstract T Unwrap();

    /// <summary>
    /// Desenvuelve el error de un resultado `Err`.
    /// </summary>
    /// <returns>El error contenido.</returns>
    /// <exception cref="InvalidOperationException">Se lanza si el resultado es `Ok`.</exception>
    public abstract E UnwrapErr();

    /// <summary>
    /// Desenvuelve el valor de un resultado `Ok` o devuelve un valor por defecto si es `Err`.
    /// </summary>
    /// <param name="defaultValue">Valor por defecto a devolver en caso de error.</param>
    /// <returns>El valor contenido o el valor por defecto.</returns>
    public abstract T UnwrapOr(T defaultValue);

    /// <summary>
    /// Ejecuta una acción sobre el valor si es `Ok`, o sobre el error si es `Err`.
    /// </summary>
    /// <param name="fnOk">Acción a ejecutar en caso de `Ok`.</param>
    /// <param name="fnErr">Acción a ejecutar en caso de `Err`.</param>
    public abstract void Match(Action<T> fnOk, Action<E> fnErr);

    /// <summary>
    /// Proyecta el resultado a un nuevo valor, aplicando una función según sea `Ok` o `Err`.
    /// </summary>
    /// <typeparam name="U">Tipo del valor resultante.</typeparam>
    /// <param name="fnOk">Función a aplicar en caso de `Ok`.</param>
    /// <param name="fnErr">Función a aplicar en caso de `Err`.</param>
    /// <returns>El resultado de aplicar la función correspondiente.</returns>
    public abstract U Match<U>(Func<T, U> fnOk, Func<E, U> fnErr);

    /// <summary>
    /// Transforma el valor de un resultado `Ok` aplicando una función, manteniendo el error si es `Err`.
    /// </summary>
    /// <typeparam name="U">Tipo del nuevo valor.</typeparam>
    /// <param name="fn">Función a aplicar al valor contenido.</param>
    /// <returns>Un nuevo `Result` con el valor transformado o el error original.</returns>
    public abstract Result<U, E> Map<U>(Func<T, U> fn);

    /// <summary>
    /// Transforma el error de un resultado `Err` aplicando una función, manteniendo el valor si es `Ok`.
    /// </summary>
    /// <typeparam name="F">Tipo del nuevo error.</typeparam>
    /// <param name="fn">Función a aplicar al error contenido.</param>
    /// <returns>Un nuevo `Result` con el error transformado o el valor original.</returns>
    public abstract Result<T, F> MapErr<F>(Func<E, F> fn);

    /// <summary>
    /// Encadena una operación que devuelve un `Result`, aplicándola solo si el estado es `Ok`.
    /// </summary>
    /// <typeparam name="U">Tipo del nuevo valor.</typeparam>
    /// <param name="fn">Función que toma el valor y devuelve un nuevo `Result`.</param>
    /// <returns>El `Result` de la función si era `Ok`, o el mismo error si era `Err`.</returns>
    public abstract Result<U, E> AndThen<U>(Func<T, Result<U, E>> fn);

    /// <summary>
    /// Encadena una operación de recuperación que devuelve un `Result`, aplicándola solo si el estado es `Err`.
    /// </summary>
    /// <typeparam name="F">Tipo del nuevo error.</typeparam>
    /// <param name="fn">Función que toma el error y devuelve un nuevo `Result`.</param>
    /// <returns>El `Result` de la función si era `Err`, o el mismo valor si era `Ok`.</returns>
    public abstract Result<T, F> OrElse<F>(Func<E, Result<T, F>> fn);

    /// <summary>
    /// Ejecuta una acción si el resultado es `Ok`.
    /// </summary>
    /// <param name="action">Acción a ejecutar con el valor.</param>
    public abstract void IfOk(Action<T> action);

    /// <summary>
    /// Ejecuta una acción si el resultado es `Err`.
    /// </summary>
    /// <param name="action">Acción a ejecutar con el error.</param>
    public abstract void IfErr(Action<E> action);

    /// <summary>
    /// Crea un resultado exitoso (`Ok`) con el valor especificado.
    /// </summary>
    public static Result<T, E> Ok(T value) => new OkResult(value);

    /// <summary>
    /// Crea un resultado de error (`Err`) con el error especificado.
    /// </summary>
    public static Result<T, E> Err(E error) => new ErrResult(error);

    /// <summary>
    /// Convierte implícitamente un valor de tipo <typeparamref name="T"/> a un `Result` `Ok`.
    /// </summary>
    public static implicit operator Result<T, E>(T value) => new OkResult(value);

    /// <summary>
    /// Convierte implícitamente un valor de tipo <typeparamref name="E"/> a un `Result` `Err`.
    /// </summary>
    public static implicit operator Result<T, E>(E error) => new ErrResult(error);

    private sealed class OkResult(T value) : Result<T, E>
    {
        private readonly T _value = value;
        public override bool IsOk => true;
        public override bool IsErr => false;

        public override T Unwrap() => _value;

        public override E UnwrapErr() =>
            throw new InvalidOperationException($"Tried to UnwrapErr from Ok: {_value}");

        public override T UnwrapOr(T defaultValue) => _value;

        public override void Match(Action<T> fnOk, Action<E> fnErr) => fnOk(_value);

        public override U Match<U>(Func<T, U> fnOk, Func<E, U> fnErr) => fnOk(_value);

        public override Result<U, E> Map<U>(Func<T, U> fn) => Result<U, E>.Ok(fn(_value));

        public override Result<T, F> MapErr<F>(Func<E, F> fn) => Result<T, F>.Ok(_value);

        public override Result<U, E> AndThen<U>(Func<T, Result<U, E>> fn) => fn(_value);

        public override Result<T, F> OrElse<F>(Func<E, Result<T, F>> fn) => Result<T, F>.Ok(_value);

        public override void IfOk(Action<T> action) => action(_value);

        public override void IfErr(Action<E> action) { }
    }

    private sealed class ErrResult(E error) : Result<T, E>
    {
        private readonly E _error = error;
        public override bool IsOk => false;
        public override bool IsErr => true;

        public override T Unwrap() =>
            throw new InvalidOperationException($"Tried to Unwrap from Err: {_error}");

        public override E UnwrapErr() => _error;

        public override T UnwrapOr(T defaultValue) => defaultValue;

        public override void Match(Action<T> fnOk, Action<E> fnErr) => fnErr(_error);

        public override U Match<U>(Func<T, U> fnOk, Func<E, U> fnErr) => fnErr(_error);

        public override Result<U, E> Map<U>(Func<T, U> fn) => Result<U, E>.Err(_error);

        public override Result<T, F> MapErr<F>(Func<E, F> fn) => Result<T, F>.Err(fn(_error));

        public override Result<U, E> AndThen<U>(Func<T, Result<U, E>> fn) =>
            Result<U, E>.Err(_error);

        public override Result<T, F> OrElse<F>(Func<E, Result<T, F>> fn) => fn(_error);

        public override void IfOk(Action<T> action) { }

        public override void IfErr(Action<E> action) => action(_error);
    }
}
