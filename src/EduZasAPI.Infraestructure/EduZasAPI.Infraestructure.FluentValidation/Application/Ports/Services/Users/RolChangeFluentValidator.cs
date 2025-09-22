namespace EduZasAPI.Infraestructure.Application.Ports.Services.Users;

using EduZasAPI.Application.DTOs.Users;
using EduZasAPI.Infraestructure.Application.Ports.Services.Common;

using FluentValidation;

/// <summary>
/// Validador para el cambio de rol de usuario,
/// aplicando reglas específicas sobre el identificador del rol.
/// </summary>
public class RolChangeFluentValidator : FluentValidator<RolChangeDTO>
{
    /// <summary>
    /// Inicializa las reglas de validación para <see cref="RolChangeDTO"/>.
    /// </summary>
    public RolChangeFluentValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThanOrEqualTo((uint)1)
            .WithMessage("Formato inválido");
    }
}
