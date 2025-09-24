using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;

using EduZasAPI.Application.Common;
using EduZasAPI.Application.Users;

using EduZasAPI.Infraestructure.EntityFramework.Application.Common;
using EduZasAPI.Infraestructure.EntityFramework.Application.Users;

namespace EduZasAPI.Test.Infraestructure.Application.Ports.DAOs;

public class UserEntityFrameworkRepositoryTest
{
    private IUserRepositoryAsync _repo;
    private EduZasDotnetContext _ctx;

    public UserEntityFrameworkRepositoryTest()
    {
        var config = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .Build();

        var connStr = config.GetConnectionString("TestConnection");

        var s = new ServiceCollection();
        s.AddTransient<IUserRepositoryAsync>(prv =>
        {
            var ctx = prv.GetRequiredService<EduZasDotnetContext>();
            ulong pageSize = 10;
            return new UserEntityFrameworkRepository(ctx, pageSize);
        });

        s.AddDbContext<EduZasDotnetContext>(opts =>
            opts.UseMySql(connStr, Microsoft.EntityFrameworkCore.ServerVersion.Parse("12.0.2-mariadb")));

        var sp = s.BuildServiceProvider();

        _repo = sp.GetRequiredService<IUserRepositoryAsync>();
        _ctx = sp.GetRequiredService<EduZasDotnetContext>();
    }

    private async Task DeleteAndInitializeDatabaseAsync()
    {
        await _ctx.Database.EnsureDeletedAsync();
        await _ctx.Database.EnsureCreatedAsync();
    }

    private static string CreateSafeEmail(string hint = "")
    {
        var id = (hint + Guid.NewGuid().ToString("N")).Replace("-", "");
        var local = "u" + id.Substring(0, Math.Min(8, id.Length)).ToLowerInvariant();
        return $"{local}@t.test";
    }

    private NewUserDTO CreateNew(
        string? email = null,
        string? password = null,
        string? firstName = null,
        string? fatherLastname = null,
        Optional<string>? midName = null,
        Optional<string>? motherLastname = null) => new NewUserDTO
        {
            Email = email ?? "test@test.com",
            Password = password ?? "test",
            FirstName = firstName ?? "Juan",
            FatherLastName = fatherLastname ?? "Lopez",
            MidName = midName ?? Optional<string>.Some("Carlos"),
            MotherLastname = motherLastname ?? Optional<string>.Some("Perez")
        };

    private UserUpdateDTO CreateUpdate(UserDomain u) => new UserUpdateDTO
    {
        Id = u.Id,
        Active = u.Active,
        Email = u.Email,
        Password = u.Password,
        FirstName = u.FirstName,
        FatherLastName = u.FatherLastName,
        MidName = u.MidName,
        MotherLastname = u.MotherLastname
    };

    [Fact]
    public async Task Add_Success()
    {
        await DeleteAndInitializeDatabaseAsync();

        var dto = CreateNew();
        var record = await _repo.AddAsync(dto);

        Assert.NotNull(record);
        Assert.Equal(1ul, record.Id);
        Assert.Equal(dto.Email, record.Email);
        Assert.Equal(dto.FirstName, record.FirstName);
    }

    [Fact]
    public async Task Add_RepeatedEmail()
    {
        await DeleteAndInitializeDatabaseAsync();

        var dto = CreateNew();
        var dto2 = CreateNew(
            firstName: "A",
            midName: "B".ToOptional(),
            fatherLastname: "C",
            motherLastname: "D".ToOptional(),
            password: "daljda");

        var record = await _repo.AddAsync(dto);

        try
        {
            var result = await _repo.AddAsync(dto);
            Assert.Fail("No deberia agregar un elemento");
        }
        catch (System.Exception)
        {
        }
    }

    [Fact]
    public async Task Delete_Success()
    {
        await DeleteAndInitializeDatabaseAsync();

        var dto = CreateNew();
        await _repo.AddAsync(dto);
        var record = await _repo.DeleteAsync(1);

        Assert.True(record.IsSome);
        Assert.Equal(1ul, record.Unwrap().Id);
        Assert.Equal(dto.Email, record.Unwrap().Email);
    }

    [Fact]
    public async Task Get_Success()
    {
        await DeleteAndInitializeDatabaseAsync();

        var dto = CreateNew();

        await _repo.AddAsync(dto);
        var record = await _repo.GetAsync(1);

        Assert.True(record.IsSome);
        Assert.Equal(1ul, record.Unwrap().Id);
        Assert.Equal(dto.Email, record.Unwrap().Email);
    }

    [Fact]
    public async Task Update_success()
    {
        await DeleteAndInitializeDatabaseAsync();

        var user = await _repo.AddAsync(CreateNew());

        user.Active = false;
        user.MidName = "Alfredo".ToOptional();
        user.MotherLastname = "AQuitame".ToOptional();

        var updated = await _repo.UpdateAsync(CreateUpdate(user));

        Assert.NotNull(updated);
        Assert.True(updated.MidName.IsSome);
        Assert.True(updated.MotherLastname.IsSome);
        Assert.Equal("Alfredo", updated.MidName.UnwrapOr(""));
        Assert.Equal("AQuitame", updated.MotherLastname.UnwrapOr(""));
        Assert.False(updated.Active);
    }

    private async Task SeedUsersAsync()
    {
        var u1 = CreateNew(
            email: CreateSafeEmail("a1"),
            firstName: "ALICE",
            fatherLastname: "SMITH"
        );

        var u2 = CreateNew(
            email: CreateSafeEmail("a2"),
            firstName: "ALICIA",
            fatherLastname: "GOMEZ"
        );

        var u3 = CreateNew(
            email: CreateSafeEmail("a3"),
            firstName: "BOB",
            fatherLastname: "SMITH"
        );

        var u4 = CreateNew(
            email: CreateSafeEmail("a4"),
            firstName: "CHARLIE",
            fatherLastname: "JOHNSON"
        );

        await _repo.AddAsync(u1);
        await _repo.AddAsync(u2);
        await _repo.AddAsync(u3);
        await _repo.AddAsync(u4);
    }

    public static IEnumerable<object?[]> SearchCases()
    {
        yield return new object?[]
        {
                new UserCriteriaDTO(), // todos None
                4,
        };

        // 2) Active = true => 3
        yield return new object?[]
        {
                new UserCriteriaDTO { Active = Optional<bool>.Some(true) },
                3,
        };

        // 3) FirstName EQ "ALICE" => 1
        yield return new object?[]
        {
                new UserCriteriaDTO
                {
                    FirstName = Optional<StringQueryDTO>.Some(new StringQueryDTO
                    {
                        SearchType = StringSearchType.EQ,
                        Text = "ALICE"
                    })
                },
                1,
        };

        // 4) FirstName LIKE "ALI" => 2 (ALICE + ALICIA)
        yield return new object?[]
        {
                new UserCriteriaDTO
                {
                    FirstName = Optional<StringQueryDTO>.Some(new StringQueryDTO
                    {
                        SearchType = StringSearchType.LIKE,
                        Text = "ALI"
                    })
                },
                2,
        };

        // 5) Active true + FirstName LIKE "ALI" => 2
        yield return new object?[]
        {
                new UserCriteriaDTO
                {
                    Active = Optional<bool>.Some(true),
                    FirstName = Optional<StringQueryDTO>.Some(new StringQueryDTO
                    {
                        SearchType = StringSearchType.LIKE,
                        Text = "ALI"
                    })
                },
                2,
        };

        // 6) Non-matching criteria => 0
        yield return new object?[]
        {
                new UserCriteriaDTO
                {
                    FirstName = Optional<StringQueryDTO>.Some(new StringQueryDTO
                    {
                        SearchType = StringSearchType.EQ,
                        Text = "NONEXISTENT"
                    })
                },
                0,
        };
    }

    [Theory]
    [MemberData(nameof(SearchCases))]
    public async Task Search_ByCriteria_ReturnsExpected(UserCriteriaDTO criteria, int expectedCount)
    {
        var page = await _repo.GetByAsync(criteria);
        var actual = page.Results.Count;

        Assert.Equal(expectedCount, actual);
    }
}
