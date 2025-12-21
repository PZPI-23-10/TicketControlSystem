using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Ticket_control_system.DTO;
using Ticket_control_system.Interfaces;
namespace Ticket_control_system.Services;
[Serializable]
public class JwtSettings
{
    public string AccessSecretKey { get; set; }
    public int DefaultAccessExpireTimeInMinutes { get; set; }
    public int RememberAccessExpireTimeInDays { get; set; }
}

public class TokenService(JwtSettings jwtSettings) : ITokenService
{
    public Token GenerateAccessToken(string id, string email, IList<string> roles, bool rememberMe)
    {
        var claims = new List<Claim> 
        { 
            new(ClaimTypes.NameIdentifier, id), 
            new(ClaimTypes.Name, email) 
        };

        // Добавляем каждую роль как отдельный Claim
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expirationTime = rememberMe
            ? DateTime.UtcNow.AddDays(jwtSettings.RememberAccessExpireTimeInDays)
            : DateTime.UtcNow.AddMinutes(jwtSettings.DefaultAccessExpireTimeInMinutes);

        return GenerateToken(jwtSettings.AccessSecretKey, expirationTime, claims);
    }
    
    public DateTime GetTokenExpirationTime(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var decodedToken = tokenHandler.ReadToken(token);
        return decodedToken.ValidTo;
    }
    
    private Token GenerateToken(string secretKey, DateTime expirationTime, List<Claim> claims)
    {
        var jwtToken = new JwtSecurityToken(
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expirationTime,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)
                ),
                SecurityAlgorithms.HmacSha256Signature));

        return new Token { TokenKey = new JwtSecurityTokenHandler().WriteToken(jwtToken), Expires = expirationTime };
    }
}