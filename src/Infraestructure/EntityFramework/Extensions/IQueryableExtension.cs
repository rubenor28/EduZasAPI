using System.Linq.Expressions;
using Application.DTOs.Common;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Extensions;

/// <summary>
/// Métodos de extensión para construir consultas dinámicas sobre <see cref="IQueryable{T}"/>.
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// Aplica una condición <c>Where</c> a la consulta si el valor (clase) no es nulo.
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad en la consulta.</typeparam>
    /// <typeparam name="V">Tipo del valor, debe ser una clase.</typeparam>
    /// <param name="source">Fuente de datos a consultar.</param>
    /// <param name="value">Valor que determina si se aplica la condición.</param>
    /// <param name="predicateFactory">Función que construye la expresión de filtro en base al valor.</param>
    /// <returns>La consulta original si el valor es nulo, o una consulta filtrada si no es nulo.</returns>
    public static IQueryable<T> WhereOptional<T, V>(
        this IQueryable<T> source,
        V? value,
        Func<V, Expression<Func<T, bool>>> predicateFactory)
        where V : class
    {
        if (value is null) return source;
        return source.Where(predicateFactory(value));
    }

    /// <summary>
    /// Aplica una condición <c>Where</c> a la consulta si el valor (struct) no es nulo.
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad en la consulta.</typeparam>
    /// <typeparam name="V">Tipo del valor, debe ser un struct.</typeparam>
    /// <param name="source">Fuente de datos a consultar.</param>
    /// <param name="value">Valor anulable que determina si se aplica la condición.</param>
    /// <param name="predicateFactory">Función que construye la expresión de filtro en base al valor.</param>
    /// <returns>La consulta original si el valor es nulo, o una consulta filtrada si tiene valor.</returns>
    public static IQueryable<T> WhereOptional<T, V>(
        this IQueryable<T> source,
        V? value,
        Func<V, Expression<Func<T, bool>>> predicateFactory)
        where V : struct
    {
        if (!value.HasValue) return source;
        return source.Where(predicateFactory(value.Value));
    }

    /// <summary>
    /// Aplica un filtro de texto sobre un campo string de la entidad, usando un <see cref="StringQueryDTO"/>.
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad en la consulta.</typeparam>
    /// <param name="source">Fuente de datos a consultar.</param>
    /// <param name="strQuery">Consulta de texto opcional con el tipo de búsqueda y el valor.</param>
    /// <param name="selector">Selector de la propiedad string a filtrar.</param>
    /// <returns>Consulta filtrada según el tipo de búsqueda especificado, o sin cambios si no hay valor.</returns>
    /// <exception cref="InvalidDataException">Si el tipo de búsqueda no está soportado.</exception>
    public static IQueryable<T> WhereStringQuery<T>(
        this IQueryable<T> source,
        StringQueryDTO? strQuery,
        Expression<Func<T, string?>> selector)
    {
        if (strQuery is null) return source;

        return strQuery.SearchType switch
        {
            StringSearchType.EQ => source.Where(BuildEquals(selector, strQuery.Text)),
            StringSearchType.LIKE => source.Where(BuildLike(selector, strQuery.Text)),
            _ => throw new InvalidDataException($"Unsupported search type: {strQuery.SearchType}")
        };
    }

    /// <summary>
    /// Construye una expresión que compara igualdad exacta entre una propiedad string y un texto dado.
    /// </summary>
    private static Expression<Func<T, bool>> BuildEquals<T>(Expression<Func<T, string?>> selector, string text)
    {
        var param = selector.Parameters[0];
        var body = Expression.Equal(selector.Body, Expression.Constant(text, typeof(string)));
        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    /// <summary>
    /// Construye una expresión que aplica un patrón LIKE sobre una propiedad string.
    /// </summary>
    private static Expression<Func<T, bool>> BuildLike<T>(Expression<Func<T, string?>> selector, string text)
    {
        var param = selector.Parameters[0];
        var pattern = $"%{EscapeLike(text)}%";

        var efFunctions = Expression.Property(null, typeof(EF), nameof(EF.Functions));

        var likeMethod = typeof(DbFunctionsExtensions)
            .GetMethod(nameof(DbFunctionsExtensions.Like), [typeof(DbFunctions), typeof(string), typeof(string)])!;

        var call = Expression.Call(
            likeMethod,
            efFunctions,
            selector.Body,
            Expression.Constant(pattern, typeof(string))
        );

        return Expression.Lambda<Func<T, bool>>(call, param);
    }

    /// <summary>
    /// Escapa caracteres especiales para usarlos en un patrón LIKE de SQL.
    /// </summary>
    /// <param name="input">Texto a escapar.</param>
    /// <returns>Texto con caracteres escapados.</returns>
    public static string EscapeLike(string input) =>
        input.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_");
}
