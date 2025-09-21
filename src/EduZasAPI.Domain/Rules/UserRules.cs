namespace EduZasAPI.Domain.Rules;

public static class UserRegexs
{
    public static readonly string SimpleName = @"^[A-ZÁÉÍÓÚÜÑ]{3,}$";
    public static readonly string CompositeName = @"^(?:DE\s(?:LA|LAS|LOS|EL)|DEL|DE|LA|LAS|LOS|EL|AL)\s[A-ZÁÉÍÓÚÜÑ]{3,}$";
    public static readonly string Tuition = @"^[A-ZÁÉÍÓÚÜÑ]{3}[OIPV]\d{6}$";
    public static readonly string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^A-Za-z0-9]).{8,}$";
}
