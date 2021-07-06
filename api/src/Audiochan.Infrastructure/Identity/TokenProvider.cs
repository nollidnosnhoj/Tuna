using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Audiochan.Infrastructure.Identity
{
    internal class TokenProvider : ITokenProvider
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;

        public TokenProvider(IOptions<JwtSettings> jwtOptions, 
            IDateTimeProvider dateTimeProvider, 
            TokenValidationParameters tokenValidationParameters, 
            IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _jwtSettings = jwtOptions.Value;
            _dateTimeProvider = dateTimeProvider;
            _tokenValidationParameters = tokenValidationParameters;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<(string, long)> GenerateAccessToken(User user)
        {
            var claims = await GetClaims(user);
            var expirationDate = _dateTimeProvider.Now.Add(_jwtSettings.AccessTokenExpiration);
            return GenerateToken(_jwtSettings.AccessTokenSecret, new ClaimsIdentity(claims), expirationDate);
        }

        public async Task<(string, long)> GenerateRefreshToken(User user, string tokenToBeRemoved = "")
        {
            var now = _dateTimeProvider.Now;
            var claims = await GetClaims(user);
            var expirationDate = now.Add(_jwtSettings.RefreshTokenExpiration);
            var (token, expirationDateEpoch) = GenerateToken(_jwtSettings.RefreshTokenSecret,
                new ClaimsIdentity(claims), expirationDate);
            var refreshToken = new RefreshToken
            {
                Token = token,
                Expiry = expirationDate,
                Created = now,
                UserId = user.Id
            };
            user.RefreshTokens.Add(refreshToken);

            if (!string.IsNullOrWhiteSpace(tokenToBeRemoved))
            {
                var oldRefreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == tokenToBeRemoved);
                if (oldRefreshToken != null)
                {
                    user.RefreshTokens.Remove(oldRefreshToken);
                }
            }
            
            // Update using DbContext's SaveChanges because using UserManager.UpdateAsync() would update in a
            // disconnected scenario, meaning it will update field, when it doesn't need to.
            await _unitOfWork.SaveChangesAsync();
            return (token, expirationDateEpoch);
        }

        public async Task<bool> ValidateRefreshToken(string token)
        {
            return await ValidateToken(token, _jwtSettings.RefreshTokenSecret);
        }

        private async Task<bool> ValidateToken(string token, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParams = _tokenValidationParameters.Clone();
            tokenValidationParams.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            try
            {
                tokenHandler.ValidateToken(
                    token,
                    tokenValidationParams,
                    out var validatedToken);
                var jwtSecurityToken = (JwtSecurityToken) validatedToken;
                var userId = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
                return await _unitOfWork.Users.ExistsAsync(u => u.Id == userId);
            }
            catch
            {
                return false;
            }
        }

        private static (string, long) GenerateToken(string secret, ClaimsIdentity claimsIdentity,
            DateTime expirationDate)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = expirationDate,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return (tokenHandler.WriteToken(token), EpochTime.GetIntDate(expirationDate.ToUniversalTime()));
        }

        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new("name", user.UserName)
            };

            var roles = await _identityService.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim("role", role)));

            return claims;
        }
    }
}