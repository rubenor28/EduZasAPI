using System.Linq.Expressions;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Extensions;

/// <summary>
/// Extensiones para consultas dinámicas en <see cref="IQueryable{T}"/>.
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// Aplica Where si el valor (clase) no es nulo.
    /// </summary>
    /// <typeparam name="T">Entidad.</typeparam>
    /// <typeparam name="V">Valor (clase).</typeparam>
    /// <param name="source">Consulta.</param>
    /// <param name="value">Valor condicional.</param>
    /// <param name="predicateFactory">Fábrica de predicado.</param>
    /// <returns>Consulta filtrada o original.</returns>
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
    /// Aplica Where si el valor (struct) no es nulo.
    /// </summary>
    /// <typeparam name="T">Entidad.</typeparam>
    /// <typeparam name="V">Valor (struct).</typeparam>
    /// <param name="source">Consulta.</param>
    /// <param name="value">Valor condicional.</param>
    /// <param name="predicateFactory">Fábrica de predicado.</param>
    /// <returns>Consulta filtrada o original.</returns>
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
    /// Aplica filtro de texto usando <see cref="StringQueryDTO"/>.
    /// </summary>
    /// <typeparam name="T">Entidad.</typeparam>
    /// <param name="source">Consulta.</param>
    /// <param name="strQuery">Criterio de búsqueda.</param>
    /// <param name="selector">Propiedad a filtrar.</param>
    /// <returns>Consulta filtrada.</returns>
    /// <exception cref="InvalidDataException">Tipo de búsqueda no soportado.</exception>
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
    /// Construye expresión de igualdad exacta.
    /// </summary>
    private static Expression<Func<T, bool>> BuildEquals<T>(Expression<Func<T, string?>> selector, string text)
    {
        var param = selector.Parameters[0];
        var body = Expression.Equal(selector.Body, Expression.Constant(text, typeof(string)));
        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    /// <summary>
    /// Construye expresión LIKE.
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
    /// Escapa caracteres para LIKE SQL.
    /// </summary>
    /// <param name="input">Texto.</param>
    /// <returns>Texto escapado.</returns>
    public static string EscapeLike(string input) =>
        input.Replace(@"\", @"\\").Replace("%", @"\%").Replace("_", @"\_");
}
