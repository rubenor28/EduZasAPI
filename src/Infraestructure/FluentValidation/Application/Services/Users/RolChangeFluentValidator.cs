using Application.DTOs.Users;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Users;

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
        RuleFor(x => x.Id).Must(x => x >= 1).WithMessage("Formato inválido");
    }
}
