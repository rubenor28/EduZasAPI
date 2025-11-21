using Domain.ValueObjects;

namespace Application.DTOs.Contacts;

///<summary>
/// DTO que representa los campos obligatorios y opcionales para
/// la actualizacion de un contacto
///</summary>
public sealed record ContactUpdateDTO
{
    ///<summary>
    /// Id del dueño de la agenda
    ///</summary>
    public required ulong AgendaOwnerId { get; set; }

    ///<summary>
    /// Id del contacto
    ///</summary>
    public required ulong UserId { get; set; }

    ///<summary>
    /// Alias del contacto
    ///</summary>
    public required string Alias { get; set; }

    ///<summary>
    /// Notas cualquiera en formato de texto para el usuario
    ///</summary>
    public required Optional<string> Notes { get; set; }
}
