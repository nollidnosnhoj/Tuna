using System.Linq;
using Ardalis.Specification;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Features.Auth.Refresh
{
    public sealed class GetUserBasedOnRefreshTokenSpecification : Specification<User>
    {
        public GetUserBasedOnRefreshTokenSpecification(string refreshToken)
        {
            Query.Include(u => u.RefreshTokens)
                .Where(u => u.RefreshTokens.Any(r => r.Token == refreshToken && u.Id == r.UserId));
        }
    }
}