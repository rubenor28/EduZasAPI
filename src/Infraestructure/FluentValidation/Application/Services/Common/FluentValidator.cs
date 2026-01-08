using Application.DTOs.Common;
using Application.Services.Validators;
using Domain.ValueObjects;
using FluentValidation;

namespace FluentValidationProj.Application.Services.Common;

/// <summary>
/// Clase base para validadores FluentValidation.
/// </summary>
/// <typeparam name="T">Tipo a validar.</typeparam>
public abstract class FluentValidator<T> : AbstractValidator<T>, IBusinessValidationService<T>
{
    /// <summary>
    /// Ejecuta la validación.
    /// </summary>
    /// <param name="data">Objeto a validar.</param>
    /// <returns>Unit si es válido, lista de errores si no.</returns>
    public Result<Unit, IEnumerable<FieldErrorDTO>> IsValid(T data)
    {
        var validation = Validate(data);

        if (validation.IsValid)
            return Unit.Value;

        var errors = validation
            .Errors.Select(error =>
            {
                var field = char.ToLower(error.PropertyName[0]) + error.PropertyName[1..];
                return new FieldErrorDTO { Field = field, Message = error.ErrorMessage };
            })
            .ToList();

        return errors;
    }
}
