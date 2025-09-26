using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

public class JwtService
{
    private JwtSettings _jwtSettings;

    public JwtService(JwtSettings settings)
    {
        _jwtSettings = settings;
    }

    public string Generate(AuthPayload payload)
    {
        var claims = new List<Claim>
        {
          new Claim(JwtRegisteredClaimNames.Sub, payload.Id.ToString()),
          new Claim(ClaimTypes.Role, payload.Role.ToString()),
        };

        return "token";
    }
}
