namespace EduZasAPI.Infraestructure.Application.Ports.DAOs;

using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using EduZasAPI.Domain.Entities;
using EduZasAPI.Domain.ValueObjects.Common;

using EduZasAPI.Application.Ports.DAOs;
using EduZasAPI.Application.DTOs.Users;

using EduZasAPI.Infraestructure.Application.DTOs;

public class UserEntityFrameworkRepositoryTest
{
    private IUserRepositoryAsync _repo;
    private EduZasDotnetContext _ctx;

    private NewUserDTO _defaultNew = new NewUserDTO
    {
        Email = "ruben.roman@test.com",
        Password = "test",
        FirstName = "Ruben",
        FatherLastName = "Roman",
    };

    private UserDomain _defaultUser = new UserDomain
    {
        Id = 1,
        Email = "ruben.roman@test.com",
        Password = "test",
        FirstName = "Ruben",
        FatherLastName = "Roman",
        CreatedAt = DateTime.Now,
        ModifiedAt = DateTime.Now,
    };


    public UserEntityFrameworkRepositoryTest()
    {
        var config = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .Build();

        var connStr = config.GetConnectionString("TestConnection");

        var services = new ServiceCollection();
        services.AddTransient<IUserRepositoryAsync>(provider =>
        {
            var context = provider.GetRequiredService<EduZasDotnetContext>();
            ulong pageSize = 10;
            return new UserEntityFrameworkRepository(context, pageSize);
        });

        services.AddDbContext<EduZasDotnetContext>(options =>
            options.UseMySql(connStr, Microsoft.EntityFrameworkCore.ServerVersion.Parse("12.0.2-mariadb")));

        var serviceProvider = services.BuildServiceProvider();

        _repo = serviceProvider.GetRequiredService<IUserRepositoryAsync>();
        _ctx = serviceProvider.GetRequiredService<EduZasDotnetContext>();
    }

    private async Task DeleteAndInitializeDatabaseAsync()
    {
        await _ctx.Database.EnsureDeletedAsync();
        await _ctx.Database.EnsureCreatedAsync();
    }

    [Fact]
    public async Task Add_Success()
    {
        await DeleteAndInitializeDatabaseAsync();

        var dto = _defaultNew;
        var record = await _repo.AddAsync(dto);

        Assert.NotNull(record);
        Assert.Equal(1ul, record.Id);
        Assert.Equal(dto.Email, record.Email);
        Assert.Equal(dto.FirstName, record.FirstName);
    }

    [Fact]
    public async Task Delete_Success()
    {
        await DeleteAndInitializeDatabaseAsync();
        await _repo.AddAsync(_defaultNew);
        var record = await _repo.DeleteAsync(1);

        Assert.True(record.IsSome);
        Assert.Equal(1ul, record.Unwrap().Id);
        Assert.Equal(_defaultNew.Email, record.Unwrap().Email);
    }

    [Fact]
    public async Task Get_Success()
    {
        await DeleteAndInitializeDatabaseAsync();
        await _repo.AddAsync(_defaultNew);
        var record = await _repo.GetAsync(1);

        Assert.True(record.IsSome);
        Assert.Equal(1ul, record.Unwrap().Id);
        Assert.Equal(_defaultNew.Email, record.Unwrap().Email);
    }

    [Fact]
    public async Task Update_success()
    {
        await DeleteAndInitializeDatabaseAsync();
        var user = await _repo.AddAsync(_defaultNew);

        var dto = new UserUpdateDTO
        {
            Email = user.Email,
            Password = user.Password,
            FirstName = user.FirstName,
            FatherLastName = user.FatherLastName,
            Id = user.Id,

            Active = false,
            MidName = Optional<string>.Some("Juan"),
            MotherLastname = Optional<string>.Some("Juanes"),
        };

        var updated = await _repo.UpdateAsync(dto);

        Assert.NotNull(updated);
        Assert.False(updated.Active);
        Assert.True(updated.MidName.IsSome);
        Assert.True(updated.MotherLastname.IsSome);
        Assert.Equal("Juan", updated.MidName.UnwrapOr(""));
        Assert.Equal("Juanes", updated.MotherLastname.UnwrapOr(""));
    }
}
