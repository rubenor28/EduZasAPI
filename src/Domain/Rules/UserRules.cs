namespace Domain.Rules;

/// <summary>
/// Contiene constantes de expresiones regulares para la validación de datos de usuario.
/// </summary>
public static class UserRules
{
    /// <summary>
    /// Valida nombres simples, requiriendo un mínimo de 3 letras mayúsculas.
    /// </summary>
    public static readonly string SimpleName = @"^[A-ZÁÉÍÓÚÜÑ]{3,}$";

    /// <summary>
    /// Valida nombres compuestos que pueden incluir prefijos comunes (ej. "DE LA", "DEL").
    /// </summary>
    public static readonly string CompositeName =
        @"^(?:DE\s(?:LA|LAS|LOS|EL)|DEL|DE|LA|LAS|LOS|EL|AL)\s[A-ZÁÉÍÓÚÜÑ]{3,}$";

    /// <summary>
    /// Valida matrículas o claves de estudiante con un formato específico.
    /// </summary>
    public static readonly string Tuition = @"^[A-ZÁÉÍÓÚÜÑ]{3}[OIPV]\d{6}$";

    /// <summary>
    /// Valida contraseñas seguras: mínimo 8 caracteres, una mayúscula, una minúscula y un símbolo.
    /// </summary>
    public static readonly string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^A-Za-z0-9]).{8,}$";
}
