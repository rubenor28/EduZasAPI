using Application.DTOs.Common;
using Domain.Entities;

namespace Application.DTOs.Contacts;


///<summary>
/// DTO que representa los campos obligatorios para la 
/// eliminacion de un contacto 
///</summary>
///<remarks>
///Implementa <see cref="IIdentifiable{T}"> para la obtencion del
///ID del registro
///</remarks>
public sealed record DeleteContactDTO
{

    /// <summary>
    /// Obtiene o establece el identificador único.
    /// </summary>
    /// <value>Identificador numérico del contacto. Campo obligatorio.</value>
    public required ContactIdDTO Id { get; set; }

    ///<summary>
    /// Usuario que ejecuta la accion
    ///</summary>
    public required Executor Executor { get; set; }
}
