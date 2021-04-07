using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Audiochan.Infrastructure.Security
{
    public class TokenProvider : ITokenProvider
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly UserManager<User> _userManager;

        public TokenProvider(IOptions<JwtSettings> jwtOptions, UserManager<User> userManager,
            IDateTimeProvider dateTimeProvider)
        {
            _jwtSettings = jwtOptions.Value;
            _userManager = userManager;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<(string, long)> GenerateAccessToken(User user)
        {
            var claims = await GetClaims(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

            var createdDate = _dateTimeProvider.Now;
            var expirationDate = createdDate.Add(_jwtSettings.AccessTokenExpiration);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationDate,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return (tokenHandler.WriteToken(token), DateTimeToUnixEpoch(expirationDate));
        }

        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new("name", user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim("role", role)));

            return claims;
        }

        public RefreshToken GenerateRefreshToken(string userId)
        {
            using var rng = new RNGCryptoServiceProvider();
            var randomBytes = new byte[64];
            rng.GetBytes(randomBytes);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expiry = _dateTimeProvider.Now.Add(_jwtSettings.RefreshTokenExpiration),
                Created = _dateTimeProvider.Now,
                UserId = userId
            };
        }

        public long DateTimeToUnixEpoch(DateTime dateTime)
        {
            return EpochTime.GetIntDate(dateTime.ToUniversalTime());
        }

        public bool IsRefreshTokenValid(RefreshToken existingToken)
        {
            if (existingToken.Revoked != null)
            {
                return false;
            }

            if (existingToken.Expiry <= _dateTimeProvider.Now)
            {
                return false;
            }

            return true;
        }
    }
}