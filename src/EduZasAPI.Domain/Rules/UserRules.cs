namespace EduZasAPI.Domain.Users;

/// <summary>
/// Contiene expresiones regulares utilizadas para validar propiedades de usuarios.
/// </summary>
public static class UserRegexs
{
    /// <summary>
    /// Expresión regular para validar nombres simples (mínimo 3 letras mayúsculas).
    /// </summary>
    public static readonly string SimpleName = @"^[A-ZÁÉÍÓÚÜÑ]{3,}$";

    /// <summary>
    /// Expresión regular para validar nombres compuestos con prefijos opcionales.
    /// </summary>
    public static readonly string CompositeName = @"^(?:DE\s(?:LA|LAS|LOS|EL)|DEL|DE|LA|LAS|LOS|EL|AL)\s[A-ZÁÉÍÓÚÜÑ]{3,}$";

    /// <summary>
    /// Expresión regular para validar matrículas o claves de estudiante.
    /// </summary>
    public static readonly string Tuition = @"^[A-ZÁÉÍÓÚÜÑ]{3}[OIPV]\d{6}$";

    /// <summary>
    /// Expresión regular para validar contraseñas (mínimo 8 caracteres, al menos una mayúscula, una minúscula y un carácter especial).
    /// </summary>
    public static readonly string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^A-Za-z0-9]).{8,}$";
}
