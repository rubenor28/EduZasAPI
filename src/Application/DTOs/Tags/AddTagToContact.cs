namespace Application.DTOs.Tags;

public sealed record AddTagToContact {
  /// <summary>
  /// Etiqueta a agregar al contacto
  /// </summary>
  public required string Tag {get;set;}

  /// <summary>
  /// ID de usuario del responsable de la agenda
  /// </summary>
  public required ulong AgendaOwnerId {get;set;}

  /// <summary>
  /// ID de usuario del contacto
  /// </summary>
  public required ulong UserId {get;set;}
}
