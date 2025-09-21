namespace EduZasAPI.Application.UseCases.Users;

using EduZasAPI.Domain.Entities;
using EduZasAPI.Domain.ValueObjects.Common;
using EduZasAPI.Application.Ports.DAOs;
using EduZasAPI.Application.Ports.Services.Common;
using EduZasAPI.Application.DTOs.Common;
using EduZasAPI.Application.DTOs.Users;
using EduZasAPI.Application.UseCases.Common;

public abstract class AddUseCase<T, U> : IUseCaseAsync<T,U, List<FieldErrorDTO>> 
where T : notnull
where U : notnull
{

    private readonly IUserRepositoryAsync _repository;
    private readonly IBusinessValidationService<NewUserDTO> _validator;

    public AddUseCase(IUserRepositoryAsync repository, IBusinessValidationService<NewUserDTO> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<Result<U, List<FieldErrorDTO>>> ExecuteAsync(T request) {

    }
}

public class AddUserUseCase : IUseCaseAsync<NewUserDTO, User, List<FieldErrorDTO>>
{
    private readonly IUserRepositoryAsync _repository;
    private readonly IBusinessValidationService<NewUserDTO> _validator;

    public AddUserUseCase(IUserRepositoryAsync repository, IBusinessValidationService<NewUserDTO> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<Result<User, List<FieldErrorDTO>>> ExecuteAsync(NewUserDTO request)
    {
        var validation = _validator.IsValid(request);

        if (validation.IsErr)
        {
            var errors = validation.UnwrapErr();
            return Result<User, List<FieldErrorDTO>>.Err(errors);
        }

        var newUser = await _repository.AddAsync(request);
        return Result<User, List<FieldErrorDTO>>.Ok(newUser);
    }
}
