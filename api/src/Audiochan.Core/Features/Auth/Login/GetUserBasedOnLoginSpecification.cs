using Ardalis.Specification;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Features.Auth.Login
{
    public sealed class GetUserBasedOnLoginSpecification : Specification<User>
    {
        public GetUserBasedOnLoginSpecification(string login)
        {
            Query.Where(u => u.UserName == login.Trim().ToLower() || u.Email == login);
        }
    }
}