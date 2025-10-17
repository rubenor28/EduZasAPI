using Application.DTOs.Common;
using Application.Services;
using Domain.ValueObjects;
using FluentValidation;

namespace FluentValidationProj.Application.Services.Common;

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
    public Result<Unit, IEnumerable<FieldErrorDTO>> IsValid(T data)
    {
        var validation = Validate(data);

        if (validation.IsValid)
            return Unit.Value;

        var errors = validation
            .Errors.Select(error =>
            {
                var field = char.ToLower(error.PropertyName[0]) + error.PropertyName.Substring(1);
                return new FieldErrorDTO { Field = field, Message = error.ErrorMessage };
            })
            .ToList();

        return errors;
    }
}
