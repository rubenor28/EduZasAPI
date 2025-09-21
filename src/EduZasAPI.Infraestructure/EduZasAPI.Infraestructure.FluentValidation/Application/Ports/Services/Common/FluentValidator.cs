namespace EduZasAPI.Infraestructure.Application.Ports.Services.Common;

using EduZasAPI.Domain.ValueObjects.Common;
using EduZasAPI.Application.DTOs.Common;
using EduZasAPI.Application.Ports.Services.Common;
using FluentValidation;

/// <summary>
/// Clase base abstracta para validadores que usan FluentValidation
/// e implementan el contrato <see cref="IBusinessValidationService{T}"/>.
/// </summary>
/// <typeparam name="T">Tipo de objeto a validar.</typeparam>
public abstract class FluentValidator<T> : AbstractValidator<T>, IBusinessValidationService<T>
{
    /// <summary>
    /// Ejecuta la validación sobre una instancia del tipo <typeparamref name="T"/>.
    /// </summary>
    /// <param name="data">Instancia a validar.</param>
    /// <returns>
    /// Resultado exitoso si el objeto es válido,
    /// de lo contrario un resultado con la lista de errores de campo.
    /// </returns>
    public Result<Unit, List<FieldErrorDTO>> IsValid(T data)
    {
        var validation = Validate(data);

        if (validation.IsValid)
            return Result<Unit, List<FieldErrorDTO>>.Ok(Unit.Value);

        var errors = validation.Errors.Select(error => new FieldErrorDTO
        {
            Field = error.PropertyName,
            Message = error.ErrorMessage,
        }).ToList();

        return Result<Unit, List<FieldErrorDTO>>.Err(errors);
    }
}
