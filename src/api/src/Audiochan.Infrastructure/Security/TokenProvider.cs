using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Domain.Entities;
using Audiochan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Audiochan.Infrastructure.Security
{
    internal class TokenProvider : ITokenProvider
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ApplicationDbContext _unitOfWork;

        public TokenProvider(IOptions<JwtSettings> jwtOptions, 
            IDateTimeProvider dateTimeProvider, 
            TokenValidationParameters tokenValidationParameters, 
            ApplicationDbContext unitOfWork)
        {
            _jwtSettings = jwtOptions.Value;
            _dateTimeProvider = dateTimeProvider;
            _tokenValidationParameters = tokenValidationParameters;
            _unitOfWork = unitOfWork;
        }

        public (string, long) GenerateAccessToken(User user)
        {
            var claims = GetClaims(user);
            var expirationDate = _dateTimeProvider.Now.Add(_jwtSettings.AccessTokenExpiration);
            return GenerateToken(_jwtSettings.AccessTokenSecret, new ClaimsIdentity(claims), expirationDate);
        }

        public async Task<(string, long)> GenerateRefreshToken(User user, string tokenToBeRemoved = "", 
            CancellationToken cancellationToken = default)
        {
            var now = _dateTimeProvider.Now;
            var expirationDate = now.Add(_jwtSettings.RefreshTokenExpiration);

            var claims = GetClaims(user);

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
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return (token, expirationDateEpoch);
        }

        public async Task<bool> ValidateRefreshToken(string token, CancellationToken cancellationToken = default)
        {
            return await ValidateToken(token, cancellationToken);
        }

        private async Task<bool> ValidateToken(string token, CancellationToken cancellationToken = default)
        {
            // setup validation token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParams = _tokenValidationParameters.Clone();
            tokenValidationParams.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.RefreshTokenSecret));
            try
            {
                // validate token
                tokenHandler.ValidateToken(
                    token,
                    tokenValidationParams,
                    out var validatedToken);
                var jwtSecurityToken = (JwtSecurityToken) validatedToken;
                
                // Grab userid from token
                var userIdString = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
                
                // Validate userid format
                if (!long.TryParse(userIdString, out var userId) || userId <= 0)
                    return false;
                
                // Check if user id exists in database
                return await _unitOfWork.Users.AnyAsync(u => u.Id == userId, cancellationToken);
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

        private List<Claim> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new("name", user.UserName),
                new("role", user.Role.ToString())
            };
            
            return claims;
        }
    }
}