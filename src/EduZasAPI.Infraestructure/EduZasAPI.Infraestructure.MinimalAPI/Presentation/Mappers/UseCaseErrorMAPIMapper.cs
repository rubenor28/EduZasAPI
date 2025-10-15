using EduZasAPI.Application.Common;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

namespace EduZasAPI.Infraestructure.MinimalAPI.Presentation.Common;

/// <summary>
/// Proporciona métodos de extensión para mapear errores de casos de uso a resultados de HTTP (IResult) en Minimal API.
/// </summary>
public static class UseCaseErrorMAPIMapper
{
    /// <summary>
    /// Convierte un error de caso de uso (UseCaseErrorImpl) en una respuesta HTTP estándar (IResult).
    /// </summary>
    /// <param name="err">El error de caso de uso a convertir.</param>
    /// <returns>Un objeto IResult que representa la respuesta HTTP correspondiente al tipo de error.</returns>
    public static IResult FromDomain(this UseCaseErrorImpl err)
    {
        return err switch
        {
            InputError errs => Results.BadRequest(new FieldErrorResponse
            {
                Message = "Formato inválido",
                Errors = errs.Errors
            }),
            UnauthorizedError => Results.Forbid(),
            NotFoundError => Results.NotFound(),
            _ => throw new InvalidOperationException()
        };
    }
}
