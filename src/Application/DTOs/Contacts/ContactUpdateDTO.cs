using Domain.Entities;
using Domain.ValueObjects;

namespace Application.DTOs.Contacts;

///<summary>
/// DTO que representa los campos obligatorios y opcionales para 
/// la actualizacion de un contacto
///</summary>
public sealed record ContactUpdateDTO : IIdentifiable<ContactIdDTO>
{
    /// <summary>
    /// Obtiene o establece el identificador único.
    /// </summary>
    /// <value>Identificador numérico del contacto. Campo obligatorio.</value>
    public required ContactIdDTO Id { get; set; }

    ///<summary>
    /// Alias del contacto
    ///</summary>
    public required string Alias { get; set; }

    ///<summary>
    /// Notas cualquiera en formato de texto para el usuario
    ///</summary>
    public required Optional<string> Notes { get; set; }
}
