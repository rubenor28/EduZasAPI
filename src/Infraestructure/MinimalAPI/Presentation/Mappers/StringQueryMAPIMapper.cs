using Application.DTOs.Common;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Presentation.Mappers;

/// <summary>
/// Métodos de extensión para convertir <see cref="StringQueryMAPI"/> a objetos de dominio.
/// </summary>
public static class StringQueryMAPIMapper
{
    /// <summary>
    /// Convierte una instancia de <see cref="StringQueryMAPI"/> a un <see cref="StringQueryDTO"/>.
    /// </summary>
    /// <param name="source">Instancia de <see cref="StringQueryMAPI"/> a convertir.</param>
    /// <returns>
    /// Un <see cref="Result{T, E}"/> que contiene un <see cref="StringQueryDTO"/> si la conversión es exitosa,
    /// o un error <see cref="Unit"/> en caso de que el tipo de búsqueda sea inválido.
    /// </returns>
    public static Result<StringQueryDTO, Unit> ToDomain(this StringQueryMAPI source)
    {
        var type = StringSearchMapper.FromString(source.SearchType);

        if (type.IsErr)
            return Unit.Value;

        return new StringQueryDTO { SearchType = type.Unwrap(), Text = source.Text };
    }

    public static StringQueryMAPI FromDomain(this StringQueryDTO source) =>
        new() { SearchType = source.SearchType.ToString(), Text = source.Text };

    public static StringQueryMAPI? FromDomain(this Optional<StringQueryDTO> source)
    {
        if (source.IsNone)
            return null;

        var value = source.Unwrap();
        return value.FromDomain();
    }

    /// <summary>
    /// Convierte una instancia opcional de <see cref="StringQueryMAPI"/> a un <see cref="Optional{StringQueryDTO}"/>.
    /// </summary>
    /// <param name="source">Instancia opcional de <see cref="StringQueryMAPI"/> a convertir.</param>
    /// <returns>
    /// Un <see cref="Result{T, E}"/> que contiene un <see cref="Optional{StringQueryDTO}"/> si la conversión es exitosa,
    /// o un error <see cref="Unit"/> si la instancia es nula.
    /// </returns>
    public static Result<Optional<StringQueryDTO>, Unit> ToOptional(this StringQueryMAPI? source)
    {
        if (source is null)
            return Optional<StringQueryDTO>.None();

        var result = source?.ToDomain();
        if (result!.IsErr)
            return Unit.Value;

        var value = result.Unwrap();
        return Optional.Some(value);
    }

    /// <summary>
    /// Intenta convertir una instancia opcional de <see cref="StringQueryMAPI"/> en un <see cref="StringQueryDTO"/>
    /// y asignar el resultado o registrar un error en caso de fallo.
    /// </summary>
    /// <param name="value">Instancia opcional de <see cref="StringQueryMAPI"/> a analizar.</param>
    /// <param name="fieldName">Nombre del campo asociado a la consulta, usado para registrar errores.</param>
    /// <param name="resultRef">Referencia donde se asignará el resultado de la conversión si es válida.</param>
    /// <param name="errListRef">Lista de errores donde se agregará un <see cref="FieldErrorDTO"/> en caso de conversión inválida.</param>
    public static void ParseStringQuery(
        StringQueryMAPI? value,
        string fieldName,
        ref Optional<StringQueryDTO> resultRef,
        List<FieldErrorDTO> errListRef
    )
    {
        if (value is null)
        {
            resultRef = Optional<StringQueryDTO>.None();
            return;
        }

        var optParse = value.ToOptional();

        if (optParse.IsErr)
        {
            errListRef.Add(new() { Field = fieldName, Message = "Format invalid" });

            return;
        }

        resultRef = optParse.Unwrap();
    }

    /// <summary>
    /// Intenta convertir una instancia de <see cref="StringQueryMAPI"/> en un <see cref="StringQueryDTO"/>
    /// y asignar el resultado o registrar un error en caso de fallo.
    /// </summary>
    /// <param name="value">Instancia de <see cref="StringQueryMAPI"/> a analizar.</param>
    /// <param name="fieldName">Nombre del campo asociado a la consulta, usado para registrar errores.</param>
    /// <param name="resultRef">Referencia donde se asignará el resultado de la conversión si es válida.</param>
    /// <param name="errListRef">Lista de errores donde se agregará un <see cref="FieldErrorDTO"/> en caso de conversión inválida.</param>
    public static void ParseStringQuery(
        StringQueryMAPI value,
        string fieldName,
        ref StringQueryDTO resultRef,
        List<FieldErrorDTO> errListRef
    )
    {
        var optParse = value.ToDomain();

        if (optParse.IsErr)
        {
            errListRef.Add(new() { Field = fieldName, Message = "Format invalid" });
            return;
        }

        resultRef = optParse.Unwrap();
    }
}
