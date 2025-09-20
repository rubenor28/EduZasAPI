/// <summary>
/// Representa un error de validación asociado a un campo específico.
/// </summary>
/// <remarks>
/// Esta estructura inmutable encapsula información sobre errores de validación
/// que ocurren en campos particulares, incluyendo el nombre del campo
/// y el mensaje de error descriptivo. No permite valores nulos o vacíos.
/// </remarks>
public readonly struct FieldError
{
    /// <summary>
    /// Obtiene el nombre del campo que generó el error.
    /// </summary>
    /// <value>Nombre del campo con error.</value>
    public string Field { get; }

    /// <summary>
    /// Obtiene el mensaje descriptivo del error.
    /// </summary>
    /// <value>Mensaje de error descriptivo.</value>
    public string Message { get; }

    /// <summary>
    /// Inicializa una nueva instancia de la estructura <see cref="FieldError"/>.
    /// </summary>
    /// <param name="field">Nombre del campo que generó el error.</param>
    /// <param name="message">Mensaje descriptivo del error.</param>
    /// <exception cref="ArgumentNullException">
    /// Se lanza cuando el campo o mensaje son nulos o vacíos.
    /// </exception>
    public FieldError(string field, string message)
    {
        if (string.IsNullOrEmpty(field)) throw new ArgumentNullException("Field can not be null", nameof(field));
        if (string.IsNullOrEmpty(message)) throw new ArgumentNullException("Message can not be null", nameof(message));

        Field = field;
        Message = message;
    }
}
