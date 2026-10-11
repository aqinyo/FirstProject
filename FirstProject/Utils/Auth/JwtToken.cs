using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FirstProject.Model.Entity;
using Microsoft.IdentityModel.Tokens;

namespace FirstProject.Utils.Auth
{
    public class JwtToken
    {
        private readonly IConfiguration _configuration;

        public JwtToken(IConfiguration configuration) => _configuration = configuration;

        public string CreateToken(User user)
        {
            var jwt = _configuration.GetSection("Jwt");
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims:
                [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Account),
                new Claim(ClaimTypes.Role, user.Role)
                ],
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}