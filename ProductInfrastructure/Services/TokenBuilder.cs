using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ProductCore.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductInfrastructure.Services
{
    public class TokenBuilder(IConfiguration configuration, ILogger<TokenBuilder> logger) : ITokenBuilder
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<TokenBuilder> _logger = logger;

        /// <summary>
        /// Generate Access Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GenerateAccessToken(string username)
        {
            try
            {
                string jwtKey = _configuration["Jwt:Key"] ?? "";
                _logger.LogInformation($"Generating JWT Access Token for user: {username}");
                SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(jwtKey));
                SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

                Claim[] claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, username),
            };

                JwtSecurityToken token = new(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30), // Token expiration time
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating access token for user: {username}");
                throw;
            }
        }
    }
}
