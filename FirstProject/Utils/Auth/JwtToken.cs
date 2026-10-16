using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FirstProject.Model.Entity;
using Microsoft.IdentityModel.Tokens;

namespace FirstProject.Utils.Auth
{
    /// <summary>
    /// JwtToken的封装工具类：主要用于生成Jwt
    /// </summary>
    public class JwtToken
    {

        private readonly IConfiguration _configuration;


        /// <summary>
        /// Jwt生成Token方法
        /// </summary>
        /// <param name="configuration"></param>
        public JwtToken(IConfiguration configuration) => _configuration = configuration;

        public string CreateToken(User user, string jti, DateTime expiresAt)
        {
            var jwt = _configuration.GetSection("Jwt");
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
                SecurityAlgorithms.HmacSha256);

            // 签发的 token 包含下面new的四个成分
            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims:
                [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Account),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, jti) // 新增Redis的jti
                ],
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}