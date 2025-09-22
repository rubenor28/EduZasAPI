namespace EduZasAPI.Domain.ValueObjects.Common;

/// <summary>
/// Representa un valor opcional que puede contener un valor de tipo T o no contener ningún valor.
/// </summary>
/// <typeparam name="T">Tipo del valor contenido. Debe ser un tipo no nulo.</typeparam>
/// <remarks>
/// Esta clase abstracta proporciona una forma segura de trabajar con valores que pueden estar presentes o no,
/// evitando el uso de referencias nulas. Inspirado en los tipos Option de lenguajes funcionales.
/// </remarks>
public abstract class Optional<T> where T : notnull
{

    /// <summary>
    /// Instancia única que representa un Optional vacío.
    /// </summary>
    private static readonly Optional<T> _none = new NoneOptional();

    /// <summary>
    /// Convierte un valor de referencia o nullable en un Optional.
    /// </summary>
    /// <param name="value">Valor a convertir.</param>
    /// <returns>
    /// Un Optional con el valor si no es nulo, o un Optional vacío si es nulo.
    /// </returns>
    public static Optional<T> ToOptional(T? value) =>
        value is null ? None() : Some(value);

    /// <summary>
    /// Convierte este Optional en un valor nullable.
    /// </summary>
    /// <returns>
    /// El valor contenido si existe; de lo contrario, <c>null</c> si <typeparamref name="T"/> es un tipo referencia
    /// o el valor por defecto de <typeparamref name="T"/> si es un tipo valor.
    /// </returns>
    public T? ToNullable() => IsSome ? Unwrap() : default;

    /// <summary>
    /// Crea un Optional que contiene el valor especificado.
    /// </summary>
    /// <param name="value">Valor a contener en el Optional.</param>
    /// <returns>Un Optional en estado Some con el valor especificado.</returns>
    public static Optional<T> Some(T value) => new SomeOptional(value);

    /// <summary>
    /// Crea un Optional vacío sin valor.
    /// </summary>
    /// <returns>Un Optional en estado None.</returns>
    public static Optional<T> None() => _none;

    /// <summary>
    /// Obtiene un valor que indica si el Optional contiene un valor.
    /// </summary>
    /// <value>true si contiene un valor; de lo contrario, false.</value>
    public abstract bool IsSome { get; }

    /// <summary>
    /// Obtiene un valor que indica si el Optional no contiene ningún valor.
    /// </summary>
    /// <value>true si no contiene valor; de lo contrario, false.</value>
    public abstract bool IsNone { get; }

    /// <summary>
    /// Desenvuelve el valor contenido en el Optional.
    /// </summary>
    /// <returns>El valor contenido si está presente.</returns>
    /// <exception cref="InvalidOperationException">
    /// Se lanza cuando se intenta desenvolver un Optional que no contiene valor.
    /// </exception>
    public abstract T Unwrap();

    /// <summary>
    /// Desenvuelve el valor contenido o devuelve un valor por defecto si está vacío.
    /// </summary>
    /// <param name="defaultValue">Valor por defecto a devolver si no hay valor contenido.</param>
    /// <returns>
    /// El valor contenido si está presente, o el valor por defecto si está vacío.
    /// </returns>
    public abstract T UnwrapOr(T defaultValue);

    /// <summary>
    /// Transforma el valor contenido aplicando una función mapper.
    /// </summary>
    /// <typeparam name="U">Tipo del nuevo valor resultante. Debe ser no nulo.</typeparam>
    /// <param name="mapper">Función que transforma el valor contenido.</param>
    /// <returns>
    /// Un nuevo Optional con el valor transformado si estaba presente, o vacío si estaba vacío.
    /// </returns>
    public abstract Optional<U> Map<U>(Func<T, U> mapper) where U : notnull;

    /// <summary>
    /// Encadena operaciones aplicando una función que devuelve otro Optional.
    /// </summary>
    /// <typeparam name="U">Tipo del nuevo valor resultante. Debe ser no nulo.</typeparam>
    /// <param name="binder">Función que toma el valor y devuelve un nuevo Optional.</param>
    /// <returns>
    /// El Optional devuelto por la función si había valor, o vacío si estaba vacío.
    /// </returns>
    public abstract Optional<U> AndThen<U>(Func<T, Optional<U>> binder) where U : notnull;

    /// <summary>
    /// Desenvuelve el valor contenido o ejecuta una función para obtener un valor por defecto.
    /// </summary>
    /// <param name="provider">Función que proporciona un valor por defecto.</param>
    /// <returns>
    /// El valor contenido si está presente, o el resultado de ejecutar la función provider.
    /// </returns>
    public abstract T UnwrapOrElse(Func<T> provider);

    /// <summary>
    /// Ejecuta una acción diferente dependiendo de si el Optional contiene un valor o está vacío.
    /// </summary>
    /// <param name="someAction">Acción a ejecutar si contiene un valor, recibiendo el valor contenido.</param>
    /// <param name="noneAction">Acción a ejecutar si no contiene valor.</param>
    /// <remarks>
    /// Este método permite manejar ambos casos (Some y None) de forma declarativa
    /// sin necesidad de verificar explícitamente el estado del Optional.
    /// </remarks>
    /// <example>
    /// optional.Match(
    ///     valor => Console.WriteLine($"Valor encontrado: {valor}"),
    ///     () => Console.WriteLine("No hay valor")
    /// );
    /// </example>
    public abstract void Match(Action<T> someAction, Action noneAction);

    /// <summary>
    /// Transforma el Optional en un valor de tipo U aplicando una función diferente según su estado.
    /// </summary>
    /// <typeparam name="U">Tipo del valor resultante de la transformación.</typeparam>
    /// <param name="someAction">Función a aplicar si contiene un valor, recibiendo el valor contenido.</param>
    /// <param name="noneAction">Función a aplicar si no contiene valor.</param>
    /// <returns>
    /// El resultado de aplicar la función correspondiente al estado del Optional.
    /// </returns>
    /// <remarks>
    /// Este método permite transformar un Optional en otro tipo de valor de forma segura,
    /// manejando explícitamente ambos casos posibles (Some y None).
    /// </remarks>
    /// <example>
    /// string mensaje = optional.Match(
    ///     valor => $"Valor: {valor}",
    ///     () => "No hay valor disponible"
    /// );
    /// </example>
    public abstract U Match<U>(Func<T, U> someAction, Func<U> noneAction);

    /// <summary>
    /// Devuelve este Optional si contiene valor, o ejecuta una función para obtener un Alternative.
    /// </summary>
    /// <param name="provider">Función que proporciona un Optional alternativo.</param>
    /// <returns>
    /// Este Optional si contiene valor, o el Optional devuelto por la función provider.
    /// </returns>
    public abstract Optional<T> OrElse(Func<Optional<T>> provider);

    /// <summary>
    /// Ejecuta la acción especificada si contiene valor.
    /// </summary>
    /// <param name="action">Acción a ejecutar.</param>
    public abstract void IfSome(Action<T> action);

    /// <summary>
    /// Ejecuta la acción especificada no contiene valor.
    /// </summary>
    /// <param name="action">Acción a ejecutar.</param>
    public abstract void IfNone(Action action);

    /// <summary>
    /// Implementación concreta de Optional para el estado Some con valor.
    /// </summary>
    private sealed class SomeOptional : Optional<T>
    {
        private readonly T _value;

        /// <summary>
        /// Inicializa una nueva instancia de la clase SomeOptional.
        /// </summary>
        /// <param name="value">Valor a contener en el Optional.</param>
        public SomeOptional(T value) => _value = value;

        /// <inheritdoc/>
        public override bool IsSome => true;

        /// <inheritdoc/>
        public override bool IsNone => false;

        /// <inheritdoc/>
        public override T Unwrap() => _value;

        /// <inheritdoc/>
        public override T UnwrapOr(T defaultValue) => _value;

        /// <inheritdoc/>
        public override T UnwrapOrElse(Func<T> provider) => _value;

        /// <inheritdoc/>
        public override void Match(Action<T> someAction, Action noneAction) => someAction(_value);

        /// <inheritdoc/>
        public override U Match<U>(Func<T, U> someAction, Func<U> noneAction) => someAction(_value);

        /// <inheritdoc/>
        public override Optional<U> Map<U>(Func<T, U> mapper) =>
            Optional<U>.Some(mapper(_value));

        /// <inheritdoc/>
        public override Optional<U> AndThen<U>(Func<T, Optional<U>> binder) =>
            binder(_value);

        /// <inheritdoc/>
        public override Optional<T> OrElse(Func<Optional<T>> provider) => this;

        /// <inheritdoc/>
        public override void IfSome(Action<T> action) => action(_value);

        /// <inheritdoc/>
        public override void IfNone(Action action) { }
    }

    /// <summary>
    /// Implementación concreta de Optional para el estado None sin valor.
    /// </summary>
    private sealed class NoneOptional : Optional<T>
    {
        /// <inheritdoc/>
        public override bool IsSome => false;

        /// <inheritdoc/>
        public override bool IsNone => true;

        /// <inheritdoc/>
        public override T Unwrap() =>
            throw new InvalidOperationException("Cannot unwrap None value");

        /// <inheritdoc/>
        public override T UnwrapOr(T defaultValue) => defaultValue;

        /// <inheritdoc/>
        public override T UnwrapOrElse(Func<T> provider) => provider();

        /// <inheritdoc/>
        public override void Match(Action<T> someAction, Action noneAction) => noneAction();

        /// <inheritdoc/>
        public override U Match<U>(Func<T, U> someAction, Func<U> noneAction) => noneAction();

        /// <inheritdoc/>
        public override Optional<U> Map<U>(Func<T, U> mapper) =>
            Optional<U>.None();

        /// <inheritdoc/>
        public override Optional<U> AndThen<U>(Func<T, Optional<U>> binder) =>
            Optional<U>.None();

        /// <inheritdoc/>
        public override Optional<T> OrElse(Func<Optional<T>> provider) =>
            provider();

        /// <inheritdoc/>
        public override void IfSome(Action<T> action) { }

        /// <inheritdoc/>
        public override void IfNone(Action action) => action();
    }
}
