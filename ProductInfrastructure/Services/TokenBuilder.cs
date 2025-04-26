using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ProductCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            var jwtKey = _configuration["Jwt:Key"] ?? "";
            _logger.LogInformation($"JWT Generation Key: {jwtKey}");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, username),
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), // Token expiration time
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
