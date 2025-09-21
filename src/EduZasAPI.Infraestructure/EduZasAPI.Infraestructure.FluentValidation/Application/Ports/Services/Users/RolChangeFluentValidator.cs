namespace EduZasAPI.Infraestructure.Application.Ports.Services.Users;

using EduZasAPI.Application.DTOs.Users;
using EduZasAPI.Infraestructure.Application.Ports.Services.Common;

using FluentValidation;

public class RolChangeFluentValidator : FluentValidator<RolChangeDTO> {
  public RolChangeFluentValidator() {
    RuleFor(x=>x.Id).GreaterThanOrEqualTo((uint)1).WithMessage("Formato inválido");
  }
}
