using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Thiskord_Back.Tests.Helpers
{
    public static class TestJwtHelper
    {
        public static string GenerateToken(int userId, string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(TestWebAppFactory.TestJwtKey)
            );
            var token = new JwtSecurityToken(
                issuer: TestWebAppFactory.TestJwtIssuer,             
                audience: TestWebAppFactory.TestJwtAudience, 
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}