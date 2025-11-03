using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Contacts;

///<summary>
/// DTO que representa los campos obligatorios y opcionales para 
/// la creacion de un contacto
///</summary>
public sealed record NewContactDTO
{
    ///<summary>
    /// Alias del contacto
    ///</summary>
    public required string Alias { get; set; }

    ///<summary>
    /// Notas cualquiera en formato de texto para el usuario
    ///</summary>
    public Optional<string> Notes { get; set; } = Optional<string>.None();

    ///<summary>
    /// ID usuario del dueño de la agenda
    ///</summary>
    public required ulong AgendaOwnerId { get; set; }

    ///<summary>
    /// ID usuario del dueño del contacto registrado
    ///</summary>
    public required ulong ContactId { get; set; }

    ///<summary>
    /// Etiquetas del usuario desde la creacion
    ///</summary>
    public required Optional<IEnumerable<string>> Tags { get; set; } =
        Optional<IEnumerable<string>>.None();

    ///<summary>
    /// Usuario que ejecuta la accion de creación
    ///</summary>
    public required Executor Executor { get; set; }
}
