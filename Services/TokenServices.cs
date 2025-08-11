﻿using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using minutes90.Entities;
using minutes90.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace minutes90.Services
{
    public class TokenServices(IConfiguration configuration, UserManager<AppUsers> manager)
        : ITokenServices
    {
        public async Task<string?> CreateToken(AppUsers user)
        {
            // Get and validate token key from configuration
            string tokenKey = configuration["TokenKey"] ?? throw new InvalidOperationException("TokenKey is missing");

            if (tokenKey.Length < 64)
                throw new InvalidOperationException("TokenKey must be at least 64 characters long");

            // Create signing key
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(tokenKey));

            // Validate name
            if (user.UserName is null)
            {
                throw new ArgumentNullException(nameof(user.UserName), "UserName cannot be null");
            }


            // Create claims for the token
            List<Claim> claims =
            [
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName)
            ];

            // Add role claims
            var roles = await manager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Create signing credentials
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512Signature);

            // Configure token descriptor
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(3),
                SigningCredentials = credentials
            };

            // Create and return the token
            JwtSecurityTokenHandler tokenHandler = new();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
