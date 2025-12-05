using System.Net;
using System.Net.Http.Json;
using Application.DAOs;
using Application.DTOs.Users;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalAPITest;

/// <summary>
/// Clase de pruebas para los endpoints de autenticación (/api/v1/auth).
/// Utiliza la CustomWebApplicationFactory para ejecutar las pruebas contra
/// una versión en memoria de la API con una base de datos SQLite.
/// </summary>
public class AuthEndpointsTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkAndTokenCookie()
    {
        // Arrange: Preparamos la base de datos con un usuario de prueba.
        var password = "password123";
        var userDto = new NewUserDTO
        {
            FirstName = "Test",
            FatherLastname = "User",
            Email = "test.user@example.com",
            Password = password,
            Role = UserType.STUDENT
        };

        // Creamos un scope para resolver los servicios de DI correctamente
        using (var scope = _factory.Services.CreateScope())
        {
            var creator = scope.ServiceProvider.GetRequiredService<ICreatorAsync<UserDomain, NewUserDTO>>();
            await creator.AddAsync(userDto);
        }

        var loginCredentials = new UserCredentialsDTO
        {
            Email = userDto.Email,
            Password = userDto.Password
        };

        // Act: Realizamos la petición de login
        var response = await _client.PostAsJsonAsync("/auth/login", loginCredentials);

        // Assert: Verificamos los resultados
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var publicUser = await response.Content.ReadFromJsonAsync<PublicUserDTO>();
        publicUser.Should().NotBeNull();
        publicUser?.Email.Should().Be(userDto.Email);

        response.Headers.Should().ContainKey("Set-Cookie");
        response.Headers.GetValues("Set-Cookie").Should().Contain(c => c.StartsWith("AuthToken="));
    }
}
