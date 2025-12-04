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
    public required string Alias { get; init; }

    ///<summary>
    /// Notas cualquiera en formato de texto para el usuario
    ///</summary>
    public string? Notes { get; init; }

    ///<summary>
    /// ID usuario del dueño de la agenda
    ///</summary>
    public required ulong AgendaOwnerId { get; init; }

    ///<summary>
    /// ID usuario del dueño del contacto registrado
    ///</summary>
    public required ulong UserId { get; init; }

    ///<summary>
    /// Etiquetas del usuario desde la creacion
    ///</summary>
    public IEnumerable<string>? Tags { get; init; }
}
