using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Common;

namespace EduZasAPI.Application.Users;

public class AddUserUseCase : AddUseCase<NewUserDTO, UserDomain>
{
    protected IHashService _hasher;
    protected IQuerierAsync<UserDomain, UserCriteriaDTO> _querier;

    public AddUserUseCase(
        IHashService hasher,
        ICreatorAsync<UserDomain, NewUserDTO> creator,
        IBusinessValidationService<NewUserDTO> validator,
        IQuerierAsync<UserDomain, UserCriteriaDTO> querier)
      : base(creator, validator)
    {
        _querier = querier;
        _hasher = hasher;
    }


    protected override async Task<Result<Unit, List<FieldErrorDTO>>> ExtraValidationAsync(NewUserDTO usr)
    {
        var errs = new List<FieldErrorDTO>();

        var emailSearch = new StringQueryDTO { Text = usr.Email, SearchType = StringSearchType.EQ };

        var search = await _querier.GetByAsync(new UserCriteriaDTO
        {
            Email = Optional.Some(emailSearch)
        });

        var results = search.Results.Count;

        if (results > 1)
            throw new InvalidDataException($"Repeated email {usr.Email} stored");

        if (results == 1)
        {
            var error = new FieldErrorDTO { Field = "email", Message = "Email ya registrado" };
            errs.Add(error);
            return Result.Err(errs);
        }

        return Result<Unit, List<FieldErrorDTO>>.Ok(Unit.Value);
    }

    protected override NewUserDTO PostValidationFormat(NewUserDTO u)
    {
        u.Password = _hasher.Hash(u.Password);
        return u;
    }
}
